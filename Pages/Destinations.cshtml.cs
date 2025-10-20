using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class DestinationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DestinationsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PortWithBoatCount> Ports { get; set; }

        public async Task OnGetAsync()
        {
            // Ajouter les ports s'ils n'existent pas déjà
            await EnsurePortsExist();

            // Récupérer tous les ports avec le nombre de bateaux disponibles
            var portsQuery = await _context.Ports
                .Include(p => p.Boats)
                .OrderBy(p => p.Name)
                .ToListAsync();

            Ports = portsQuery.Select(p => new PortWithBoatCount
            {
                Id = p.Id,
                Name = p.Name,
                City = p.City ?? "Ville non spécifiée",
                Country = p.Country ?? "France",
                BoatCount = p.Boats?.Count ?? 0
            }).ToList();
        }

        private async Task EnsurePortsExist()
        {
            // Liste des ports à ajouter
            var portsToAdd = new List<Port>
            {
                new Port
                {
                    Name = "Port de Nice",
                    City = "Nice",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port de Monaco",
                    City = "Monaco",
                    Country = "Monaco"
                },
                new Port
                {
                    Name = "Port de Cannes",
                    City = "Cannes",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port de Bayonne",
                    City = "Bayonne",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port de Brest",
                    City = "Brest",
                    Country = "France"
                },
                new Port
                {
                    Name = "Ancien port de Marseille",
                    City = "Marseille",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port de Lorient",
                    City = "Lorient",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port de Sète",
                    City = "Sète",
                    Country = "France"
                },
                new Port
                {
                    Name = "Port d'Arcachon",
                    City = "Arcachon",
                    Country = "France"
                }
            };

            // Vérifier et ajouter uniquement les ports qui n'existent pas
            foreach (var port in portsToAdd)
            {
                var exists = await _context.Ports
                    .AnyAsync(p => p.Name == port.Name);

                if (!exists)
                {
                    _context.Ports.Add(port);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
    // Classe helper pour afficher les informations du port
    public class PortWithBoatCount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int BoatCount { get; set; }
    }
}