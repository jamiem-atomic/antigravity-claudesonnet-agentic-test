using Microsoft.EntityFrameworkCore;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<MessageThread> MessageThreads { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Listing configuration
        modelBuilder.Entity<Listing>()
            .HasOne(l => l.Seller)
            .WithMany(u => u.Listings)
            .HasForeignKey(l => l.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Listing>()
            .Property(l => l.Price)
            .HasPrecision(18, 2);

        // Favourite configuration
        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.Listing)
            .WithMany(l => l.Favourites)
            .HasForeignKey(f => f.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one user can favourite a listing only once
        modelBuilder.Entity<Favourite>()
            .HasIndex(f => new { f.UserId, f.ListingId })
            .IsUnique();

        // MessageThread configuration
        modelBuilder.Entity<MessageThread>()
            .HasOne(mt => mt.Listing)
            .WithMany(l => l.MessageThreads)
            .HasForeignKey(mt => mt.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MessageThread>()
            .HasOne(mt => mt.Buyer)
            .WithMany()
            .HasForeignKey(mt => mt.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MessageThread>()
            .HasOne(mt => mt.Seller)
            .WithMany()
            .HasForeignKey(mt => mt.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint: one thread per buyer-listing combination
        modelBuilder.Entity<MessageThread>()
            .HasIndex(mt => new { mt.BuyerId, mt.ListingId })
            .IsUnique();

        // Message configuration
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Thread)
            .WithMany(mt => mt.Messages)
            .HasForeignKey(m => m.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Report configuration
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Listing)
            .WithMany(l => l.Reports)
            .HasForeignKey(r => r.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reporter)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
