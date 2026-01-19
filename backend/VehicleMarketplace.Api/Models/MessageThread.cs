using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleMarketplace.Api.Models;

public class MessageThread
{
    public int Id { get; set; }

    public int ListingId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;

    [ForeignKey("BuyerId")]
    public User Buyer { get; set; } = null!;

    [ForeignKey("SellerId")]
    public User Seller { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
