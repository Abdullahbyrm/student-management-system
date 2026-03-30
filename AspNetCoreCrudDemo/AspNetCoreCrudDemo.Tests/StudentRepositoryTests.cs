using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreCrudDemo.Data;
using AspNetCoreCrudDemo.Models;
using AspNetCoreCrudDemo.Repositories;
using Xunit;

namespace AspNetCoreCrudDemo.Tests
{
    public class StudentRepositoryTests
    {
        // Testler arasında state(veritabanı bilgisi) paylaşmamak için her teste ayrı (sıfır kilometre) 
        // bir InMemory veritabanı simülasyonu yaratan yardımcı metodumuz.
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddStudentToDatabase()
        {
            // 1. Arrange (Hazırlık)
            var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);
            var student = new Student 
            { 
                Name = "Ali Yılmaz", 
                Email = "ali@ornek.com", 
                Course = "C# ile .NET Core", 
                EnrollmentDate = DateTime.Now 
            };

            // 2. Act (Eylem - Aşçıya kaydet diyoruz)
            await repository.AddAsync(student);

            // 3. Assert (Doğrulama - Kaydolduğunu cetvelle ölçüyoruz)
            Assert.Equal(1, context.Students.Count());
            Assert.Equal("Ali Yılmaz", context.Students.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllStudents()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Students.AddRange(
                new Student { Name = "Ayşe", Email = "a@a.com", Course = "Java" },
                new Student { Name = "Fatma", Email = "f@f.com", Course = "Python" }
            );
            await context.SaveChangesAsync();
            var repository = new StudentRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnStudent_WhenIdExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var studentToFind = new Student { Name = "Mehmet", Email = "m@m.com", Course = "Flutter" };
            context.Students.Add(studentToFind);
            await context.SaveChangesAsync();
            var repository = new StudentRepository(context);

            // Act
            var result = await repository.GetByIdAsync(studentToFind.StudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Mehmet", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange (Bomboş bir veritabanı yaratıyoruz)
            var context = GetInMemoryDbContext();
            var repository = new StudentRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result); // Böyle biri yok, dönen sonuç kesinlikle NULL olmalı!
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStudentFromDatabase()
        {
            // Arrange (İçinde 1 kişi olan veritabanı)
            var context = GetInMemoryDbContext();
            var studentToDelete = new Student { Name = "Ahmet", Email = "a@a.com", Course = "React" };
            context.Students.Add(studentToDelete);
            await context.SaveChangesAsync();
            var repository = new StudentRepository(context);

            // Act (Silme emri)
            await repository.DeleteAsync(studentToDelete);

            // Assert (Doğrulama: 1 kişi varken 0 kişi (Empty) olduysa başarılıdır!)
            Assert.Empty(context.Students);
        }
    }
}
