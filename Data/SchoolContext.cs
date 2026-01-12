// Data/SchoolContext.cs
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;

// Use your project's root namespace followed by the Data folder name
namespace StudentRegistrationApp.Data 
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}