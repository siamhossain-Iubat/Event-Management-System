using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring the decimal precision for the 'money' columns
            modelBuilder.Entity<Event>()
                .Property(e => e.TicketPrice)
                .HasColumnType("money");

            modelBuilder.Entity<Registration>()
                .Property(r => r.TotalPaid)
                .HasColumnType("money");

            // Optional: Fluent API to ensure the Master-Detail relationship
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Attendee)
                .WithMany(a => a.Registrations)
                .HasForeignKey(r => r.AttendeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
