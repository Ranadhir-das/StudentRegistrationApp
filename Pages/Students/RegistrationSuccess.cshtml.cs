using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Pages.Students
{
    public class RegistrationSuccessModel : PageModel
    {
        private readonly SchoolContext _context;

        public RegistrationSuccessModel(SchoolContext context)
        {
            _context = context;
        }

        // This holds the student data to display on the slip
        public Student? RegisteredStudent { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Retrieve the RegNo from TempData
            string? regNo = TempData["GeneratedRegNo"]?.ToString();

            if (string.IsNullOrEmpty(regNo))
            {
                // If someone tries to access this page directly without registering, send them away
                return RedirectToPage("/Index");
            }

            // 2. Fetch the student from DB so we can show their Name and Photo
            RegisteredStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.RegistrationNo == regNo);

            // 3. CRITICAL: Keep TempData alive so it doesn't disappear on refresh
            TempData.Keep("GeneratedRegNo");

            return Page();
        }
    }
}