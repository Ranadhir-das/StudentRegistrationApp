using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Services;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly StudentRegistrationApp.Data.SchoolContext _context;

        private readonly EmailService _emailService;

        public IndexModel(StudentRegistrationApp.Data.SchoolContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Property to hold the list of students displayed on the page
        public IList<Student> Student { get;set; } = new List<Student>();

        // Property to capture the search term from the form (SupportsGet = true allows it to be read from the URL)
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Only proceed to query and filter if a search term is present.
            if (!string.IsNullOrEmpty(SearchString))
            {
                var students = from s in _context.Students
                               select s;


                students = students.Where(s => s.FullName.ToLower().Contains(SearchString.ToLower())
                                           || s.RegistrationNo.ToLower().Contains(SearchString.ToLower()));

                // Assign the filtered results to the Student list
                Student = await students.ToListAsync();
            }
            // If no search string is provided, Student remains an empty list, 
            // which prevents the table from rendering on initial load.
        }
        public async Task<IActionResult> OnGetSendResetOtpAsync()
        {
            // 1. Get user email from Session (Ensure this key matches your Login page)
            string? userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Login");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
            if (student == null) return RedirectToPage("/Login");

            // 2. Generate and Save OTP
            string otp = new Random().Next(100000, 999999).ToString();
            student.ResetOtp = otp;
            student.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // 3. Send Professional Email
            string subject = "Reset Password Verification";
            string body = $@"
                            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #007bff; padding: 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 24px;'>Security Verification</h1>
                    </div>
                    <div style='padding: 30px; line-height: 1.6; color: #333;'>
                        <p>Hello <strong>{student.FullName}</strong>,</p>
                        <p>We received a request to reset the password for your student account. Please use the verification code below to complete the process:</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <span style='font-size: 30px; font-weight: bold; letter-spacing: 8px; color: #007bff; background: #f8f9fa; padding: 15px 30px; border: 1px dashed #007bff; border-radius: 4px;'>
                                {otp}
                            </span>
                        </div>
                
                        <p style='font-size: 14px; color: #666;'>This code is valid for <strong>10 minutes</strong>. For security, do not share this code with anyone.</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #999;'>If you did not request this change, please ignore this email or contact support if you have concerns about your account security.</p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; color: #777;'>
                        &copy; {DateTime.Now.Year} Student Registration System | All Rights Reserved
                    </div>
                </div>";
            
            await _emailService.SendEmailAsync(student.Email, subject, body);

            // 4. Redirect to the existing ResetPassword page
            TempData["StatusMessage"] = "Verification code has been sent to your email.";
            return RedirectToPage("/Students/ResetPassword", new { email = student.Email });
        }
    }
}