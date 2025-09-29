using System;
using System.ComponentModel.DataAnnotations;

namespace Safety_Tech.Models.Models
{
    /// <summary>
    /// Model for demonstrating all HTML controls and their validations.
    /// </summary>
    public class HtmlControlsDemo
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string TextBox { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Range(1, 100)]
        public int Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [Required]
        public bool Checkbox { get; set; }

        [Required]
        public string RadioOption { get; set; }

        [Required]
        public string Dropdown { get; set; }

        [StringLength(200)]
        public string TextArea { get; set; }

        [Url]
        public string Url { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]
        public bool AcceptTerms { get; set; }
    }
} 