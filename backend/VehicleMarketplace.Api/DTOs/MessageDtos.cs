namespace VehicleMarketplace.Api.DTOs;

public class MessageThreadDto
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public int BuyerId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? LastMessagePreview { get; set; }
}

public class MessageDto
{
    public int Id { get; set; }
    public int ThreadId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class CreateThreadRequest
{
    public int ListingId { get; set; }
    public string InitialMessage { get; set; } = string.Empty;
}

public class SendMessageRequest
{
    public string Body { get; set; } = string.Empty;
}
