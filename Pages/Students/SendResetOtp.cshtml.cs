using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Services;
using System.Security.Claims;

namespace StudentRegistrationApp.Pages.Students
{
    // This page doesn't need a .cshtml file because it redirects immediately
    [IgnoreAntiforgeryToken] 
    public class SendResetOtpModel : PageModel
    {
        private readonly SchoolContext _context;
        private readonly EmailService _emailService;

        public SendResetOtpModel(SchoolContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Get the logged-in user's email from the Session/Auth Cookie
            // Replace "UserEmail" with the key you used during Login
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Students/Login");
            }

            // 2. Find student in PostgreSQL
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
            if (student == null)
            {
                return RedirectToPage("/Students/Login");
            }

            // 3. Generate 6-digit OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // 4. Save to DB with Expiry
            student.ResetOtp = otp;
            student.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // 5. Send the Professional Email
            string subject = "Security Verification Code";
            string body = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #007bff;'>Password Reset Request</h2>
                    <p>Hello {student.FullName},</p>
                    <p>You requested to reset your password. Please use the following code:</p>
                    <h1 style='background: #f8f9fa; padding: 10px; text-align: center; letter-spacing: 5px;'>{otp}</h1>
                    <p>This code expires in 10 minutes.</p>
                </div>";

            await _emailService.SendEmailAsync(student.Email, subject, body);

            TempData["StatusMessage"] = $"A verification code has been sent to {student.Email}. Please check your inbox.";

            return RedirectToPage("/Students/ResetPassword", new { email = student.Email });
        }
    }
}