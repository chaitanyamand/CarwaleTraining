namespace FinanceService.Entities
{
    /*
     * Represents the value-for-money status of a car.
     * Used as a result object for business logic evaluation.
     */
    public class CarStatusEntity
    {
        // Unique identifier of the car
        public int Id { get; set; }

        // Indicates whether the car is considered value for money
        public bool IsValueForMoney { get; set; }
    }
}
