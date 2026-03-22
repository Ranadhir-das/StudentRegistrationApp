using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public IList<Student> StudentList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // --- SECURITY CHECK ---
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                return RedirectToPage("/Admin/Login");
            }

            // --- SEARCH LOGIC ---
            var students = from s in _context.Students
                           select s;

            if (!string.IsNullOrEmpty(SearchString))
            {
                // Filters database results by Name or Registration Number
                students = students.Where(s => s.FullName.Contains(SearchString) 
                                       || s.RegistrationNo.Contains(SearchString));
            }

            StudentList = await students.AsNoTracking().ToListAsync();
            return Page();
        }

        // Logout functionality
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Admin/Login");
        }
    }
}