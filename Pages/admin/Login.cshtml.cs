using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentRegistrationApp.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = default!;

        public void OnGet()
        {
            // Clear session if they navigate here to ensure a fresh start
            HttpContext.Session.Remove("UserRole");
        }

        public IActionResult OnPost()
        {
            // REPLACE these with your desired Admin credentials
            if (Email == "admin@knu.ac.in" && Password == "Admin@123")
            {
                // SET SESSION: This is the key that unlocks the Admin folder
                HttpContext.Session.SetString("UserRole", "Admin");
                
                return RedirectToPage("/Admin/Index");
            }

            // If login fails
            ModelState.AddModelError(string.Empty, "Invalid Admin email or password.");
            return Page();
        }

        // Logout Handler
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Admin/Login");
        }
    }
}