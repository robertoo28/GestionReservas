using GestionReservas.model;
using Microsoft.EntityFrameworkCore;

namespace GestionReservas.data;

public class AppDbContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>().ToTable("reservations");
    }
}