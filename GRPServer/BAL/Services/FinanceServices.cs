using FinanceService.BAL.Interfaces;
using FinanceService.DAL.Interfaces;
using FinanceService.Enums;
using FinanceService.Entities;

namespace FinanceService.BAL.Repositories
{
    /*
     * Business Access Layer implementation for finance-related operations.
     * Handles business rules to evaluate car value-for-money status.
     */
    public class FinanceBAL : IFinanceBAL
    {
        private readonly IStockDAL _stockRepository;
        private readonly ILogger<FinanceBAL> _logger;

        // Constructor injecting the stock data access layer and logger
        public FinanceBAL(IStockDAL stockRepository, ILogger<FinanceBAL> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        /*
         * Evaluates which cars from the provided list are considered value for money
         * based on business rules (age, mileage, price, fuel type).
         */
        public async Task<List<CarStatusEntity>> GetCarValueForMoneyStatusAsync(List<int> carIds)
        {
            try
            {
                _logger.LogInformation($"Processing value for money check for {carIds.Count} car IDs");

                // Retrieve stock information for the provided car IDs
                var stocks = await _stockRepository.GetStocksByIdsAsync(carIds);

                if (!stocks.Any())
                {
                    _logger.LogWarning("No stocks found for the provided car IDs");
                    return new List<CarStatusEntity>();
                }

                // Evaluate each stock and prepare result list
                var carStatuses = new List<CarStatusEntity>();

                foreach (var stock in stocks)
                {
                    bool isValueForMoney = DetermineValueForMoney(stock);

                    carStatuses.Add(new CarStatusEntity
                    {
                        Id = stock.Id,
                        IsValueForMoney = isValueForMoney
                    });
                }

                _logger.LogInformation($"Successfully processed {carStatuses.Count} car statuses");
                return carStatuses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing car value for money status");
                throw new Exception($"Business logic error: {ex.Message}", ex);
            }
        }

        /*
         * Contains the decision logic to determine whether a single car is value for money.
         * Evaluates age, mileage, price, and fuel type against set criteria.
         */
        private bool DetermineValueForMoney(Stock stock)
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var carAge = currentYear - stock.MakeYear;

                // Business rule checks
                bool ageCheck = carAge <= 5;
                bool mileageCheck = stock.Kilometers < 100000;
                bool priceCheck = stock.Price > 0 && stock.Price <= 2000000;
                bool fuelEfficiencyCheck = stock.FuelType == FuelEnum.Electric ||
                                           stock.FuelType == FuelEnum.Hybrid ||
                                           stock.FuelType == FuelEnum.CNG;

                // Count number of criteria met
                int criteriaMetCount = 0;
                if (ageCheck) criteriaMetCount++;
                if (mileageCheck) criteriaMetCount++;
                if (priceCheck) criteriaMetCount++;
                if (fuelEfficiencyCheck) criteriaMetCount++;

                // A car is value for money if 3 or more criteria are met
                return criteriaMetCount >= 3;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error determining value for money for stock ID: {stock.Id}");
                return false;
            }
        }
    }
}
