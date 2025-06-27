using AutoMapper;
using StocksAPI.DTOs;
using StocksAPI.Entities;
using StocksAPI.Enums;

namespace StocksAPI.Mappings
{
    /*
     * AutoMapper profile for converting between DTOs and entities.
     * Handles logic for mapping filters, stock entities, and enriched fields.
     */
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map incoming search request DTO to internal filter entity
            CreateMap<StockSearchRequestDTO, Filters>()
                .ForMember(dest => dest.MinimumBudget, opt => opt.MapFrom(src => GetMinBudget(src.Budget)))
                .ForMember(dest => dest.MaximumBudget, opt => opt.MapFrom(src => GetMaxBudget(src.Budget)))
                .ForMember(dest => dest.FuelTypes, opt => opt.MapFrom(src => ParseFuelTypes(src.Fuel)));

            // Map stock entity to outward-facing stock DTO
            CreateMap<Stock, StockDTO>()
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id)) 
            .ForMember(dest => dest.Km, opt => opt.MapFrom(src => src.Kilometers))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityName))
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src => FormatPrice(src.Price))) // Custom currency format
            .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => $"{src.MakeYear} {src.MakeName} {src.ModelName}")) // Concatenated name                .ForMember(dest => dest.Fuel, opt => opt.MapFrom(src => src.FuelType.ToString())) // Enum to string
            .ForMember(dest => dest.IsValueForMoney, opt => opt.Ignore()) // Set In BAL
            .ForMember(dest=>dest.Fuel, opt => opt.MapFrom(src=> src.FuelType.ToString())); // Business logic, handled separately
        }

        /*
         * Parses a '+'-separated string of fuel type values (e.g. "1+2+3")
         * Converts them into a list of FuelType enum values.
         */
        private static List<FuelType>? ParseFuelTypes(string? fuelTypesString)
        {
            if (string.IsNullOrWhiteSpace(fuelTypesString))
                return null;

            var fuelTypes = new List<FuelType>();
            var fuelTypeStrings = fuelTypesString.Split('+', StringSplitOptions.RemoveEmptyEntries);

            foreach (var fuelTypeString in fuelTypeStrings)
            {
                if (Enum.TryParse<FuelType>(fuelTypeString.Trim(), true, out var fuelType))
                {
                    fuelTypes.Add(fuelType);
                }
            }

            return fuelTypes.Any() ? fuelTypes : null;
        }

        /*
         * Formats a raw price value into Indian currency short form:
         * Examples: 15,000 -> Rs. 15.0 K, 1,20,000 -> Rs. 1.2 L
         */
        private static string FormatPrice(decimal price)
        {
            if (price >= 10000000) // 1 Crore
            {
                return $"Rs. {price / 10000000:F1} Cr";
            }
            else if (price >= 100000) // 1 Lakh
            {
                return $"Rs. {price / 100000:F1} Lakh";
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

        /*
         * Extracts the minimum budget from a budget string in the format "min-max".
         * Example: "100000-500000" => 100000
         */
        private static int? GetMinBudget(string? budget)
        {
            if (string.IsNullOrWhiteSpace(budget))
                return null;

            var budgetParts = budget.Split('-');
            if (budgetParts.Length == 2 && int.TryParse(budgetParts[0], out var minBudget))
            {
                return minBudget;
            }
            return null;
        }

        /*
         * Extracts the maximum budget from a budget string in the format "min-max".
         * Example: "100000-500000" => 500000
         */
        private static int? GetMaxBudget(string? budget)
        {
            if (string.IsNullOrWhiteSpace(budget))
                return null;

            var budgetParts = budget.Split('-');
            if (budgetParts.Length == 2 && int.TryParse(budgetParts[1], out var maxBudget))
            {
                return maxBudget;
            }
            return null;
        }
    }
}
