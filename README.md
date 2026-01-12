# Student Registration & Admit Card Portal

A comprehensive web-based Student Management System built with **ASP.NET Core Razor Pages**. This portal allows students to register, upload passport photos, and generate professional PDF Admit Cards/Registration Certificates.

## 🚀 Features

* **Secure Authentication**: Student login system using **BCrypt.Net** for password hashing and **Cookie Authentication** for session management.
* **Profile Management**: Students can register, edit their details, and update their passport photos.
* **PDF Generation**: Generates modern, high-quality Registration Certificates using **DinkToPdf**.
* **Responsive Design**: A custom-styled UI featuring a blue university-themed navbar, a professional multi-column footer, and a homepage image carousel.
* **Secure File Handling**: Automatic photo renaming (using GUIDs) and old file cleanup on the server.

## 🛠️ Tech Stack

* **Backend**: .NET 8 / ASP.NET Core Razor Pages
* **Database**: PostgreSQL
* **ORM**: Entity Framework Core
* **Frontend**: Bootstrap 5, Custom CSS, JavaScript
* **Security**: BCrypt.Net-Next, Cookie-based Auth
* **PDF Library**: DinkToPdf (wrapper for wkhtmltopdf)

## 📸 Screenshots

| Homepage Carousel | Student Profile | PDF Certificate |
| :--- | :--- | :--- |
| ![Home](https://via.placeholder.com/300x150?text=KNU+Home) | ![Profile](https://via.placeholder.com/300x150?text=Student+Profile) | ![PDF](https://via.placeholder.com/300x150?text=Admit+Card) |

*(Note: Replace these placeholders with actual screenshots from your app later!)*

## ⚙️ Installation & Setup

1. **Clone the repository**:
   ```bash
   git clone git@github.com:Ranadhir-das/StudentRegistrationApp.git
   cd StudentRegistrationApp