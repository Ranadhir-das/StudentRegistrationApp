using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Services;

namespace StudentRegistrationApp.Pages.Students
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly SchoolContext _context;
        private readonly EmailService _emailService;

        public ForgotPasswordModel(SchoolContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
            {
                ModelState.AddModelError("", "Email not found in our records.");
                return Page();
            }

            // Generate 6-digit OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // Save to DB with 10-minute expiry
            student.ResetOtp = otp;
            student.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // Send Email
            //string subject = "Your Password Reset OTP";
           // string body = $"<h3>Hello {student.FullName},</h3><p>Your OTP for password reset is: <b>{otp}</b>. It is valid for 10 minutes.</p>";

            string subject = "Reset Your Account Password";
            string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 10px; padding: 20px;'>
                    <div style='text-align: center; border-bottom: 2px solid #007bff; padding-bottom: 10px;'>
                        <h2 style='color: #007bff;'>Student Registration App</h2>
                    </div>
                    <div style='padding: 20px;'>
                        <p>Hello <strong>{student.FullName}</strong>,</p>
                        <p>We received a request to reset your password. Use the verification code below to proceed:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #333; background: #f4f4f4; padding: 10px 20px; border-radius: 5px; border: 1px dashed #007bff;'>
                                {otp}
                            </span>
                        </div>
                        <p style='color: #555; font-size: 14px;'>This code is valid for <strong>10 minutes</strong>. If you did not request this, please ignore this email.</p>
                    </div>
                    <div style='text-align: center; border-top: 1px solid #eee; padding-top: 20px; color: #888; font-size: 12px;'>
                        <p>&copy; {DateTime.Now.Year} Your College Name. All rights reserved.</p>
                    </div>
                </div>";
            
            await _emailService.SendEmailAsync(email, subject, body);

            // Pass email to next page so we know whose password to change
            return RedirectToPage("./ResetPassword", new { email = email });
        }
    }
}