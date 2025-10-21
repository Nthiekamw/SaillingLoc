using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SailingLoc.Pages
{
    public class CategoriesModel : PageModel
    {
        public List<BoatCategory> Categories { get; set; }
        public List<CarouselBoat> CarouselBoats { get; set; }

        public void OnGet()
        {
            // Initialisation des catégories principales
            Categories = new List<BoatCategory>
            {
                new BoatCategory
                {
                    Id = 1,
                    Name = "Catamaran",
                    ImageUrl = "/images/catamaran.webp",
                    Description = "Stabilité et espace pour une navigation confortable"
                },
                new BoatCategory
                {
                    Id = 2,
                    Name = "Yacht",
                    ImageUrl = "/images/yatch.webp",
                    Description = "Luxe et élégance pour une croisière inoubliable"
                },
                new BoatCategory
                {
                    Id = 3,
                    Name = "Bateau traditionnel",
                    ImageUrl = "/images/pic-1-1.jpeg",
                    Description = "Authenticité et charme d'antan"
                },
                new BoatCategory
                {
                    Id = 4,
                    Name = "Voilier",
                    ImageUrl = "/images/bateaux%20moteur.jpg",
                    Description = "Navigation à la voile pour les amateurs"
                },
                new BoatCategory
                {
                    Id = 5,
                    Name = "Bateau à moteur",
                    ImageUrl = "/images/catamaran.webp",
                    Description = "Puissance et vitesse pour des sensations fortes"
                }
            };

            // Initialisation des bateaux du carrousel
            CarouselBoats = new List<CarouselBoat>
            {
                new CarouselBoat
                {
                    Id = 1,
                    Name = "Pic",
                    ImageUrl = "/images/pic-1-1.jpeg",
                    Description = "Bateau traditionnel en bois pour une expérience authentique"
                },
                new CarouselBoat
                {
                    Id = 2,
                    Name = "Moteur",
                    ImageUrl = "/images/bateaux%20moteur.jpg",
                    Description = "Puissance et vitesse pour des sensations fortes"
                },
                new CarouselBoat
                {
                    Id = 3,
                    Name = "Catamaran",
                    ImageUrl = "/images/catamaran.webp",
                    Description = "Stabilité et espace pour une navigation confortable"
                },
                new CarouselBoat
                {
                    Id = 4,
                    Name = "Yacht",
                    ImageUrl = "/images/yatch.webp",
                    Description = "Luxe et élégance pour une croisière inoubliable"
                },
                new CarouselBoat
                {
                    Id = 5,
                    Name = "Yacht de luxe",
                    ImageUrl = "/images/yatch.webp",
                    Description = "Le summum du confort et de l'élégance"
                },
                new CarouselBoat
                {
                    Id = 6,
                    Name = "Catamaran Premium",
                    ImageUrl = "/images/catamaran.webp",
                    Description = "Espace et stabilité exceptionnels"
                },
                new CarouselBoat
                {
                    Id = 7,
                    Name = "Bateau rapide",
                    ImageUrl = "/images/bateaux%20moteur.jpg",
                    Description = "Vitesse et performance maximales"
                },
                new CarouselBoat
                {
                    Id = 8,
                    Name = "Voilier traditionnel",
                    ImageUrl = "/images/pic-1-1.jpeg",
                    Description = "Navigation authentique et écologique"
                }
            };
        }

        // Classes pour les modèles de données
        public class BoatCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
        }

        public class CarouselBoat
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
        }
    }
}