using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleMarketplace.Api.Models;

public class Listing
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public string Make { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1950, 2027)]
    public int Year { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    public FuelType FuelType { get; set; }

    [Required]
    public Transmission Transmission { get; set; }

    [Required]
    public BodyType BodyType { get; set; }

    [Required]
    public VehicleCondition Condition { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    // Store as JSON string
    public string Photos { get; set; } = "[]";

    [Required]
    public ListingStatus Status { get; set; } = ListingStatus.Draft;

    [Required]
    public int SellerId { get; set; }

    public string? RejectionReason { get; set; }

    public string? RemovalReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SellerId")]
    public User Seller { get; set; } = null!;

    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<MessageThread> MessageThreads { get; set; } = new List<MessageThread>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
