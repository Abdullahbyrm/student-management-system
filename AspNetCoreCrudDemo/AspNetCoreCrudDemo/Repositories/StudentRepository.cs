using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreCrudDemo.Data;
using AspNetCoreCrudDemo.Interfaces;
using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Repositories
{
    // Bu sınıf(Aşçı), üstteki IStudentRepository (Menü) içindeki kuralları mecburi olarak barındırmak zorundadır.
    public class StudentRepository : IStudentRepository
    {
        // Mutfak Dolabımız (Veritabanı)
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int? id)
        {
            if (id == null) return null;
            return await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);
        }

        public async Task AddAsync(Student student)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
