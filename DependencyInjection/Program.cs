
namespace DIExample
{
    public interface IEngine
    {
        void Start();
    }

    public class PetrolEngine : IEngine
    {
        public void Start()
        {
            Console.WriteLine("Petrol engine started.");
        }
    }

    public class Car
    {
        private IEngine _engine;
        // private IEngine Engine {get ; set;} // Property DI
        public Car(IEngine engine) // Constructor DI
        {
            _engine = engine;
        }

        public void Drive()
        {
            _engine.Start();
            Console.WriteLine("Car is driving...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IEngine engine = new PetrolEngine();
            Car car = new Car(engine);
            car.Drive();
        }
    }
}
