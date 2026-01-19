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
public class ThreadsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ThreadsController(ApplicationDbContext context)
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
    public async Task<ActionResult<List<MessageThreadDto>>> GetThreads()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var query = _context.MessageThreads
            .Include(mt => mt.Listing)
            .Include(mt => mt.Buyer)
            .Include(mt => mt.Seller)
            .Include(mt => mt.Messages)
            .AsQueryable();

        // Filter threads for current user (or show all for admin)
        if (!IsAdmin())
        {
            query = query.Where(mt => mt.BuyerId == userId.Value || mt.SellerId == userId.Value);
        }

        var threads = await query
            .OrderByDescending(mt => mt.UpdatedAt)
            .ToListAsync();

        var result = threads.Select(mt => new MessageThreadDto
        {
            Id = mt.Id,
            ListingId = mt.ListingId,
            ListingTitle = mt.Listing.Title,
            BuyerId = mt.BuyerId,
            BuyerName = mt.Buyer.DisplayName,
            SellerId = mt.SellerId,
            SellerName = mt.Seller.DisplayName,
            CreatedAt = mt.CreatedAt,
            UpdatedAt = mt.UpdatedAt,
            LastMessagePreview = mt.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Body
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageThreadDto>> GetThread(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var thread = await _context.MessageThreads
            .Include(mt => mt.Listing)
            .Include(mt => mt.Buyer)
            .Include(mt => mt.Seller)
            .Include(mt => mt.Messages)
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(mt => mt.Id == id);

        if (thread == null)
        {
            return NotFound();
        }

        // Check access
        if (!IsAdmin() && thread.BuyerId != userId.Value && thread.SellerId != userId.Value)
        {
            return Forbid();
        }

        var result = new MessageThreadDto
        {
            Id = thread.Id,
            ListingId = thread.ListingId,
            ListingTitle = thread.Listing.Title,
            BuyerId = thread.BuyerId,
            BuyerName = thread.Buyer.DisplayName,
            SellerId = thread.SellerId,
            SellerName = thread.Seller.DisplayName,
            CreatedAt = thread.CreatedAt,
            UpdatedAt = thread.UpdatedAt
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MessageThreadDto>> CreateThread([FromBody] CreateThreadRequest request)
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

        // Get listing
        var listing = await _context.Listings.FindAsync(request.ListingId);
        if (listing == null)
        {
            return NotFound(new { message = "Listing not found" });
        }

        // Can't message your own listing
        if (listing.SellerId == userId.Value)
        {
            return BadRequest(new { message = "Cannot message your own listing" });
        }

        // Check if thread already exists
        var existingThread = await _context.MessageThreads
            .Include(mt => mt.Listing)
            .Include(mt => mt.Buyer)
            .Include(mt => mt.Seller)
            .FirstOrDefaultAsync(mt => mt.ListingId == request.ListingId && mt.BuyerId == userId.Value);

        if (existingThread != null)
        {
            // Add message to existing thread
            var existingMessage = new Message
            {
                ThreadId = existingThread.Id,
                SenderId = userId.Value,
                Body = request.InitialMessage,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(existingMessage);
            existingThread.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new MessageThreadDto
            {
                Id = existingThread.Id,
                ListingId = existingThread.ListingId,
                ListingTitle = existingThread.Listing.Title,
                BuyerId = existingThread.BuyerId,
                BuyerName = existingThread.Buyer.DisplayName,
                SellerId = existingThread.SellerId,
                SellerName = existingThread.Seller.DisplayName,
                CreatedAt = existingThread.CreatedAt,
                UpdatedAt = existingThread.UpdatedAt
            });
        }

        // Create new thread
        var thread = new MessageThread
        {
            ListingId = request.ListingId,
            BuyerId = userId.Value,
            SellerId = listing.SellerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.MessageThreads.Add(thread);
        await _context.SaveChangesAsync();

        // Add initial message
        var message = new Message
        {
            ThreadId = thread.Id,
            SenderId = userId.Value,
            Body = request.InitialMessage,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Load related data
        await _context.Entry(thread).Reference(t => t.Listing).LoadAsync();
        await _context.Entry(thread).Reference(t => t.Buyer).LoadAsync();
        await _context.Entry(thread).Reference(t => t.Seller).LoadAsync();

        return CreatedAtAction(nameof(GetThread), new { id = thread.Id }, new MessageThreadDto
        {
            Id = thread.Id,
            ListingId = thread.ListingId,
            ListingTitle = thread.Listing.Title,
            BuyerId = thread.BuyerId,
            BuyerName = thread.Buyer.DisplayName,
            SellerId = thread.SellerId,
            SellerName = thread.Seller.DisplayName,
            CreatedAt = thread.CreatedAt,
            UpdatedAt = thread.UpdatedAt
        });
    }

    [HttpGet("{id}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var thread = await _context.MessageThreads.FindAsync(id);
        if (thread == null)
        {
            return NotFound();
        }

        // Check access
        if (!IsAdmin() && thread.BuyerId != userId.Value && thread.SellerId != userId.Value)
        {
            return Forbid();
        }

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ThreadId == id)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ThreadId = m.ThreadId,
                SenderId = m.SenderId,
                SenderName = m.Sender.DisplayName,
                Body = m.Body,
                SentAt = m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("{id}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(int id, [FromBody] SendMessageRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        // Check if user is suspended
        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.IsSuspended)
        {
            return Forbid();
        }

        var thread = await _context.MessageThreads.FindAsync(id);
        if (thread == null)
        {
            return NotFound();
        }

        // Check access
        if (thread.BuyerId != userId.Value && thread.SellerId != userId.Value)
        {
            return Forbid();
        }

        var message = new Message
        {
            ThreadId = id,
            SenderId = userId.Value,
            Body = request.Body,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        thread.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new MessageDto
        {
            Id = message.Id,
            ThreadId = message.ThreadId,
            SenderId = message.SenderId,
            SenderName = user.DisplayName,
            Body = message.Body,
            SentAt = message.SentAt
        });
    }
}
