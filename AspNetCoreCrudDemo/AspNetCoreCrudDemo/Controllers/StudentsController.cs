using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AspNetCoreCrudDemo.Data;
using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Controllers
{
    // Controller sınıfımız `Controller` (MVC için) baz sınıfından türetilir.
    public class StudentsController : Controller
    {
        // Veritabanı ile konuşacak olan aracımız (Context)
        private readonly ApplicationDbContext _context;
        // Olan biteni kara kaplı deftere kaydeden müdürümüz (Logger)
        private readonly ILogger<StudentsController> _logger;

        // Constructor Injection (Bağımlılık Enjeksiyonu). 
        // Program.cs içerisindeki AddDbContext ile oluşturulan DbContext buraya otomatik olarak verilir.
        public StudentsController(ApplicationDbContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Students
        // 1. Index Metodu (Listeleme): Veritabanındaki tüm öğrencileri alıp View'a (Ekrana) gönderir.
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Öğrenciler listesi başarıyla görüntülendi.");
            // _context.Students verilerin bulunduğu DbSet. ToListAsync() ile veriler çekilir.
            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        // 2. Details Metodu: URL'den gelen `id` parametresine göre öğrencinin bilgilerini getirir.
        public async Task<IActionResult> Details(int? id)
        {
            // Eğer id gelmediyse ya da veritabanı tablosu yoksa 404 sayfasına (NotFound) yönlendirilmesi istenir.
            if (id == null || _context.Students == null)
            {
                _logger.LogWarning("Details sayfası için geçersiz giriş yapıldı (Böyle bir tablo ya da ID yok).");
                return NotFound();
            }

            // Gelen id'ye ait ilk öğrenciyi (veya varsayılan olarak null) bulur.
            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            
            if (student == null) // Öyle bir id veritabanında yoksa
            {
                _logger.LogWarning($"{id} ID'li öğrenci arandı ama bulunamadı!");
                return NotFound();
            }

            // Verileri bulduysa, detayları göstermek için View'a nesneyi paslar.
            return View(student);
        }

        // GET: Students/Create
        // 3. Create Metodu (GET): Sadece boş bir kayıt ekleme formunu ekranda göstermekle sorumludur.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // 4. Create Metodu (POST): Forma girilen verilerin veritabanına kaydedilmesini sağlar.
        // [HttpPost] formun Submit edildiğini gösterir.
        // [ValidateAntiForgeryToken] CSRF (Siteler arası istek sahtekarlığı) saldırılarına karşı korur.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,Name,Email,Course,EnrollmentDate")] Student student)
        {
            // Model Validasyonu: Student.cs modelinde yazdığımız [Required] gibi attribute'lara uyuyor mu?
            if (ModelState.IsValid)
            {
                _context.Add(student); // Yeni veriyi context'e ekle
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına yaz (SQL INSERT işlemi yapar)
                
                _logger.LogInformation($"Sisteme yepyeni bir öğrenci kaydedildi: {student.Name}");
                
                // Başarılı olursa PRG (Post-Redirect-Get) pattern gereği Index (listeleme) sayfasına yönlendir.
                return RedirectToAction(nameof(Index));
            }
            // Eğer validasyon kurallarına uyulmamışsa (isim boş vs.) aynı formu hatalarla beraber geri getir.
            return View(student);
        }

        // GET: Students/Edit/5
        // 5. Edit Metodu (GET): Düzenlenmek istenen öğrenciyi id'sine göre bulur ve düzenleme formunda gösterir.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id); // Primary Key (ID) ile hızlıca arama yapar
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // 6. Edit Metodu (POST): Formdan kullanıcının değiştirdiği verileri alıp veritabanında günceller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Name,Email,Course,EnrollmentDate")] Student student)
        {
            // URL'deki ID ile gönderilen form (model) içindeki ID uyumsuzsa, muhtemelen bir oynama vardır.
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student); // Veriyi güncel hale getir
                    await _context.SaveChangesAsync(); // Veritabanına yaz (SQL UPDATE komutu çalıştırır)
                    _logger.LogInformation($"{student.Name} isimli öğrencinin bilgileri güncellendi.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Concurrency (Eşzamanlılık): Biz kaydederken başkası silmiş olabilir mi? kontrolü.
                    _logger.LogError(ex, $"{student.Name} güncellenirken eştarihli bir çakışma hatası (Concurrency) yaşandı!");
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Başka bir hata türü varsa fırlat.
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // İşlem başarılı.
            }
            return View(student); // Kurallara uymuyorsa hatalarla birlikte tekrar göster.
        }

        // GET: Students/Delete/5
        // 7. Delete Metodu (GET): Silme sayfasına gidildiğinde gerçekten "bunu mu silmek istiyorsun?" demek için veriyi bulur ve o sayfaya gönderir.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        // 8. DeleteConfirmed Metodu (POST): Silme işlemine onay verdiğinde POST isteği gelir ve veritabanından kalıcı olarak uçar.
        // ActionName("Delete") dememizin sebebi, method isimleri (imzaları) aynı olmasın (her ikisi de int aldığı için).
        // Fakat route olarak (URL üzerinde) yine /Students/Delete bekliyor olacağız.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            
            var student = await _context.Students.FindAsync(id); // ID'yi bul
            // Öğrenci varsa bağlamdan (context) çıkar
            if (student != null)
            {
                _logger.LogWarning($"DİKKAT: {student.Name} isimli öğrenci veritabanından kalıcı olarak SİLİNDİ!");
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync(); // Silme işlemini veritabanında kesinleştir (SQL DELETE).
            return RedirectToAction(nameof(Index));
        }

        // Yardımıcı özel metot: Parametre olarak alınan ID veritabanımızda gerçekten var mı yok mu?
        private bool StudentExists(int id)
        {
          return (_context.Students?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
