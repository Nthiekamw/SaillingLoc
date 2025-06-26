using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SaillingLoc.Models;

namespace SaillingLoc.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets (enlever Users si User hérite de IdentityUser)
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<BoatType> BoatTypes { get; set; }
        public DbSet<Boat> Boats { get; set; }
        public DbSet<BoatPhoto> BoatPhotos { get; set; }
        public DbSet<BoatEquipment> BoatEquipments { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Obligatoire pour Identity !

            // Relations personnalisées
            modelBuilder.Entity<User>()
                .HasMany(u => u.Documents)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Boats)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boat>()
                .HasMany(b => b.Photos)
                .WithOne(p => p.Boat)
                .HasForeignKey(p => p.BoatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Boat>()
                .HasMany(b => b.Equipments)
                .WithOne(e => e.Boat)
                .HasForeignKey(e => e.BoatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Boat>()
                .HasMany(b => b.Availabilities)
                .WithOne(a => a.Boat)
                .HasForeignKey(a => a.BoatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Boat>()
                .HasOne(b => b.BoatType)
                .WithMany(bt => bt.Boats)
                .HasForeignKey(b => b.BoatTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boat>()
                .HasOne(b => b.Port)
                .WithMany(p => p.Boats)
                .HasForeignKey(b => b.PortId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Boat)
                .WithMany()
                .HasForeignKey(r => r.BoatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Review)
                .WithOne(rv => rv.Reservation)
                .HasForeignKey<Review>(rv => rv.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Contract)
                .WithOne(c => c.Reservation)
                .HasForeignKey<Contract>(c => c.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Reservation)
                .HasForeignKey<Payment>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Reservation)
                .WithMany()
                .HasForeignKey(m => m.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
