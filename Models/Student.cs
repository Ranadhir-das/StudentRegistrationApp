// Models/Student.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentRegistrationApp.Models
{
public class Student
{
    [Key]
    public int ID { get; set; } // Primary Key

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DOB { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Registration No")]
    public string RegistrationNo { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)] // This ensures the dots appear instead of text in the UI
    public string Password { get; set; } = string.Empty;

    // Add this inside your Student class in Models/Student.cs
    [NotMapped] // This ensures it's not saved to the database
    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Field for file upload (Passport Photo). This will store the path/filename.
    // The actual file handling (uploading/saving) will be done in the PageModel.
    [Display(Name = "Passport Photo")]
    public string PassportPhotoPath { get; set; } = string.Empty;
}
}