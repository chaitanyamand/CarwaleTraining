using StocksAPI.Enums;

namespace StocksAPI.Entities
{
    /*
        * Entity representing search filter criteria 
        * used to query stock listings from the database.
    */
    public class Filters
    {
        /* Minimum price in rupees (converted from lakhs on input) */
        public int? MinimumBudget { get; set; }

        /* Maximum price in rupees (converted from lakhs on input) */
        public int? MaximumBudget { get; set; }

        /* List of selected fuel types (e.g., Petrol, Diesel, EV) */
        public List<FuelType>? FuelTypes { get; set; }

        /* Car make (e.g., Honda, Maruti) */
        public string? MakeName { get; set; }

        /* Car model (e.g., City, Swift) */
        public string? ModelName { get; set; }

        /* Minimum manufacturing year */
        public int? MinYear { get; set; }

        /* Maximum manufacturing year */
        public int? MaxYear { get; set; }

        /* Maximum kilometers driven */
        public int? MaxKilometers { get; set; }

        /* City or location name */
        public string? Location { get; set; }
    }
}