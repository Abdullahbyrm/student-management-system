using System.ComponentModel.DataAnnotations;

namespace AspNetCoreCrudDemo.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Şifrenin "karıştırılmış" hali burada duracak.

        public string Role { get; set; } = "User"; // Başlangıçta herkes standart kullanıcı.

        public string? RefreshToken { get; set; } // Yedek bilet (Ana bilet eskiyince sunucuya bunu gösteriyoruz)
        public DateTime? RefreshTokenExpiry { get; set; } // Yedek biletin son kullanma tarihi (30 GÜN)
    }
}
