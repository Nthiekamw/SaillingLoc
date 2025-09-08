
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace SailingLoc.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Subject { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Veuillez remplir tous les champs correctement.";
                return Page();
            }

            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(Name, Email));
                emailMessage.To.Add(new MailboxAddress("Admin", "williamnthiekam392@gmail.com"));
                emailMessage.Subject = Subject;
                emailMessage.Body = new TextPart("html")
                {
                    Text = $"<p><strong>Nom :</strong> {Name}</p>" +
                           $"<p><strong>Email :</strong> {Email}</p>" +
                           $"<p><strong>Téléphone :</strong> {Phone}</p>" +
                           $"<p><strong>Message :</strong><br>{Message}</p>"
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("williamnthiekam392@gmail.com", "jdju bioh ffqd nbgo"); 
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                SuccessMessage = "Votre message a été envoyé avec succès !";
                ModelState.Clear();
            }
            catch (Exception ex)
{
    ErrorMessage = $"Erreur lors de l'envoi : {ex.Message}";
}

            // catch
            // {
            //     ErrorMessage = "Erreur lors de l'envoi du message. Veuillez réessayer plus tard.";
            // }

            return Page();
        }
    }
}









































// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace SailingLoc.Pages
// {
//     public class ContactModel : PageModel
//     {
//         [BindProperty]
//         public string Name { get; set; }

//         [BindProperty]
//         public string Email { get; set; }

//         [BindProperty]
//         public string Phone { get; set; }

//         [BindProperty]
//         public string Subject { get; set; }

//         [BindProperty]
//         public string Message { get; set; }

//         public void OnGet()
//         {
//         }

//         public IActionResult OnPost()
//         {
//             if (!ModelState.IsValid)
//             {
//                 return Page();
//             }

//             // Ici, vous pouvez ajouter la logique pour traiter le formulaire (par exemple, envoyer un email)

//             return RedirectToPage("ThankYou"); // Rediriger après soumission
//         }
//     }
// }