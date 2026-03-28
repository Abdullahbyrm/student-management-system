using System.ComponentModel.DataAnnotations;

namespace AspNetCoreCrudDemo.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Öğrenci adı zorunludur.")]
        [Display(Name = "Öğrenci Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-Posta zorunludur.")]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kurs alanı zorunludur.")]
        [Display(Name = "Kayıtlı Olduğu Kurs")]
        public string Course { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Kayıt Tarihi")]
        public DateTime EnrollmentDate { get; set; }
    }
}
