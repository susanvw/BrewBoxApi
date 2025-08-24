using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BrewBoxApi.Domain.Aggregates.Identity;
using BrewBoxApi.Domain.SeedWork;
using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Domain.Aggregates.Drinks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<string>, string>(options)
{
      public DbSet<Order> Orders { get; set; }
      public DbSet<Drink> Drinks { get; set; }
      public DbSet<OrderDrink> OrderDrinks { get; set; }

      protected override void OnModelCreating(ModelBuilder builder)
      {
            base.OnModelCreating(builder);

            // Apply global query filter for soft deletes
            foreach (var entityType in builder.Model.GetEntityTypes()
                .Where(t => typeof(BaseModel).IsAssignableFrom(t.ClrType)))
            {
                  builder.Entity(entityType.ClrType)
                      .HasQueryFilter(GetIsNotDeletedFilter(entityType.ClrType));
            }

            // Configure Drink entity
            builder.Entity<Drink>(entity =>
            {
                  entity.HasKey(d => new { d.Type, d.Size }); // Composite key
                  entity.Property(d => d.Type)
                  .HasConversion<string>()
                  .HasMaxLength(100);
                  entity.Property(d => d.Size)
                  .HasConversion<string>()
                  .HasMaxLength(50);
                  entity.Property(d => d.Price)
                  .HasColumnType("decimal(18,2)");
                  entity.Property(d => d.CreatedById)
                  .HasMaxLength(450);
                  entity.HasOne(d => d.CreatedBy)
                  .WithMany()
                  .HasForeignKey(d => d.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
                  entity.HasIndex(d => new { d.Type, d.Size }); // Optimize lookups
            });

            // Configure Order entity
            builder.Entity<Order>(entity =>
            {
                  entity.HasKey(o => o.Id);
                  entity.Property(o => o.Id).HasMaxLength(64);
                  entity.Property(o => o.Status)
                  .HasConversion<string>()
                  .HasMaxLength(50);
                  entity.Property(o => o.TotalPrice)
                  .HasColumnType("decimal(18,2)");
                  entity.Property(o => o.Tip)
                  .HasColumnType("decimal(18,2)");
                  entity.Property(o => o.CreatedById)
                  .HasMaxLength(450);
                  entity.Property(o => o.BaristaId)
                  .HasMaxLength(450);
                  entity.HasOne(o => o.CreatedBy)
                  .WithMany()
                  .HasForeignKey(o => o.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
                  entity.HasOne(o => o.Barista)
                  .WithMany()
                  .HasForeignKey(o => o.BaristaId)
                  .OnDelete(DeleteBehavior.SetNull);
                  entity.HasIndex(o => o.CreatedById);
                  entity.HasIndex(o => o.BaristaId);
                  entity.HasIndex(o => o.Status);
            });

            // Configure OrderDrink entity
            builder.Entity<OrderDrink>(entity =>
            {
                  entity.HasKey(od => new { od.OrderId, od.DrinkId });
                  entity.Property(od => od.OrderId).HasMaxLength(64);
                  entity.Property(od => od.DrinkId).HasMaxLength(64);
                  entity.Property(od => od.Price)
                  .HasColumnType("decimal(18,2)");
                  entity.Property(od => od.Quantity)
                  .IsRequired();
                  entity.Property(od => od.CreatedById)
                  .HasMaxLength(450);
                  entity.HasOne(od => od.Order)
                  .WithMany(o => o.Drinks)
                  .HasForeignKey(od => od.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
                  entity.HasOne(od => od.Drink)
                  .WithMany()
                  .HasForeignKey(od => od.DrinkId)
                  .OnDelete(DeleteBehavior.Restrict);
                  entity.HasOne(od => od.CreatedBy)
                  .WithMany()
                  .HasForeignKey(od => od.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
                  entity.HasIndex(od => od.OrderId);
                  entity.HasIndex(od => od.DrinkId);
            });
      }

      private static LambdaExpression GetIsNotDeletedFilter(Type type)
      {
            var parameter = Expression.Parameter(type, "e");
            var body = Expression.IsFalse(
                Expression.Property(parameter, nameof(BaseModel.IsDeleted)));
            return Expression.Lambda(body, parameter);
      }
}