// Program.cs
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL; 
using StudentRegistrationApp.Data; // Adjust namespace if necessary
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

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Where to go if not logged in
        options.AccessDeniedPath = "/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
