namespace VehicleMarketplace.Api.Models;

public enum FuelType
{
    Petrol,
    Diesel,
    Hybrid,
    Electric,
    Other
}

public enum Transmission
{
    Manual,
    Automatic,
    Other
}

public enum BodyType
{
    Hatchback,
    Sedan,
    SUV,
    Van,
    Truck,
    Coupe,
    Convertible,
    Other
}

public enum VehicleCondition
{
    New,
    Used,
    ForParts
}

public enum ListingStatus
{
    Draft,
    PendingApproval,
    Published,
    Rejected,
    Unpublished,
    Removed
}
