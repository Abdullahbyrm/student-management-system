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

        // 4. Yeni 'Users' tablomuzu da aynı şekilde tanıtıyoruz.
        public DbSet<User> Users { get; set; }

        // 5. Kurslar tablomuz
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Başlangıç Kurs Verileri (Seed Data)
            modelBuilder.Entity<Course>().HasData(
                new Course { 
                    CourseId = 1, 
                    Title = "C# ile .NET Core (Backend)", 
                    Description = "Modern web uygulamaları ve güçlü API'ler geliştirmek için C# ve .NET Core ekosistemini sıfırdan zirveye öğrenin. Katmanlı mimari, Entity Framework ve Onion Architecture prensiplerine hakim olun.", 
                    Category = "Yazılım", 
                    Duration = "120 Saat", 
                    ImageUrl = "/images/csharp_backend.png",
                    Rating = 4.9 
                },
                new Course { 
                    CourseId = 2, 
                    Title = "React & Node.js (Full Stack)", 
                    Description = "Günümüzün en popüler JavaScript kütüphaneleriyle uçtan uca modern ve dinamik web platformları inşa edin. MERN stack ile gerçek projeler üzerinden Full Stack yetkinliği kazanın.", 
                    Category = "Web", 
                    Duration = "95 Saat", 
                    ImageUrl = "/images/react_nodejs.png",
                    Rating = 4.8 
                },
                new Course { 
                    CourseId = 3, 
                    Title = "Python ile Veri Bilimi (Data Science)", 
                    Description = "Büyük veriyi analiz edin, yapay zeka modelleri kurun ve geleceğin mesleği olan veri biliminde uzmanlaşın. Pandas, NumPy ve Scikit-learn kütüphanelerini ustalıkla kullanın.", 
                    Category = "Yapay Zeka", 
                    Duration = "80 Saat", 
                    ImageUrl = "/images/python_data.png",
                    Rating = 4.9 
                },
                new Course { 
                    CourseId = 4, 
                    Title = "Java Spring Boot (Kurumsal Web)", 
                    Description = "Büyük şirketlerin ve bankaların vazgeçilmez altyapısı olan Java Spring Boot ile kurumsal mimariye adım atın. Microservices ve Cloud-Native geliştirme temellerini öğrenin.", 
                    Category = "Kurumsal", 
                    Duration = "110 Saat", 
                    ImageUrl = "/images/java_spring.png",
                    Rating = 4.7 
                },
                new Course { 
                    CourseId = 5, 
                    Title = "Flutter & Dart (Mobil Uygulama)", 
                    Description = "Tek bir kod tabanından hem iOS hem de Android için native performansında muhteşem mobil uygulamalar geliştirin. Cross-platform dünyasında aranan bir geliştirici olun.", 
                    Category = "Mobil", 
                    Duration = "85 Saat", 
                    ImageUrl = "/images/flutter_dart.png",
                    Rating = 4.9 
                }
            );
        }
    }
}
