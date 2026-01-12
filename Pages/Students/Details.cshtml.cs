using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;
using StudentRegistrationApp.Services;

namespace StudentRegistrationApp.Pages.Students
{
    public class DetailsModel : PageModel
    {
        private readonly StudentRegistrationApp.Data.SchoolContext _context;
        private readonly PdfService _pdfService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DetailsModel(
            StudentRegistrationApp.Data.SchoolContext context, 
            PdfService pdfService,
            IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _pdfService = pdfService;
            _hostEnvironment = hostEnvironment;
        }

    
        public Student Student { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.ID == id);

            if (student == null) return NotFound();
            
            Student = student;
            return Page();
        }

        public async Task<IActionResult> OnGetDownloadPdfAsync(int id)
{
    var student = await _context.Students.FirstOrDefaultAsync(m => m.ID == id);
    if (student == null) return NotFound();

    // 1. Get Physical Path for Passport Photo
    string photoFullPath = Path.Combine(_hostEnvironment.WebRootPath, student.PassportPhotoPath.TrimStart('/'));
    string photoUri = new Uri(photoFullPath).AbsoluteUri;

    // Physical Path for Logo (wwwroot/assets/logo.png)
    string logoFullPath = Path.Combine(_hostEnvironment.WebRootPath, "assets", "logo.jpeg");
    string logoUri = new Uri(logoFullPath).AbsoluteUri;

    // Pass both URIs to the service
    var pdfBytes = _pdfService.GenerateStudentPdf(student, photoUri, logoUri);

    return File(pdfBytes, "application/pdf", $"{student.FullName}_Certificate.pdf");
}
    }
}