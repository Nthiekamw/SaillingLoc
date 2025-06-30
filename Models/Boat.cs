using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre maximal de passagers doit être au moins 1")]
        public int MaxPassengers { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le prix journalier doit être positif")]
        public decimal DailyPrice { get; set; }

        public bool SkipperRequired { get; set; }

        [Required(ErrorMessage = "Le type de bateau est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un type valide")]
        public int? BoatTypeId { get; set; }

        [Required(ErrorMessage = "Le port est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un port valide")]
        public int PortId { get; set; }

        public string? UserId { get; set; }
        public string? Photo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public BoatType? BoatType { get; set; }
        public User? User { get; set; }
        public Port? Port { get; set; }

        public ICollection<BoatPhoto> Photos { get; set; } = new List<BoatPhoto>();
        public ICollection<BoatEquipment> Equipments { get; set; } = new List<BoatEquipment>();
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
