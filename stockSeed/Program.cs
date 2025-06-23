using System.Text.Json;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace StockSeeder;

public enum FuelType
{
    Petrol = 1,
    Diesel = 2,
    CNG = 3,
    LPG = 4,
    Electric = 5,
    Hybrid = 6
}

public class Stock
{
    public string MakeName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int MakeYear { get; set; }
    public decimal Price { get; set; }
    public int Kilometers { get; set; }
    public FuelType FuelType { get; set; } = FuelType.Petrol;
    public string CityName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> StockImages { get; set; } = new();
    public string EmiText { get; set; } = string.Empty;
    public string TagText { get; set; } = string.Empty;
}

public class ApiResponse
{
    public List<ApiStock> Stocks { get; set; } = new();
    public string? NextPageUrl { get; set; }
}

public class ApiStock
{
    public string MakeName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int MakeYear { get; set; }
    public string PriceNumeric { get; set; } = "0";
    public string KmNumeric { get; set; } = "0";
    public string Fuel { get; set; } = "Petrol";
    public string AreaName { get; set; } = "";
    public string CityName { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public List<string> StockImages { get; set; } = new();
    public string EmiText { get; set; } = "";
    public string TagText { get; set; } = "";
}

class Program
{
    private const string BaseUrl = "https://stg.carwale.com";
    private const string FirstPage = "/api/stocks/";
    private const string ConnectionString = "Server=localhost;Database=stocks;Uid=root;Pwd=user123;";

    static async Task Main()
    {
        var client = new HttpClient();
        var nextPage = FirstPage;
        var totalInserted = 0;

        for (int i = 0; i < 10 && !string.IsNullOrWhiteSpace(nextPage); i++)
        {
            try
            {
                Console.WriteLine($"Fetching: {nextPage}");
                var response = await client.GetStringAsync(BaseUrl + nextPage);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ApiResponse>(response, options);

                if (result?.Stocks == null || result.Stocks.Count == 0)
                {
                    Console.WriteLine("No stocks found.");
                    break;
                }

                foreach (var apiStock in result.Stocks)
                {
                    try
                    {
                        var stock = MapToStock(apiStock);

                        if (stock.MakeYear < 1990 || stock.MakeYear > DateTime.UtcNow.Year + 1)
                        {
                            Console.WriteLine($"Skipping unrealistic year: {stock.MakeYear}");
                            continue;
                        }

                        var stockId = await SaveStockAsync(stock);
                        totalInserted++;
                        Console.WriteLine($"Inserted: {stock.MakeName} {stock.ModelName} ({stockId})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to insert stock: {ex.Message}");
                    }
                }

                nextPage = result.NextPageUrl;
                await Task.Delay(300);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching page: {ex.Message}");
                break;
            }
        }

        Console.WriteLine($"Finished. Total inserted: {totalInserted}");
    }

    static Stock MapToStock(ApiStock e)
    {
        if (!Enum.TryParse<FuelType>(e.Fuel, true, out var fuel))
        {
            Console.WriteLine($"Unknown fuel type '{e.Fuel}', defaulting to Petrol.");
            fuel = FuelType.Petrol;
        }

        return new Stock
        {
            MakeName = e.MakeName,
            ModelName = e.ModelName,
            MakeYear = e.MakeYear,
            Price = decimal.TryParse(e.PriceNumeric, out var price) ? price : 0,
            Kilometers = int.TryParse(e.KmNumeric, out var kms) ? kms : 0,
            FuelType = fuel,
            CityName = string.IsNullOrWhiteSpace(e.CityName) ? "Unknown" : e.CityName,
            ImageUrl = e.ImageUrl,
            StockImages = e.StockImages ?? new(),
            EmiText = e.EmiText,
            TagText = e.TagText
        };
    }

    static async Task<int> SaveStockAsync(Stock stock)
    {
        using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        var sql = @"
INSERT INTO stocks (
    MakeName, ModelName, MakeYear, Price, Kilometers, FuelType,
    CityName, CreatedDate, IsActive, ImageUrl, StockImages, EmiText, TagText
)
VALUES (
    @MakeName, @ModelName, @MakeYear, @Price, @Kilometers, @FuelType,
    @CityName, @CreatedDate, @IsActive, @ImageUrl, @StockImages, @EmiText, @TagText
);
SELECT LAST_INSERT_ID();";

        var parameters = new
        {
            stock.MakeName,
            stock.ModelName,
            stock.MakeYear,
            stock.Price,
            stock.Kilometers,
            FuelType = (int)stock.FuelType,
            stock.CityName,
            stock.CreatedDate,
            stock.IsActive,
            stock.ImageUrl,
            StockImages = JsonSerializer.Serialize(stock.StockImages),
            stock.EmiText,
            stock.TagText
        };

        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }
}
