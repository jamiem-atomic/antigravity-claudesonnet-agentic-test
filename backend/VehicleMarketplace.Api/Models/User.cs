using System.ComponentModel.DataAnnotations;

namespace VehicleMarketplace.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public bool IsAdmin { get; set; } = false;

    public bool IsSuspended { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
