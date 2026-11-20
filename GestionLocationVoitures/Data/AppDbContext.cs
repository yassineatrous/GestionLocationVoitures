using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using GestionLocationVoitures.Models;

namespace GestionLocationVoitures.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Vehicule> Vehicules { get; set; }
        public DbSet<Entretien> Entretiens { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ContratLocation> ContratsLocation { get; set; }

        public static string GetDatabasePath()
        {
            DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && dir.GetFiles("*.csproj").Length == 0)
            {
                dir = dir.Parent;
            }

            string folder = dir != null ? dir.FullName : AppContext.BaseDirectory;
            return Path.Combine(folder, "location_voitures.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connString = new SqliteConnectionStringBuilder
                {
                    DataSource = GetDatabasePath()
                }.ToString();

                optionsBuilder.UseSqlite(connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<Agent>().ToTable("Agents");

            modelBuilder.Entity<Vehicule>()
                .HasMany(v => v.HistoriqueEntretiens)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContratLocation>()
                .HasOne(c => c.Reservation)
                .WithOne()
                .HasForeignKey<ContratLocation>("ReservationId");
        }
    }
}