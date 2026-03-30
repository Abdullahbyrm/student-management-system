using System.ComponentModel.DataAnnotations;

namespace AspNetCoreCrudDemo.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Lütfen öğrenci adını giriniz.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Öğrenci adı en az 2, en fazla 50 karakter olmalıdır.")]
        [Display(Name = "Öğrenci Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        [EmailAddress(ErrorMessage = "Lütfen e-posta formatına uygun (örn: ali@örnek.com) bir adres giriniz.")]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen listeden geçerli bir kurs seçiniz.")]
        [Display(Name = "Kayıtlı Olduğu Kurs")]
        public string Course { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen kayıt tarihini belirtiniz.")]
        [DataType(DataType.Date, ErrorMessage = "Lütfen geçerli bir tarih formatı seçiniz.")]
        [Display(Name = "Kayıt Tarihi")]
        public DateTime EnrollmentDate { get; set; }

        // Fotoğraf dosya yolu (nullable — fotoğraf zorunlu değil)
        [Display(Name = "Fotoğraf")]
        public string? PhotoPath { get; set; }
    }
}
