using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AspNetCoreCrudDemo.Interfaces;
using AspNetCoreCrudDemo.Models;

using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreCrudDemo.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _repository;
        private readonly ILogger<StudentsController> _logger;
        private readonly IWebHostEnvironment _env;

        public StudentsController(IStudentRepository repository, ILogger<StudentsController> logger, IWebHostEnvironment env)
        {
            _repository = repository;
            _logger = logger;
            _env = env;
        }

        // GET: Students
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Öğrenciler listesi başarıyla görüntülendi.");
            return View(await _repository.GetAllAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details sayfası için geçersiz giriş yapıldı (Geçersiz ID).");
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning($"{id} ID'li öğrenci arandı ama bulunamadı!");
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("StudentId,Name,Email,Course,EnrollmentDate")] Student student, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    student.PhotoPath = await SavePhoto(Photo);
                }

                await _repository.AddAsync(student);
                _logger.LogInformation($"Sisteme yepyeni bir öğrenci kaydedildi: {student.Name}");
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Name,Email,Course,EnrollmentDate,PhotoPath")] Student student, IFormFile? Photo)
        {
            if (id != student.StudentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Photo != null && Photo.Length > 0)
                    {
                        // Eski fotoğrafı sil
                        DeletePhotoFile(student.PhotoPath);
                        student.PhotoPath = await SavePhoto(Photo);
                    }

                    await _repository.UpdateAsync(student);
                    _logger.LogInformation($"{student.Name} isimli öğrencinin bilgileri güncellendi.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "{StudentName} güncellenirken çakışma hatası yaşandı!", student.Name);
                    if (!StudentExists(student.StudentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Model geçersizse hataları logla (Hata tespiti için ekledik)
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Güncelleme Hatası: {ErrorMessage}", error.ErrorMessage);
            }

            return View(student);
        }

        // POST: Students/DeletePhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            DeletePhotoFile(student.PhotoPath);
            student.PhotoPath = null;
            await _repository.UpdateAsync(student);

            _logger.LogInformation($"{student.Name} öğrencisinin fotoğrafı silindi.");
            return RedirectToAction(nameof(Edit), new { id });
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student != null)
            {
                DeletePhotoFile(student.PhotoPath);
                _logger.LogWarning($"DİKKAT: {student.Name} isimli öğrenci kalıcı olarak SİLİNDİ!");
                await _repository.DeleteAsync(student);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Search?q=ali  (AJAX API)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string q)
        {
            var results = await _repository.SearchAsync(q ?? "");
            var data = results.Select(s => new
            {
                s.StudentId,
                s.Name,
                s.Email,
                s.Course,
                s.PhotoPath
            });
            return Json(data);
        }

        // ===== Yardımcı Metodlar =====

        private async Task<string> SavePhoto(IFormFile photo)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return "/uploads/students/" + fileName;
        }

        private void DeletePhotoFile(string? photoPath)
        {
            if (string.IsNullOrEmpty(photoPath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, photoPath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private bool StudentExists(int id)
        {
            return _repository.StudentExists(id);
        }
    }
}
