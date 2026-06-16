namespace Flora.Cms.Domain.Entities;

public class ContactInquiry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public required string Email { get; set; }

    public string? Company { get; set; }

    public required string Subject { get; set; }

    public required string Message { get; set; }

    public string Locale { get; set; } = "en";

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
