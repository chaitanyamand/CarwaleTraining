using AutoMapper;
using StocksAPI.DTOs;
using StocksAPI.Entities;
using StocksAPI.Enums;

namespace StocksAPI.Mappings
{
    // AutoMapper profile for mapping between DTOs and Entities
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map StockSearchRequestDTO to Filters entity
            CreateMap<StockSearchRequestDTO, Filters>()
                .ForMember(dest => dest.MinimumBudget, opt => opt.MapFrom(src => src.MinBudget))
                .ForMember(dest => dest.MaximumBudget, opt => opt.MapFrom(src => src.MaxBudget))
                .ForMember(dest => dest.FuelTypes, opt => opt.MapFrom(src => ParseFuelTypes(src.FuelTypes)));

            // Map Stock entity to StockDTO
            CreateMap<Stock, StockDTO>()
                .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src => FormatPrice(src.Price)))
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => $"{src.MakeYear} {src.MakeName} {src.ModelName}"))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType.ToString()))
                .ForMember(dest => dest.IsValueForMoney, opt => opt.Ignore()); // Set in BAL
        }

        // Parse comma-separated fuel types string to list of FuelType enum
        private static List<FuelType>? ParseFuelTypes(string? fuelTypesString)
        {
            if (string.IsNullOrWhiteSpace(fuelTypesString))
                return null;

            var fuelTypes = new List<FuelType>();
            var fuelTypeStrings = fuelTypesString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var fuelTypeString in fuelTypeStrings)
            {
                if (Enum.TryParse<FuelType>(fuelTypeString.Trim(), true, out var fuelType))
                {
                    fuelTypes.Add(fuelType);
                }
            }

            return fuelTypes.Any() ? fuelTypes : null;
        }

        // Format price to Indian currency format
        private static string FormatPrice(decimal price)
        {
            if (price >= 10000000) // 1 Crore
            {
                return $"Rs. {price / 10000000:F1} Cr";
            }
            else if (price >= 100000) // 1 Lakh
            {
                return $"Rs. {price / 100000:F1} L";
            }
            else if (price >= 1000) // 1 Thousand
            {
                return $"Rs. {price / 1000:F1} K";
            }
            else
            {
                return $"Rs. {price:F0}";
            }
        }
    }
}