using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;
using BCrypt.Net;

namespace StudentRegistrationApp.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly SchoolContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Add this line

        public CreateModel(SchoolContext context, IWebHostEnvironment hostEnvironment) // Add hostEnvironment parameter
          {
              _context = context;
              _hostEnvironment = hostEnvironment; // Assign it
          }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        [BindProperty]
        public IFormFile PassportPhotoUpload { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
{
    // Check if the model is valid (data annotations like [Required] on Student properties)
    if (!ModelState.IsValid)
    {
        return Page();
    }

     Student.Password = BCrypt.Net.BCrypt.HashPassword(Student.Password);

    // --- File Upload Logic ---
    if (PassportPhotoUpload != null)
    {
        // Define the folder path (wwwroot/images/photos)
        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/photos");

        // Ensure the directory exists
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        // Create a unique filename (recommended practice)
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + PassportPhotoUpload.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save the file to the physical disk location
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await PassportPhotoUpload.CopyToAsync(fileStream);
        }

        // Save the relative path to the Student object for the database
        Student.PassportPhotoPath = "/images/photos/" + uniqueFileName;
    }


    // -------------------------
    Student.DOB = DateTime.SpecifyKind(Student.DOB, DateTimeKind.Utc);
    // Save the Student record to the PostgreSQL database
    _context.Students.Add(Student);
    await _context.SaveChangesAsync();

    // Redirect to the list view after successful registration
    return RedirectToPage("./RegistrationSuccess");
}
    }
}
