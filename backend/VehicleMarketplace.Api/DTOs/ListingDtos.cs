using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.DTOs;

public class ListingDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? RemovalReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateListingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Mileage { get; set; }
    public FuelType FuelType { get; set; }
    public Transmission Transmission { get; set; }
    public BodyType BodyType { get; set; }
    public VehicleCondition Condition { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
}

public class UpdateListingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Mileage { get; set; }
    public FuelType FuelType { get; set; }
    public Transmission Transmission { get; set; }
    public BodyType BodyType { get; set; }
    public VehicleCondition Condition { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
}

public class RejectListingRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class RemoveListingRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ListingQueryParams
{
    public string? Search { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int? MileageMax { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public string? BodyType { get; set; }
    public string? Location { get; set; }
    public int? SellerId { get; set; }
    public string? SortBy { get; set; } // "price_asc", "price_desc", "year_desc", "newest"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
