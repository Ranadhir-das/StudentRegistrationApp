using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;
        public PdfService(IConverter converter) => _converter = converter;

        public byte[] GenerateStudentPdf(Student student, string photoPath, string logoPath)
        {
            var html = $@"
            <html>
            <head>
                <style>
                    @page {{ size: A4; margin: 0; }}
                    body {{ font-family: 'Helvetica', 'Arial', sans-serif; color: #333; line-height: 1.5; padding: 40px 15px; background: #fff; }}
                    
                    /* Main Border Container */
                    .certificate-container {{ 
                        border: 10px double #1a237e; 
                        padding: 30px; 
                        position: relative;
                        min-height: 600px;
                    }}
            
                    /* Header Styling */
                    .header {{ text-align: center; border-bottom: 2px solid #1a237e; padding-bottom: 20px; margin-bottom: 30px; }}
                    .logo {{ height: 80px; width: auto; margin-bottom: 10px; }}
                    .university-name {{ font-size: 28px; font-weight: bold; color: #1a237e; margin: 0; text-transform: uppercase; }}
                    .university-address {{ font-size: 14px; margin: 5px 0; color: #555; }}
                    .doc-title {{ font-size: 22px; margin-top: 15px; font-weight: bold; text-decoration: underline; color: #d32f2f; }}
            
                    /* Two Column Layout (Floating for PDF compatibility) */
                    .content-row {{ width: 100%; margin-top: 40px; clear: both; overflow: hidden; }}
                    
                    .details-column {{ float: left; width: 65%; }}
                    .photo-column {{ float: right; width: 25%; text-align: right; }}
            
                    /* Table Styling */
                    table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
                    th {{ text-align: left; font-size: 14px; color: #777; text-transform: uppercase; padding: 12px 5px; width: 40%; }}
                    td {{ font-size: 16px; font-weight: 600; padding: 12px 5px; border-bottom: 1px solid #eee; }}
            
                    /* Photo Box */
                    .photo {{ 
                        width: 150px; 
                        height: 180px; 
                        border: 2px solid #1a237e; 
                        padding: 2px;
                        object-fit: cover;
                        box-shadow: 2px 2px 5px rgba(0,0,0,0.1);
                    }}
            
                    /* Footer Stamp Area */
                    .footer-sig {{ margin-top: 80px; width: 100%; }}
                    .sig-box {{ float: right; width: 200px; text-align: center; border-top: 1px solid #333; padding-top: 5px; font-weight: bold; font-size: 14px; }}
                </style>
            </head>
            <body>
                <div class='certificate-container'>
                    <div class='header'>
                        <img src='{logoPath}' class='logo'/>
                        <p class='university-name'>Kazi Nazrul University</p>
                        <p class='university-address'>Asansol, Paschim Bardhaman, West Bengal - 713302</p>
                        <div class='doc-title'>REGISTRATION CERTIFICATE</div>
                    </div>
            
                    <div class='content-row'>
                        <div class='details-column'>
                            <table>
                                <tr><th>Full Name</th><td>{student.FullName}</td></tr>
                                <tr><th>Registration No</th><td>{student.RegistrationNo}</td></tr>
                                <tr><th>Date of Birth</th><td>{student.DOB.ToShortDateString()}</td></tr>
                                <tr><th>Phone Number</th><td>{student.PhoneNumber}</td></tr>
                                <tr><th>Date of Issue</th><td>{DateTime.Now.ToShortDateString()}</td></tr>
                            </table>
                        </div>
                        
                        <div class='photo-column'>
                            <img src='{photoPath}' class='photo' />
                            <p style='font-size: 10px; margin-top: 5px; text-align: center; color: #888;'>Passport Photo</p>
                        </div>
                    </div>
            
                    <div class='footer-sig'>
                        <div class='sig-box'>
                            Registrar Signature
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Portrait, PaperSize = PaperKind.A4 },
                Objects = { new ObjectSettings() { HtmlContent = html } }
            };

            return _converter.Convert(doc);
        }
    }
}