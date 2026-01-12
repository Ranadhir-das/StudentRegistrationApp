using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly StudentRegistrationApp.Data.SchoolContext _context;

        public IndexModel(StudentRegistrationApp.Data.SchoolContext context)
        {
            _context = context;
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

                // Filter by FullName or RegistrationNo (case-insensitive)
                // Use ToLower() for case-insensitive comparison (necessary for PostgreSQL)
                students = students.Where(s => s.FullName.ToLower().Contains(SearchString.ToLower())
                                           || s.RegistrationNo.ToLower().Contains(SearchString.ToLower()));

                // Assign the filtered results to the Student list
                Student = await students.ToListAsync();
            }
            // If no search string is provided, Student remains an empty list, 
            // which prevents the table from rendering on initial load.
        }
    }
}