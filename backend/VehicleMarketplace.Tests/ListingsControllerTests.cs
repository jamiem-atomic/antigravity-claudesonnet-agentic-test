using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleMarketplace.Api.Controllers;
using VehicleMarketplace.Api.DTOs;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Tests;

public class ListingsControllerTests : TestBase
{
    private readonly ListingsController _controller;

    public ListingsControllerTests()
    {
        _controller = new ListingsController(_context);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private void SetupUser(int userId, bool isAdmin = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetListings_ShouldReturnPublishedListings()
    {
        // Arrange
        await SeedUsersAsync();
        var listings = new[]
        {
            new Listing { Id = 1, Title = "Pub 1", Status = ListingStatus.Published, Price = 10000, SellerId = 1, Description = "Desc", Make = "Make", Model = "Model" },
            new Listing { Id = 2, Title = "Draft 1", Status = ListingStatus.Draft, Price = 20000, SellerId = 1, Description = "Desc", Make = "Make", Model = "Model" },
            new Listing { Id = 3, Title = "Pub 2", Status = ListingStatus.Published, Price = 30000, SellerId = 1, Description = "Desc", Make = "Make", Model = "Model" }
        };
        _context.Listings.AddRange(listings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetListings(new ListingQueryParams());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResult = Assert.IsType<PagedResult<ListingDto>>(okResult.Value);
        Assert.Equal(2, pagedResult.TotalCount);
        Assert.DoesNotContain(pagedResult.Items, l => l.Status == "Draft");
    }

    [Fact]
    public async Task CreateListing_ShouldCreateDraft()
    {
        // Arrange
        await SeedUsersAsync();
        SetupUser(1);

        var request = new CreateListingRequest
        {
            Title = "New Car",
            Description = "Description",
            Price = 15000,
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            Mileage = 50000,
            FuelType = FuelType.Petrol,
            Transmission = Transmission.Automatic,
            BodyType = BodyType.Sedan,
            Condition = VehicleCondition.Used,
            Location = "Test City"
        };

        // Act
        var result = await _controller.CreateListing(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<ListingDto>(createdResult.Value);
        Assert.Equal("New Car", dto.Title);
        Assert.Equal("Draft", dto.Status);
        
        var savedListing = await _context.Listings.FindAsync(dto.Id);
        Assert.NotNull(savedListing);
        Assert.Equal(ListingStatus.Draft, savedListing.Status);
    }
}
