using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace StudentRegistrationApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SchoolContext _context;

        public LoginModel(SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string RegistrationNo { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
{
    var student = await _context.Students
        .FirstOrDefaultAsync(s => s.RegistrationNo == RegistrationNo);

    if (student != null && BCrypt.Net.BCrypt.Verify(Password, student.Password))
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, student.FullName),
            new Claim("RegistrationNo", student.RegistrationNo),
            new Claim("StudentId", student.ID.ToString()),
            new Claim("ProfilePath", student.PassportPhotoPath ?? "/images/default-profile.png")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return RedirectToPage("/Index"); // Successful Login
    }

    ErrorMessage = "Invalid Registration Number or Password.";
    return Page(); // Failed Login
}
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}
