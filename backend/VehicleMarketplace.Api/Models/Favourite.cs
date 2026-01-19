using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleMarketplace.Api.Models;

public class Favourite
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ListingId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;
}
