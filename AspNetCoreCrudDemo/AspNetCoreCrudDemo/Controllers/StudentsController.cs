using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AspNetCoreCrudDemo.Interfaces;
using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Controllers
{
    // Controller sınıfımız `Controller` (MVC için) baz sınıfından türetilir.
    public class StudentsController : Controller
    {
        // Artık Mutfağa (DbContext) doğrudan girmiyoruz, Aşçımızı (Repository) çağırıyoruz.
        private readonly IStudentRepository _repository;
        private readonly ILogger<StudentsController> _logger;

        // Constructor Injection (Bağımlılık Enjeksiyonu). 
        public StudentsController(IStudentRepository repository, ILogger<StudentsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Öğrenciler listesi başarıyla görüntülendi.");
            // Garson aşçıdan tüm menüyü (verileri) getirmesini ister.
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,Name,Email,Course,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(student);
                
                _logger.LogInformation($"Sisteme yepyeni bir öğrenci kaydedildi: {student.Name}");
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Name,Email,Course,EnrollmentDate")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(student);
                    _logger.LogInformation($"{student.Name} isimli öğrencinin bilgileri güncellendi.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, $"{student.Name} güncellenirken eştarihli bir çakışma hatası (Concurrency) yaşandı!");
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id);
            
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student != null)
            {
                _logger.LogWarning($"DİKKAT: {student.Name} isimli öğrenci veritabanından kalıcı olarak SİLİNDİ!");
                await _repository.DeleteAsync(student);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _repository.StudentExists(id);
        }
    }
}
