using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("Users");

        entity.HasKey(u => u.Id);

        entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
        entity.Property(u => u.PasswordHash).IsRequired();
        entity.Property(u => u.Role).HasConversion<int>().IsRequired();
        entity.Property(u => u.CreatedAt).IsRequired();
        entity.Property(u => u.IsActive).IsRequired();
    });

    modelBuilder.Entity<Ticket>(entity =>
    {
        entity.ToTable("Ticket");

        entity.HasKey(t => t.Id);

        entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
        entity.Property(t => t.Description).IsRequired();
        entity.Property(t => t.Status).HasConversion<int>().IsRequired();
        entity.Property(t => t.CreatedAt).IsRequired();
        entity.Property(t => t.UpdatedAt).IsRequired(false);
        entity.Property(t => t.UserEmail).IsRequired().HasMaxLength(255);
    });
}
}