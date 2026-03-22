using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies; 
using StudentRegistrationApp.Services;


namespace StudentRegistrationApp.Pages.Students
{
    public class ResetPasswordModel : PageModel
    {
        private readonly SchoolContext _context;

        private readonly EmailService _emailService;

        public ResetPasswordModel(SchoolContext context, EmailService emailService) 
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string OTP { get; set; } = string.Empty;
        [BindProperty] public string NewPassword { get; set; } = string.Empty;

        public void OnGet(string email)
        {
            // This fills the hidden Email field in your form automatically
            Email = email; 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == Email);
        
            if (student == null)
            {
                ModelState.AddModelError("", "Session expired. Please try again.");
                return Page();
            }
        
            // 1. Verify OTP
            if (student.ResetOtp != OTP || student.OtpExpiry < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Invalid or expired OTP.");
                return Page();
            }
        
            // 2. Update Password in Database
            student.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            student.ResetOtp = null;
            student.OtpExpiry = null;
            
            await _context.SaveChangesAsync(); // SAVE FIRST
        
            // 3. Send the Confirmation Email (Professional Style)
            string confirmSubject = "Security Alert: Password Changed";
            string confirmBody = $"<h3>Hello {student.FullName},</h3><p>Your password was successfully changed on {DateTime.Now}. If this wasn't you, contact support.</p>";
            
            try {
                await _emailService.SendEmailAsync(student.Email, confirmSubject, confirmBody);
            } catch {
                // We catch this so a Gmail error doesn't stop the password from being changed
            }
        
            // 4. LOGOUT THE USER
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
        
            // 5. Redirect to Login
            TempData["SuccessMessage"] = "Password updated! A confirmation email has been sent. Please login again.";
            return RedirectToPage("/Students/Login");
        }

        public async Task<IActionResult> OnGetSendOtpAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null) return RedirectToPage("/Students/Login");
        
            // Generate New OTP
            string otp = new Random().Next(100000, 999999).ToString();
            student.ResetOtp = otp;
            student.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();
        
            // Send Professional Email (Use your professional body string here)
            await _emailService.SendEmailAsync(student.Email, "New Verification Code", $"Your new OTP is: {otp}");
        
            TempData["StatusMessage"] = "A new code has been sent to your email.";
            return RedirectToPage(new { email = email });
        }


    }
}