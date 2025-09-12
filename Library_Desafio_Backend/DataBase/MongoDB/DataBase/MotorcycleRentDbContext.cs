using Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase
{
    public class MotorcycleRentDbContext : DbContext
    {
        public DbSet<DeliveryPersonClass> DeliveryPerson { get; set; }
        public DbSet<RentClass> Rent { get; set; }
        public DbSet<MotorcycleClass> Motorcycles { get; set; }
        public DbSet<MotorcycleHistoryClass> MotorcycleHistory { get; set; }
        public DbSet<RentalPlanClass> RentalPlan { get; set; }

        public MotorcycleRentDbContext(DbContextOptions<MotorcycleRentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryPersonClass>().ToCollection("DeliveryPerson");
            modelBuilder.Entity<DeliveryPersonClass>()
            .HasIndex(d => d.Identificador)
            .IsUnique(true);
            modelBuilder.Entity<DeliveryPersonClass>()
            .HasIndex(d => d.Cnpj)
            .IsUnique(true);
            modelBuilder.Entity<DeliveryPersonClass>()
            .HasIndex(d => d.Numero_Cnh)
            .IsUnique(true);

            modelBuilder.Entity<RentClass>().ToCollection("Rent");
            modelBuilder.Entity<RentClass>()
           .Property(r => r.Identificador)
           .ValueGeneratedOnAdd();          
            modelBuilder.Entity<RentClass>()
            .HasIndex(r => r.Identificador)
            .IsUnique(true);

            modelBuilder.Entity<MotorcycleClass>().ToCollection("Motorcycle");
            modelBuilder.Entity<MotorcycleClass>()
            .HasIndex(m => m.Identificador)
            .IsUnique(true);
            modelBuilder.Entity<MotorcycleClass>()
            .HasIndex(m => m.Placa)
            .IsUnique(true);

            modelBuilder.Entity<MotorcycleHistoryClass>().ToCollection("MotorcycleHistory");
            modelBuilder.Entity<MotorcycleHistoryClass>()
            .Property(h => h.Identificador)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<MotorcycleHistoryClass>()
            .HasIndex(h => h.Identificador)
            .IsUnique(true);

            modelBuilder.Entity<RentalPlanClass>().ToCollection("RentalPlan");
            modelBuilder.Entity<RentalPlanClass>()
           .Property(r => r.Identificador)
           .ValueGeneratedOnAdd();
            modelBuilder.Entity<RentalPlanClass>()
            .HasIndex(r => r.Identificador)
            .IsUnique(true);
        }
    }
}
