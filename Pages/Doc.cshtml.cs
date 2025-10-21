using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SailingLoc.Pages
{
    public class DocModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public DocModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        // Propriétés pour les documents
        public Document IdentityCard { get; set; }
        public Document BoatLicense { get; set; }
        public Document Insurance { get; set; }

        public void OnGet()
        {
            // Initialiser les documents avec des données de test
            // Dans une vraie application, récupérer depuis la base de données
            IdentityCard = new Document
            {
                Id = 1,
                Name = "Carte d'identité",
                Description = "Une copie recto-verso de votre pièce d'identité valide",
                Status = DocumentStatus.Validated,
                UploadDate = new DateTime(2023, 5, 15),
                FilePath = "/documents/identity_card.pdf"
            };

            BoatLicense = new Document
            {
                Id = 2,
                Name = "Permis bateau",
                Description = "Votre permis bateau valide (si nécessaire pour la location)",
                Status = DocumentStatus.Rejected,
                UploadDate = new DateTime(2023, 6, 20),
                FilePath = "/documents/boat_license.pdf",
                RejectionReason = "La photo n'est pas lisible"
            };

            Insurance = new Document
            {
                Id = 3,
                Name = "Assurance",
                Description = "Votre attestation d'assurance couvrant la location",
                Status = DocumentStatus.None,
                UploadDate = null,
                FilePath = null
            };

            // Afficher le message d'erreur pour le permis bateau rejeté
            if (BoatLicense.Status == DocumentStatus.Rejected)
            {
                ErrorMessage = BoatLicense.RejectionReason;
            }
        }

        public async Task<IActionResult> OnPostUploadAsync(int documentId)
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                ErrorMessage = "Veuillez sélectionner un fichier à télécharger.";
                return Page();
            }

            // Vérifier le type de fichier
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(UploadedFile.FileName).ToLowerInvariant();
            
            if (!Array.Exists(allowedExtensions, ext => ext == extension))
            {
                ErrorMessage = "Format de fichier non autorisé. Veuillez télécharger un PDF ou une image.";
                return Page();
            }

            // Vérifier la taille du fichier (max 5 MB)
            if (UploadedFile.Length > 5 * 1024 * 1024)
            {
                ErrorMessage = "Le fichier est trop volumineux. Taille maximale : 5 MB.";
                return Page();
            }

            try
            {
                // Créer le dossier de destination s'il n'existe pas
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");
                Directory.CreateDirectory(uploadsFolder);

                // Générer un nom de fichier unique
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Sauvegarder le fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // TODO: Sauvegarder les informations du document dans la base de données
                // Exemple:
                // var document = new Document
                // {
                //     UserId = GetCurrentUserId(),
                //     DocumentType = documentId,
                //     FilePath = $"/uploads/documents/{fileName}",
                //     UploadDate = DateTime.Now,
                //     Status = DocumentStatus.Pending
                // };
                // _context.Documents.Add(document);
                // await _context.SaveChangesAsync();

                SuccessMessage = "Document téléchargé avec succès. Il sera vérifié sous peu.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du téléchargement : {ex.Message}";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int documentId)
        {
            try
            {
                // TODO: Supprimer le document de la base de données et du système de fichiers
                // Exemple:
                // var document = _context.Documents.Find(documentId);
                // if (document != null)
                // {
                //     // Supprimer le fichier physique
                //     var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                //     if (System.IO.File.Exists(filePath))
                //     {
                //         System.IO.File.Delete(filePath);
                //     }
                //     
                //     // Supprimer de la base de données
                //     _context.Documents.Remove(document);
                //     _context.SaveChanges();
                // }

                SuccessMessage = "Document supprimé avec succès.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression : {ex.Message}";
            }

            return RedirectToPage();
        }
    }

    // Modèle de document
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime? UploadDate { get; set; }
        public string FilePath { get; set; }
        public string RejectionReason { get; set; }
    }

    // Énumération pour les statuts de document
    public enum DocumentStatus
    {
        None,           // Aucun document téléchargé
        Pending,        // En attente de vérification
        Validated,      // Validé
        Rejected        // Rejeté
    }
}