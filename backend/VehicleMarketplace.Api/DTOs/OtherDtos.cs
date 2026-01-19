using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.DTOs;

public class ReportDto
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public int ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportRequest
{
    public int ListingId { get; set; }
    public ReportReason Reason { get; set; }
    public string? Comment { get; set; }
}

public class FavouriteDto
{
    public int Id { get; set; }
    public ListingDto Listing { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class MetricsDto
{
    public int TotalUsers { get; set; }
    public int TotalListings { get; set; }
    public int PublishedListings { get; set; }
    public int PendingListings { get; set; }
    public int RejectedListings { get; set; }
    public int TotalMessages { get; set; }
    public int TotalReports { get; set; }
}
