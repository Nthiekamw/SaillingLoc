using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SaillingLoc.Models
{
    public class Boat
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du bateau est obligatoire")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La marque est obligatoire")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Le modèle est obligatoire")]
        public string Model { get; set; }

        [Range(0.1, 1000, ErrorMessage = "La longueur doit être positive et réaliste")]
        public double Length { get; set; }

        [Required(ErrorMessage = "Le type de moteur est obligatoire")]
        public string EngineType { get; set; }

        public decimal DailyPrice { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre maximal de passagers doit être au moins 1")]
        public int MaxPassengers { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix journalier doit être positif")]
        public decimal PricePerDay { get; set; }

        public bool SkipperRequired { get; set; }

        [Required(ErrorMessage = "Le type de bateau est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un type valide")]
        public int BoatTypeId { get; set; }

        // Clé étrangère pour le port
        [Required(ErrorMessage = "Le port est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un port valide")]
        public int PortId { get; set; }

  
        public string UserId { get; set; }  // UserId ici représente le propriétaire du bateau

        // Photo principale du bateau
        public string? Photo { get; set; }

        // Date de création et de mise à jour
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Références aux entités associées
        public BoatType? BoatType { get; set; }
        public User? User { get; set; }  // Propriétaire du bateau
        public Port? Port { get; set; } // Propriété de navigation vers Port
         // Ajoute ces propriétés pour la localisation GPS
        public double Latitude { get; set; }
        public double Longitude { get; set; }
          public int? Capacity { get; set; }
          
           public string? OwnerId { get; set; }    // ← doit exister !
    public User? Owner { get; set; }    

        // Collection de photos supplémentaires du bateau
        public ICollection<BoatPhoto> Photos { get; set; } = new List<BoatPhoto>();

        // Collection d'équipements disponibles sur le bateau
        public ICollection<BoatEquipment> Equipments { get; set; } = new List<BoatEquipment>();

        // Disponibilité du bateau
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

        // Réservations associées au bateau
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // Méthode pour récupérer la première photo si elle existe
        public string GetFirstPhotoUrl()
        {
            if (Photos != null && Photos.Any())
            {
                return Photos.First().PhotoUrl;  // Si la collection Photos contient des éléments, renvoyer la première photo
            }
            return Photo;  // Si aucune photo supplémentaire, renvoyer la photo principale
        }
    }
}
