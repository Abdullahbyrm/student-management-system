using AspNetCoreCrudDemo.Models;
using AspNetCoreCrudDemo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreCrudDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _repository;

        public HomeController(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _repository.GetAllAsync();
            var studentList = students.ToList();

            // Dashboard istatistikleri
            ViewBag.TotalStudents = studentList.Count;
            ViewBag.TotalCourses = studentList.Select(s => s.Course).Distinct().Count();
            ViewBag.ThisMonthCount = studentList.Count(s => s.EnrollmentDate.Month == DateTime.Now.Month && s.EnrollmentDate.Year == DateTime.Now.Year);

            // Son 5 öğrenci (en son eklenen en üstte)
            ViewBag.RecentStudents = studentList.OrderByDescending(s => s.EnrollmentDate).Take(5).ToList();

            return View();
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
