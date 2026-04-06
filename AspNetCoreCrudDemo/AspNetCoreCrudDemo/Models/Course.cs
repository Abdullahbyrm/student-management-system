using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreCrudDemo.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        public double Rating { get; set; } = 5.0;
    }
}
