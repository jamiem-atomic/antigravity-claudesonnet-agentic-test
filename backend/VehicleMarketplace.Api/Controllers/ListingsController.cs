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
public class ListingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ListingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ListingDto>>> GetListings([FromQuery] ListingQueryParams queryParams)
    {
        var query = _context.Listings.Include(l => l.Seller).AsQueryable();

        // Filter by status - only show published listings to non-admin users
        // UNLESS the user is strictly filtering by their own SellerId (My Listings view)
        var currentUserId = GetCurrentUserId();
        bool viewingOwnListings = currentUserId.HasValue && 
                                  queryParams.SellerId.HasValue && 
                                  queryParams.SellerId.Value == currentUserId.Value;

        if (!IsAdmin() && !viewingOwnListings)
        {
            query = query.Where(l => l.Status == ListingStatus.Published);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchLower = queryParams.Search.ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(searchLower) ||
                l.Make.ToLower().Contains(searchLower) ||
                l.Model.ToLower().Contains(searchLower));
        }

        // Apply filters
        if (!string.IsNullOrWhiteSpace(queryParams.Make))
        {
            query = query.Where(l => l.Make.ToLower() == queryParams.Make.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Model))
        {
            query = query.Where(l => l.Model.ToLower() == queryParams.Model.ToLower());
        }

        if (queryParams.PriceMin.HasValue)
        {
            query = query.Where(l => l.Price >= queryParams.PriceMin.Value);
        }

        if (queryParams.PriceMax.HasValue)
        {
            query = query.Where(l => l.Price <= queryParams.PriceMax.Value);
        }

        if (queryParams.YearMin.HasValue)
        {
            query = query.Where(l => l.Year >= queryParams.YearMin.Value);
        }

        if (queryParams.YearMax.HasValue)
        {
            query = query.Where(l => l.Year <= queryParams.YearMax.Value);
        }

        if (queryParams.MileageMax.HasValue)
        {
            query = query.Where(l => l.Mileage <= queryParams.MileageMax.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.FuelType))
        {
            if (Enum.TryParse<FuelType>(queryParams.FuelType, true, out var fuelType))
            {
                query = query.Where(l => l.FuelType == fuelType);
            }
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Transmission))
        {
            if (Enum.TryParse<Transmission>(queryParams.Transmission, true, out var transmission))
            {
                query = query.Where(l => l.Transmission == transmission);
            }
        }

        if (!string.IsNullOrWhiteSpace(queryParams.BodyType))
        {
            if (Enum.TryParse<BodyType>(queryParams.BodyType, true, out var bodyType))
            {
                query = query.Where(l => l.BodyType == bodyType);
            }
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Location))
        {
            query = query.Where(l => l.Location.ToLower().Contains(queryParams.Location.ToLower()));
        }

        if (queryParams.SellerId.HasValue)
        {
            query = query.Where(l => l.SellerId == queryParams.SellerId.Value);
        }

        // Apply sorting
        query = queryParams.SortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(l => l.Price),
            "price_desc" => query.OrderByDescending(l => l.Price),
            "year_desc" => query.OrderByDescending(l => l.Year),
            "newest" => query.OrderByDescending(l => l.CreatedAt),
            _ => query.OrderByDescending(l => l.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(l => MapToDto(l))
            .ToListAsync();

        return Ok(new PagedResult<ListingDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ListingDto>> GetListing(int id)
    {
        var listing = await _context.Listings
            .Include(l => l.Seller)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null)
        {
            return NotFound();
        }

        // Check visibility
        var currentUserId = GetCurrentUserId();
        if (listing.Status != ListingStatus.Published &&
            !IsAdmin() &&
            listing.SellerId != currentUserId)
        {
            return NotFound();
        }

        return Ok(MapToDto(listing));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ListingDto>> CreateListing([FromBody] CreateListingRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        // Check if user is suspended
        var user = await _context.Users.FindAsync(userId.Value);
        if (user?.IsSuspended == true)
        {
            return Forbid();
        }

        var listing = new Listing
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Mileage = request.Mileage,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            BodyType = request.BodyType,
            Condition = request.Condition,
            Location = request.Location,
            Photos = JsonSerializer.Serialize(request.Photos),
            Status = ListingStatus.Draft,
            SellerId = userId.Value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Listings.Add(listing);
        await _context.SaveChangesAsync();

        listing.Seller = user!;
        return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, MapToDto(listing));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ListingDto>> UpdateListing(int id, [FromBody] UpdateListingRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        // Check ownership
        if (listing.SellerId != userId.Value && !IsAdmin())
        {
            return Forbid();
        }

        // Check if user is suspended
        var user = await _context.Users.FindAsync(userId.Value);
        if (user?.IsSuspended == true)
        {
            return Forbid();
        }

        // Only allow editing if in Draft or Rejected status
        if (listing.Status != ListingStatus.Draft && listing.Status != ListingStatus.Rejected && !IsAdmin())
        {
            return BadRequest(new { message = "Can only edit listings in Draft or Rejected status" });
        }

        listing.Title = request.Title;
        listing.Description = request.Description;
        listing.Price = request.Price;
        listing.Make = request.Make;
        listing.Model = request.Model;
        listing.Year = request.Year;
        listing.Mileage = request.Mileage;
        listing.FuelType = request.FuelType;
        listing.Transmission = request.Transmission;
        listing.BodyType = request.BodyType;
        listing.Condition = request.Condition;
        listing.Location = request.Location;
        listing.Photos = JsonSerializer.Serialize(request.Photos);
        listing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToDto(listing));
    }

    [HttpPost("{id}/submit")]
    [Authorize]
    public async Task<ActionResult<ListingDto>> SubmitListing(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        // Check ownership
        if (listing.SellerId != userId.Value)
        {
            return Forbid();
        }

        // Check if user is suspended
        var user = await _context.Users.FindAsync(userId.Value);
        if (user?.IsSuspended == true)
        {
            return Forbid();
        }

        if (listing.Status != ListingStatus.Draft && listing.Status != ListingStatus.Rejected)
        {
            return BadRequest(new { message = "Can only submit listings in Draft or Rejected status" });
        }

        listing.Status = ListingStatus.PendingApproval;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(MapToDto(listing));
    }

    [HttpPost("{id}/unpublish")]
    [Authorize]
    public async Task<ActionResult<ListingDto>> UnpublishListing(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        // Check ownership
        if (listing.SellerId != userId.Value && !IsAdmin())
        {
            return Forbid();
        }

        if (listing.Status != ListingStatus.Published)
        {
            return BadRequest(new { message = "Can only unpublish published listings" });
        }

        listing.Status = ListingStatus.Unpublished;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(MapToDto(listing));
    }

    private static ListingDto MapToDto(Listing listing)
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
