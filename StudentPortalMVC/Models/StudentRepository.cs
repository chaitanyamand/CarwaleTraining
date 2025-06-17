using System.Collections.Generic;
using System.Linq;

namespace StudentPortal.Models
{
    public class StudentRepository : IStudentRepository
    {
        private readonly List<Student> _students = new()
        {
            new Student { Id = 1, Name = "Nikhil", Course = "Mathematics" },
            new Student { Id = 2, Name = "Nikki", Course = "Physics" },
            new Student { Id = 3, Name = "Nick", Course = "Chemistry" }
        };

        public List<Student> GetAll() => _students;

        public Student? GetById(int id) =>
            _students.FirstOrDefault(s => s.Id == id);
    }
}
