using BrewBoxApi.Domain.Aggregates.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BrewBoxApi.Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Identity;
using BrewBoxApi.Domain.SeedWork;
using System.Linq.Expressions;

namespace BrewBoxApi.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : IdentityDbContext<ApplicationUser, IdentityRole<string>, string>(options)
{
      public DbSet<Order> Orders { get; set; }
      public DbSet<Drink> Drinks { get; set; }

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
                  entity.HasOne(o => o.CreatedBy)
                        .WithMany()
                        .HasForeignKey(o => o.CreatedById)
                        .OnDelete(DeleteBehavior.Restrict);
                  entity.HasOne(o => o.Barista)
                        .WithMany()
                        .HasForeignKey(o => o.BaristaId)
                        .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Drink entity
            builder.Entity<Drink>(entity =>
            {
                  entity.HasKey(d => d.Id);
                  entity.Property(d => d.Id).HasMaxLength(64);
                  entity.Property(d => d.Type)
                        .HasMaxLength(100);
                  entity.Property(d => d.Size)
                        .HasConversion<string>()
                        .HasMaxLength(50);
                  entity.Property(d => d.Price)
                        .HasColumnType("decimal(18,2)");
                  entity.Property(d => d.CreatedById)
                        .HasMaxLength(450);
                  entity.HasOne(d => d.Order)
                        .WithMany(o => o.Drinks)
                        .HasForeignKey(d => d.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);

                  entity.HasOne(d => d.CreatedBy)
                        .WithMany()
                        .HasForeignKey(d => d.CreatedById)
                        .OnDelete(DeleteBehavior.Restrict);
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
