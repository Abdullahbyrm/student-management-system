using AspNetCoreCrudDemo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreCrudDemo.Controllers
{
    [Authorize] // Sadece giriş yapmış olanlar girebilir
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcı Admin mi?
            if (User.IsInRole("Admin"))
            {
                // Admin için genel istatistikleri hazırlayalım
                ViewBag.TotalStudents = await _context.Students.CountAsync();
                ViewBag.TotalUsers = await _context.Users.CountAsync();
                ViewBag.PopularCourse = await _context.Students
                    .GroupBy(s => s.Course)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync() ?? "Henüz Yok";
                
                return View("AdminIndex"); // Admin'e özel görünüm
            }
            else
            {
                // Standart kullanıcı (Öğrenci) için kişisel bilgileri hazırlayalım
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                ViewBag.UserName = User.Identity?.Name ?? "Öğrenci";
                
                // Eğer bu kullanıcı öğrenci tablosunda da varsa kursunu çekelim
                var studentInfo = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
                ViewBag.CourseName = studentInfo?.Course ?? "Henüz Bir Kursa Kayıtlı Değilsiniz";

                return View("UserIndex"); // Öğrenciye özel görünüm
            }
        }
    }
}
