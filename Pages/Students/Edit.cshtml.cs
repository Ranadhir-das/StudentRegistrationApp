using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly StudentRegistrationApp.Data.SchoolContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EditModel(StudentRegistrationApp.Data.SchoolContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        [BindProperty]
        public IFormFile? Upload { get; set; } // Property to catch the new file

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.ID == id);
            if (student == null) return NotFound();
            
            Student = student;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1. Check if a new file was uploaded
            if (Upload != null)
            {
                // Delete old photo file if it exists
                if (!string.IsNullOrEmpty(Student.PassportPhotoPath))
                {
                    var oldPath = Path.Combine(_hostEnvironment.WebRootPath, Student.PassportPhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Save new photo
                string folder = Path.Combine(_hostEnvironment.WebRootPath, "images/photos");
                string fileName = Guid.NewGuid().ToString() + "_" + Upload.FileName;
                string filePath = Path.Combine(folder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }

                // Update the path in the database object
                Student.PassportPhotoPath = "/images/photos/" + fileName;
            }

            _context.Attach(Student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(Student.ID)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }

        private bool StudentExists(int id) => _context.Students.Any(e => e.ID == id);
    }
}