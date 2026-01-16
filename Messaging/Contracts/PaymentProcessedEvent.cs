namespace FCG.Lib.Shared.Messaging.Contracts;

public record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public string GameTitle { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}

public enum PaymentStatus
{
    Approved = 1,
    Rejected = 2
}
