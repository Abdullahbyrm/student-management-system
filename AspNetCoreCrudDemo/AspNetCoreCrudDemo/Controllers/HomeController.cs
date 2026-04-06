using AspNetCoreCrudDemo.Models;
using AspNetCoreCrudDemo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreCrudDemo.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreCrudDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _repository;
        private readonly ApplicationDbContext _context;

        public HomeController(IStudentRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Eğer kullanıcı zaten giriş yapmışsa, onu Dashboard'a yönlendirelim.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Veritabanındaki kursları ana sayfaya gönderelim
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Home/SearchCourses?q=react (AJAX API)
        [HttpGet]
        public async Task<IActionResult> SearchCourses(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return Json(await _context.Courses.ToListAsync());
            }

            var results = await _context.Courses
                .Where(c => c.Title.Contains(q) || c.Category.Contains(q))
                .ToListAsync();

            return Json(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
