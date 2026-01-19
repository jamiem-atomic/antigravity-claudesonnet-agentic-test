using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleMarketplace.Api.Models;

public class Report
{
    public int Id { get; set; }

    public int ListingId { get; set; }

    public int ReporterId { get; set; }

    [Required]
    public ReportReason Reason { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;

    [ForeignKey("ReporterId")]
    public User Reporter { get; set; } = null!;
}
