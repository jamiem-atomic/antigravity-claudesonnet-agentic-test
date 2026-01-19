using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VehicleMarketplace.Api.Data;
using VehicleMarketplace.Api.DTOs;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        // Check if listing exists
        var listing = await _context.Listings.FindAsync(request.ListingId);
        if (listing == null)
        {
            return NotFound(new { message = "Listing not found" });
        }

        var report = new Report
        {
            ListingId = request.ListingId,
            ReporterId = userId.Value,
            Reason = request.Reason,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        // Load related data
        await _context.Entry(report).Reference(r => r.Listing).LoadAsync();
        await _context.Entry(report).Reference(r => r.Reporter).LoadAsync();

        return Ok(new ReportDto
        {
            Id = report.Id,
            ListingId = report.ListingId,
            ListingTitle = report.Listing.Title,
            ReporterId = report.ReporterId,
            ReporterName = report.Reporter.DisplayName,
            Reason = report.Reason.ToString(),
            Comment = report.Comment,
            CreatedAt = report.CreatedAt
        });
    }
}
