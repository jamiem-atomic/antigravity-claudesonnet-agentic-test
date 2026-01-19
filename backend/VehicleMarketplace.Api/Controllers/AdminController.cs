using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VehicleMarketplace.Api.Data;
using VehicleMarketplace.Api.DTOs;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Listings Management
    [HttpGet("listings")]
    public async Task<ActionResult<PagedResult<ListingDto>>> GetAllListings([FromQuery] ListingQueryParams queryParams)
    {
        var query = _context.Listings.Include(l => l.Seller).AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchLower = queryParams.Search.ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(searchLower) ||
                l.Make.ToLower().Contains(searchLower) ||
                l.Model.ToLower().Contains(searchLower));
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

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(l => MapListingToDto(l))
            .ToListAsync();

        return Ok(new PagedResult<ListingDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        });
    }

    [HttpPost("listings/{id}/approve")]
    public async Task<ActionResult<ListingDto>> ApproveListing(int id)
    {
        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        if (listing.Status != ListingStatus.PendingApproval)
        {
            return BadRequest(new { message = "Can only approve listings in PendingApproval status" });
        }

        listing.Status = ListingStatus.Published;
        listing.RejectionReason = null;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(MapListingToDto(listing));
    }

    [HttpPost("listings/{id}/reject")]
    public async Task<ActionResult<ListingDto>> RejectListing(int id, [FromBody] RejectListingRequest request)
    {
        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        if (listing.Status != ListingStatus.PendingApproval)
        {
            return BadRequest(new { message = "Can only reject listings in PendingApproval status" });
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { message = "Rejection reason is required" });
        }

        listing.Status = ListingStatus.Rejected;
        listing.RejectionReason = request.Reason;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(MapListingToDto(listing));
    }

    [HttpPost("listings/{id}/remove")]
    public async Task<ActionResult<ListingDto>> RemoveListing(int id, [FromBody] RemoveListingRequest request)
    {
        var listing = await _context.Listings.Include(l => l.Seller).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(new { message = "Removal reason is required" });
        }

        listing.Status = ListingStatus.Removed;
        listing.RemovalReason = request.Reason;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(MapListingToDto(listing));
    }

    // User Management
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Phone = u.Phone,
                Location = u.Location,
                IsAdmin = u.IsAdmin,
                IsSuspended = u.IsSuspended,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("users/{id}/suspend")]
    public async Task<ActionResult<UserDto>> SuspendUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.IsAdmin)
        {
            return BadRequest(new { message = "Cannot suspend admin users" });
        }

        user.IsSuspended = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Phone = user.Phone,
            Location = user.Location,
            IsAdmin = user.IsAdmin,
            IsSuspended = user.IsSuspended,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPost("users/{id}/unsuspend")]
    public async Task<ActionResult<UserDto>> UnsuspendUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsSuspended = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Phone = user.Phone,
            Location = user.Location,
            IsAdmin = user.IsAdmin,
            IsSuspended = user.IsSuspended,
            CreatedAt = user.CreatedAt
        });
    }

    // Reports Management
    [HttpGet("reports")]
    public async Task<ActionResult<List<ReportDto>>> GetAllReports()
    {
        var reports = await _context.Reports
            .Include(r => r.Listing)
            .Include(r => r.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                ListingId = r.ListingId,
                ListingTitle = r.Listing.Title,
                ReporterId = r.ReporterId,
                ReporterName = r.Reporter.DisplayName,
                Reason = r.Reason.ToString(),
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reports);
    }

    // Metrics
    [HttpGet("metrics")]
    public async Task<ActionResult<MetricsDto>> GetMetrics()
    {
        var metrics = new MetricsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalListings = await _context.Listings.CountAsync(),
            PublishedListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.Published),
            PendingListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.PendingApproval),
            RejectedListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.Rejected),
            TotalMessages = await _context.Messages.CountAsync(),
            TotalReports = await _context.Reports.CountAsync()
        };

        return Ok(metrics);
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
