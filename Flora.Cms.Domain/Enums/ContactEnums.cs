namespace Flora.Cms.Domain.Enums;

/// <summary>Triage state of a contact inquiry in the admin inbox.</summary>
public enum ContactInquiryStatus
{
    /// <summary>Just received, not yet looked at.</summary>
    NEW,

    /// <summary>Opened/acknowledged by an admin.</summary>
    READ,

    /// <summary>Cleared from the active inbox without deleting the record.</summary>
    ARCHIVED,
}

/// <summary>How outbound notification emails are delivered.</summary>
public enum EmailProvider
{
    /// <summary>Write the email to the server log instead of sending (safe default for local/dev).</summary>
    LOG,

    /// <summary>Deliver over SMTP (Mailtrap, Gmail, corporate relay, …).</summary>
    SMTP,

    /// <summary>Deliver via the Resend HTTP API.</summary>
    RESEND,
}
