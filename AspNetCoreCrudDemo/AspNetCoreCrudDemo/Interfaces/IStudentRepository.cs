using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int? id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        bool StudentExists(int id);

        // Arama: İsim veya e-posta içinde anahtar kelime geçen öğrencileri bul
        Task<IEnumerable<Student>> SearchAsync(string query);
    }
}
