namespace LINQ
{
    class LINQ
    {
        static void MainTest(string[] args)
        {
            List<Person> people = DataManager.GetPeople();
            List<Student> students = DataManager.GetStudents();
            List<School> schools = DataManager.GetSchools();

            people.ForEach(p => Console.WriteLine(p.ToString()));
            Console.WriteLine("");

            IEnumerable<string> names = people.Select(p => p.Name);
            foreach (string name in names) Console.WriteLine(name);

            people = people.OrderBy(p => p.City).ThenBy(p => p.Name).ToList();
            Console.WriteLine("");
            people.ForEach(p => Console.WriteLine(p.ToString()));

            people = people.Where(p => p.Age > 25).ToList();
            Console.WriteLine("");
            people.ForEach(p => Console.WriteLine(p.ToString()));

            Person bob = people.First(p => p.Name == "Bob"); // exception if not found
            Console.WriteLine("");
            Console.WriteLine(bob.ToString());

            Person emily = people.FirstOrDefault(p => p.Name == "Emily"); //safe find
            if (emily == null)
            {
                Console.WriteLine("Emily Not Found");
            }
            else
            {
                Console.WriteLine("EmilyFound");
            }
            // Similarly Last and LastOrDefault

            people = people.Skip(2).Take(3).ToList();
            Console.WriteLine("");
            people.ForEach(p => Console.WriteLine(p.ToString()));

            people = people.DistinctBy(p => p.JobTitle).ToList();
            Console.WriteLine("");
            people.ForEach(p => Console.WriteLine(p.ToString()));


            // Returns Boolean
            Console.WriteLine(people.Any(p => p.Age > 30));
            Console.WriteLine(people.All(p => p.Age > 25));

            // Aggregates
            Console.WriteLine(people.Sum(p => p.Age));
            Console.WriteLine(people.Max(p => p.Age));
            Console.WriteLine(people.Min(p => p.Age));
            Console.WriteLine(people.Average(p => p.Age));

            Console.WriteLine(people.MinBy(p => p.Age).ToString());


            // Joins
            var joinQuery = from s in schools
                            join st in students on s.SchoolID equals st.SchoolID
                            select new { StudentName = st.Name, SchoolName = s.Name };
            foreach (var studentschool in joinQuery)
            {
                Console.WriteLine($"\tStudent Name: {studentschool.StudentName}, School Name: {studentschool.SchoolName}");
            }

            //Group Join
            var groupJoinQuery = from s in schools
                                 join st in students on s.SchoolID equals st.SchoolID into schoolGroup
                                 select new { SchoolName = s.Name, Students = schoolGroup };
            foreach (var school in groupJoinQuery)
            {
                Console.WriteLine($"School Name: {school.SchoolName}");
                foreach (var student in school.Students)
                {
                    Console.WriteLine($"\tStudent Id: {student.StudentID}, Name: {student.Name}");
                }
            }
        }

    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string JobTitle { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Age: {Age}, City: {City}, Job: {JobTitle}";
        }

    }

    public class School
    {
        public int SchoolID { get; set; }
        public string Name { get; set; }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public int SchoolID { get; set; }
        public string Name { get; set; }
    }

    public class DataManager
    {
        public static List<Person> GetPeople()
        {
            return new List<Person>
            {
                new Person { Name = "Alice", Age = 28, City = "New York", JobTitle = "Engineer" },
                new Person { Name = "Bob", Age = 35, City = "Chicago", JobTitle = "Manager" },
                new Person { Name = "Charlie", Age = 22, City = "New York", JobTitle = "Intern" },
                new Person { Name = "Diana", Age = 30, City = "Los Angeles", JobTitle = "Engineer" },
                new Person { Name = "Ethan", Age = 40, City = "Chicago", JobTitle = "CEO" },
                new Person { Name = "Fiona", Age = 27, City = "New York", JobTitle = "Designer" },
                new Person { Name = "George", Age = 33, City = "Seattle", JobTitle = "Engineer" },
                new Person { Name = "Hannah", Age = 29, City = "Chicago", JobTitle = "Designer" },
            };
        }

        public static List<School> GetSchools()
        {
            return new List<School> {
                new School{ SchoolID = 1, Name = "Springfield High" },
                new School{ SchoolID = 2, Name = "Westfield Academy" },
                new School{ SchoolID = 3, Name = "Dot Net School" }
            };
        }

        public static List<Student> GetStudents()
        {
            return new List<Student> {
                new Student{ StudentID = 1, SchoolID = 1, Name = "John Doe" },
                new Student{ StudentID = 2, SchoolID = 1, Name = "Jane Smith" },
                new Student{ StudentID = 3, SchoolID = 2, Name = "Will Johnson" },
                new Student{ StudentID = 4, SchoolID = 3, Name = "Sara Taylor" },
                new Student{ StudentID = 5, SchoolID = 3, Name = "Steven Smith" }
            };
        }
    } 
}