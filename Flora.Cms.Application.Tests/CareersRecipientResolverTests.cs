using Flora.Cms.Application.Features.Contact;
using Xunit;

namespace Flora.Cms.Application.Tests;

/// <summary>
/// The careers alert recipient is resolved from the CMS payload's <c>careersCta.email</c>,
/// falling back to the contact inbox. These cover the shared payload reader for that path.
/// </summary>
public class CareersRecipientResolverTests
{
    private const string Payload = """
        {
          "content": {
            "en": {
              "careersCta": { "email": "hr-en@floragroup.net" },
              "contact": { "formRecipientEmail": "info-en@floragroup.net" }
            },
            "ar": {
              "careersCta": { "email": "hr-ar@floragroup.net" }
            }
          }
        }
        """;

    [Fact]
    public void Reads_careers_hr_email_per_locale()
    {
        Assert.Equal("hr-en@floragroup.net",
            ContactRecipientResolver.TryReadEmailFromPayload(Payload, "en", "careersCta", "email"));
        Assert.Equal("hr-ar@floragroup.net",
            ContactRecipientResolver.TryReadEmailFromPayload(Payload, "ar", "careersCta", "email"));
    }

    [Fact]
    public void Missing_careers_email_returns_null_so_caller_falls_back_to_contact_inbox()
    {
        const string payload = """
            { "content": { "en": { "contact": { "formRecipientEmail": "info@floragroup.net" } } } }
            """;

        Assert.Null(ContactRecipientResolver.TryReadEmailFromPayload(payload, "en", "careersCta", "email"));
        Assert.Equal("info@floragroup.net",
            ContactRecipientResolver.TryReadEmailFromPayload(payload, "en", "contact", "formRecipientEmail"));
    }
}
