using CloudCorp.Models;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Diagnostics;


namespace CloudCorp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Index(string? swalMessage = null)
        {ViewData["SwalMessage"] = TempData["SwalMessage"] as string;
            var model = new ContactModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitContactForm(ContactModel model)
        {
            

            if (ModelState.IsValid)
            {
                string? swalMessage = string.Empty;

                try
                {
                    // Get the email settings from User Secrets
                    var emailSettings = _configuration.GetSection("EmailSettings");
                    var smtpServer = emailSettings["EmailHost"];
                    var smtpPort = Convert.ToInt32(emailSettings["EmailPort"]);
                    var username = emailSettings["EmailAddress"];
                    var password = emailSettings["EmailPassword"];

                    // Create the email message
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Your Name", "your-email@example.com"));
                    message.To.Add(new MailboxAddress("Recipient Name", "jvtdevtest17@gmail.com"));
                    message.Subject = "New Contact Form Submission";

                    // Build the email body
                    var builder = new BodyBuilder();
                    builder.TextBody = $"Name: {model.SenderName}{Environment.NewLine}Email: {model.SenderEmail}{Environment.NewLine}Message: {model.Message}";
                    message.Body = builder.ToMessageBody();

                    // Configure the email client
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(username, password);

                        // Send the email
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                    TempData["SwalMessage"] = "Success: We have received your message!";
                    // Redirect to a confirmation page
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["SwalMessage"] = "Error: Looks like something went wrong!";

                    return RedirectToAction("Index");
                    // Handle any exceptions that occurred during email sending
                    //ModelState.AddModelError("", "An error occurred while sending the email. Please try again later.");
                    // Log the exception if needed
                    throw;
                }
            }
            return View("Index", model);
            // Show a success message using JavaScript alert
            //string successMessage = "Form submitted successfully!";
            //return Content($"<script>Swal.fire('Success', '{successMessage}', 'success').then(() => window.location.href = '/Home/Index');</script>");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}