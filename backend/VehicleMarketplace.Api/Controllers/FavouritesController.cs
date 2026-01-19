using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using VehicleMarketplace.Api.Data;
using VehicleMarketplace.Api.DTOs;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavouritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavouritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    [HttpGet]
    public async Task<ActionResult<List<FavouriteDto>>> GetFavourites()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var favourites = await _context.Favourites
            .Include(f => f.Listing)
            .ThenInclude(l => l.Seller)
            .Where(f => f.UserId == userId.Value)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        var result = favourites.Select(f => new FavouriteDto
        {
            Id = f.Id,
            Listing = MapListingToDto(f.Listing),
            CreatedAt = f.CreatedAt
        }).ToList();

        return Ok(result);
    }

    [HttpPost("{listingId}")]
    public async Task<ActionResult> AddFavourite(int listingId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        // Check if listing exists
        var listing = await _context.Listings.FindAsync(listingId);
        if (listing == null)
        {
            return NotFound(new { message = "Listing not found" });
        }

        // Check if already favourited
        var existing = await _context.Favourites
            .FirstOrDefaultAsync(f => f.UserId == userId.Value && f.ListingId == listingId);

        if (existing != null)
        {
            return Ok(new { message = "Already favourited" });
        }

        var favourite = new Favourite
        {
            UserId = userId.Value,
            ListingId = listingId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Favourites.Add(favourite);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Added to favourites" });
    }

    [HttpDelete("{listingId}")]
    public async Task<ActionResult> RemoveFavourite(int listingId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var favourite = await _context.Favourites
            .FirstOrDefaultAsync(f => f.UserId == userId.Value && f.ListingId == listingId);

        if (favourite == null)
        {
            return NotFound(new { message = "Favourite not found" });
        }

        _context.Favourites.Remove(favourite);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Removed from favourites" });
    }

    private static ListingDto MapListingToDto(Listing listing)
    {
        List<string> photos;
        try
        {
            photos = JsonSerializer.Deserialize<List<string>>(listing.Photos) ?? new List<string>();
        }
        catch
        {
            photos = new List<string>();
        }

        return new ListingDto
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Make = listing.Make,
            Model = listing.Model,
            Year = listing.Year,
            Mileage = listing.Mileage,
            FuelType = listing.FuelType.ToString(),
            Transmission = listing.Transmission.ToString(),
            BodyType = listing.BodyType.ToString(),
            Condition = listing.Condition.ToString(),
            Location = listing.Location,
            Photos = photos,
            Status = listing.Status.ToString(),
            SellerId = listing.SellerId,
            SellerName = listing.Seller?.DisplayName ?? "Unknown",
            RejectionReason = listing.RejectionReason,
            RemovalReason = listing.RemovalReason,
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt
        };
    }
}
