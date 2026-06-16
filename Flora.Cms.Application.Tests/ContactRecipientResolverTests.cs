using Flora.Cms.Application.Features.Contact;
using Xunit;

namespace Flora.Cms.Application.Tests;

public class ContactRecipientResolverTests
{
    private const string PayloadWithRecipient = """
        {
          "content": {
            "en": {
              "contact": { "formRecipientEmail": "en-inbox@floragroup.net" }
            },
            "ar": {
              "contact": { "formRecipientEmail": "ar-inbox@floragroup.net" }
            }
          }
        }
        """;

    [Fact]
    public void TryReadFromPayload_returns_locale_specific_inbox()
    {
        var en = ContactRecipientResolver.TryReadFromPayload(PayloadWithRecipient, "en");
        var ar = ContactRecipientResolver.TryReadFromPayload(PayloadWithRecipient, "ar");

        Assert.Equal("en-inbox@floragroup.net", en);
        Assert.Equal("ar-inbox@floragroup.net", ar);
    }

    [Fact]
    public void TryReadFromPayload_falls_back_to_en_when_locale_missing()
    {
        const string payload = """
            {
              "content": {
                "en": {
                  "contact": { "formRecipientEmail": "fallback@floragroup.net" }
                }
              }
            }
            """;

        var result = ContactRecipientResolver.TryReadFromPayload(payload, "ar");

        Assert.Equal("fallback@floragroup.net", result);
    }

    [Fact]
    public void TryReadFromPayload_returns_null_when_missing()
    {
        const string payload = """{ "content": { "en": { "contact": {} } } }""";

        Assert.Null(ContactRecipientResolver.TryReadFromPayload(payload, "en"));
    }
}
