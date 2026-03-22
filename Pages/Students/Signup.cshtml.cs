using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;
using BCrypt.Net;

namespace StudentRegistrationApp.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly SchoolContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        // COMBINED CONSTRUCTOR: You only need one to inject all dependencies
        public CreateModel(SchoolContext context, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        [BindProperty]
        public IFormFile PassportPhotoUpload { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove RegistrationNo from validation because we generate it here
            ModelState.Remove("Student.RegistrationNo");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Generate a Unique Registration Number
            Student.RegistrationNo = await GenerateUniqueRegNo();

            // 2. Hash Password
            Student.Password = BCrypt.Net.BCrypt.HashPassword(Student.Password);

            // 3. File Upload Logic
            if (PassportPhotoUpload != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/photos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + PassportPhotoUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PassportPhotoUpload.CopyToAsync(fileStream);
                }
                Student.PassportPhotoPath = "/images/photos/" + uniqueFileName;
            }

            // Ensure Date is in UTC for database compatibility
            Student.DOB = DateTime.SpecifyKind(Student.DOB, DateTimeKind.Utc);

            // 4. Save to Database
            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            // 5. TRIGGER EMAIL: Call the method after successful save
            await SendWelcomeEmail(Student.Email, Student.FullName, Student.RegistrationNo);

            // Pass the number to the success page
            TempData["GeneratedRegNo"] = Student.RegistrationNo;

            return RedirectToPage("./RegistrationSuccess");
        }

        private async Task<string> GenerateUniqueRegNo()
        {
            Random res = new Random();
            string newRegNo = string.Empty;
            bool isUnique = false;

            while (!isUnique)
            {
                int ranNum = res.Next(1000, 9999);
                newRegNo = $"REG-{DateTime.Now.Year}-{ranNum}";

                var exists = await _context.Students.AnyAsync(s => s.RegistrationNo == newRegNo);
                if (!exists) isUnique = true;
            }
            return newRegNo;
        }

        private async Task SendWelcomeEmail(string email, string name, string regNo)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(emailSettings["SenderEmail"], "Kazi Nazrul University");
                message.To.Add(new MailAddress(email));
                message.Subject = "Registration Successful - Welcome!";

                message.Body = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #1a237e; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #1a237e;'>Welcome to the University, {name}!</h2>
                    <p>Your registration has been completed successfully.</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 5px solid #1a237e;'>
                        <p><strong>Registration Number:</strong> {regNo}</p>
                        <p><strong>Email:</strong> {email}</p>
                    </div>
                    <p style='margin-top: 20px;'>You can now log in to the student portal to download your registration certificate and admit card.</p>
                    <hr/>
                    <p style='font-size: 12px; color: #777;'>This is an automated message. Please do not reply.</p>
                </div>";
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"])))
                {
                    client.Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // We don't want to crash the whole app if email fails
                Console.WriteLine("Email Error: " + ex.Message);
            }
        }
    }
}