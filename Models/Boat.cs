using System;
using System.Collections.Generic;

namespace SaillingLoc.Models
{
    public class Boat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double Length { get; set; }
        public string EngineType { get; set; }
        public string Description { get; set; }
        public int MaxPassengers { get; set; }
        public decimal DailyPrice { get; set; }
        public bool SkipperRequired { get; set; }
        public int BoatTypeId { get; set; }
        public int UserId { get; set; }
        public int PortId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BoatType BoatType { get; set; }
        public User User { get; set; }
        public Port Port { get; set; }
        public ICollection<BoatPhoto> Photos { get; set; }
        public ICollection<BoatEquipment> Equipments { get; set; }
        public ICollection<Availability> Availabilities { get; set; }
    }
}
