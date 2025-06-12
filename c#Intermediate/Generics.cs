//Generics Prac

namespace Generics 
{
    class Generics 
    {
        public static void MainTest(string[] args)
        {
            var mobileStore = new Store<Mobile>();
            mobileStore.AddProduct(new Mobile("M001", "Apple", "2024-01-01"), "M001");

            var bookStore = new Store<Book>();
            bookStore.AddProduct(new Book("C# in Depth", "Random", "B001"), "B001");

            var mobile = mobileStore.GetProductById("M001");
            Console.WriteLine($"Mobile: {mobile.companyName}, Released: {mobile.releaseDate}");

            var book = bookStore.GetProductById("B001");
            Console.WriteLine($"Book: {book.Title} by {book.Author}");
        }
    }

    class Product
    {
        public Product(string Id)
        {
            this.Id = Id;
        }
        public string Id { get; set; }
    }

    class Mobile : Product
    {
        public string companyName;
        public string releaseDate;

        public Mobile(string Id, string companyName, string releaseDate) : base(Id)
        {
            this.companyName = companyName;
            this.releaseDate = releaseDate;
        }
    }

    class Book : Product
    {
        public string Title;
        public string Author;

        public Book(string title, string author, string Id) : base(Id)
        {
            this.Title = title;
            this.Author = author;
        }
    }

    class Store<T> where T : Product
    {
        Dictionary<string, T> products = new Dictionary<string, T>();

        public void AddProduct(T product, string Id)
        {
            products.Add(Id, product);
        }

        public T GetProductById(string Id)
        {
            return products.ContainsKey(Id) ? products[Id] : null;
        }
    }
}
