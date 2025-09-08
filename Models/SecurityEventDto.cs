public class SecurityEventDto
{
    public Guid Id { get; set; }
    public required string EventType { get; set; }
    public DateTimeOffset OccurredUtc { get; set; }
    public required string Details { get; set; }
    public required string AuthorUserEmail { get; set; }
    public required string AffectedUserEmail { get; set; }
}