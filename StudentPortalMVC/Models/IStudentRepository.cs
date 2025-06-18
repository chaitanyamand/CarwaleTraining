using System.Collections.Generic;

namespace StudentPortal.Models
{
    public interface IStudentRepository
    {
        List<Student> GetAll();
        Student? GetById(int id);
        void Add(Student student);
    }
}
