using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Models;

namespace SaillingLoc.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Déclaration des tables
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
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des décimales
            modelBuilder.Entity<Boat>().Property(b => b.DailyPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.CommissionAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Reservation>().Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");

            // BoatPhoto
            modelBuilder.Entity<BoatPhoto>()
                .HasOne(bp => bp.Boat)
                .WithMany(b => b.Photos)
                .HasForeignKey(bp => bp.BoatId)
                .OnDelete(DeleteBehavior.Cascade); // Photo supprimée si bateau supprimé

            // Boat -> Port, BoatType, User
            modelBuilder.Entity<Boat>()
                .HasOne(b => b.Port)
                .WithMany(p => p.Boats)
                .HasForeignKey(b => b.PortId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boat>()
                .HasOne(b => b.BoatType)
                .WithMany(bt => bt.Boats)
                .HasForeignKey(b => b.BoatTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boat>()
                .HasOne(b => b.User)
                .WithMany(u => u.Boats)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Boat)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BoatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Reservation)
                .WithOne(r => r.Contract)
                .HasForeignKey<Contract>(c => c.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Reservation)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reservation)
                .WithOne(res => res.Review)
                .HasForeignKey<Review>(r => r.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Reservation)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            // UserDocument
            modelBuilder.Entity<UserDocument>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Availability
            modelBuilder.Entity<Availability>()
                .HasOne(a => a.Boat)
                .WithMany(b => b.Availabilities)
                .HasForeignKey(a => a.BoatId)
                .OnDelete(DeleteBehavior.Cascade);

            // BoatEquipment
            modelBuilder.Entity<BoatEquipment>()
                .HasOne(be => be.Boat)
                .WithMany(b => b.Equipments)
                .HasForeignKey(be => be.BoatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}