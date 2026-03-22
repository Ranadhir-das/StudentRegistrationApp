// Program.cs
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL; 
using StudentRegistrationApp.Data; 
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using StudentRegistrationApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

// ...
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SchoolContext");

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<PdfService>();

builder.Services.AddRazorPages();

//builder.Services.AddRazorPages(options =>
//{
    // Only allow users with an "Admin" role or a specific session to see this folder
 //   options.Conventions.AuthorizeFolder("/admin"); 
//});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; 
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddSession(); 
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<StudentRegistrationApp.Services.EmailService>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
