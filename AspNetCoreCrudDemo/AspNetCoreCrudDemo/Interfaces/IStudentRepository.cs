using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Interfaces
{
    // C# Interfaces (Arayüzler): Bir sınıfın hangi yeteneklere (metotlara) sahip olması gerektiğini listeleyen "menülerdir".
    public interface IStudentRepository
    {
        // 1. Veritabanındaki tüm öğrencileri listeleyen yetenek
        Task<IEnumerable<Student>> GetAllAsync();
        
        // 2. Özel bir ID'ye sahip öğrenciyi arayıp getiren yetenek
        Task<Student?> GetByIdAsync(int? id);
        
        // 3. Veritabanına yeni bir öğrenci ekleyip kaydeden yetenek
        Task AddAsync(Student student);
        
        // 4. Var olan bir öğrencinin bilgilerini güncelleyen yetenek
        Task UpdateAsync(Student student);
        
        // 5. Bir öğrenciyi veritabanından kalıcı olarak silen yetenek
        Task DeleteAsync(Student student);
        
        // 6. Bu ID'ye sahip bir öğrencinin gerçekten var olup olmadığını kontrol eden yetenek
        bool StudentExists(int id);
    }
}
