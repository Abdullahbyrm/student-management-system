using AspNetCoreCrudDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreCrudDemo.Data
{
    // 1. Sınıfımızı DbContext isimli "Ana Çevirmen" sınıfından türetiyoruz (Miras Alıyoruz).
    public class ApplicationDbContext : DbContext
    {
        // 2. Bir nevi kurulum ayarı (ConnectionString gibi şeyleri içeri almak için yapıcı metot)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 3. Veritabanındaki 'Students' tablosu, bizim C# tarafındaki 'Student' modeli ile eşleşecek diyoruz.
        public DbSet<Student> Students { get; set; }
    }
}
