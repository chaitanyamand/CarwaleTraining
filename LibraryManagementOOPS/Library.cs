//Practice for OOPS in C#

namespace Program
{
    class Library
    {
        public static void Main(string[] args)
        {
            Admin admin = new Admin();
            Member member = new Member();

            Book book1 = new Book("The Hobbit", "J.R.R. Tolkien");
            Book book2 = new Book("Clean Code", "Robert C. Martin");

            admin.addBook(book1);
            admin.addBook(book2);

            foreach (Book book in admin.showBooks())
                book.DisplayDetail();

            bool borrowed = admin.borrowBook(book2, member);
            Console.WriteLine(borrowed ? "Borrowed successfully" : "Borrow failed");

            foreach (Book book in admin.showBooks())
                book.DisplayDetail();

            bool returned = admin.returnBook(book2, member);
            Console.WriteLine(returned ? "Returned successfully" : "Return failed");

            foreach (Book book in admin.showBooks())
                book.DisplayDetail();
        }

    }

    class Book
    {
        private string Title { get; set; }
        private string Author { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Book(string title, string author)
        {
            this.Title = title;
            this.Author = author;
        }

        public void DisplayDetail()
        {
            Console.WriteLine($"Book Title : {this.Title}, Book Author : {this.Author}, Availability : {this.IsAvailable}");
        }
    }

    abstract class User
    {
        public string Name { get; private set; }

        public virtual void DisplayRole()
        {
            Console.WriteLine("Role : User");
        }
    }

    class Member : User
    {
        List<Book> booksBorrowed = new List<Book>();

        public bool borrowBook(Book book)
        {
            if (book.IsAvailable)
            {
                booksBorrowed.Add(book);
                book.IsAvailable = false;
                return true;
            }
            return false;
        }

        public bool returnBook(Book book)
        {
            if (!book.IsAvailable)
            {
                if (!booksBorrowed.Remove(book)) return false;
                book.IsAvailable = true;
                return true;
            }
            return false;
        }

        public override void DisplayRole()
        {
            Console.WriteLine("Role : Library Member");

        }

    }

    class Admin : User
    {
        List<Book> booksAvailableInLibrary = new List<Book>();
        Dictionary<Book, User> bookToMemberMap = new Dictionary<Book, User>();

        public void addBook(Book book)
        {
            booksAvailableInLibrary.Add(book);
        }

        public bool borrowBook(Book book, Member member)
        {
            if (booksAvailableInLibrary.Contains(book) && book.IsAvailable && !bookToMemberMap.ContainsKey(book))
            {
                if (!member.borrowBook(book)) return false;
                this.bookToMemberMap[book] = member;
                return true;
            }
            return false;
        }

        public bool returnBook(Book book, Member member)
        {
            if (booksAvailableInLibrary.Contains(book) && !book.IsAvailable && bookToMemberMap[book] == member)
            {
                if (!member.returnBook(book)) return false;
                bookToMemberMap.Remove(book);
                return true;
            }
            return false;
        }

        public List<Book> showBooks()
        {
            return booksAvailableInLibrary;
        }

        public override void DisplayRole()
        {
            Console.WriteLine("Role : Admin");

        }

    }
}