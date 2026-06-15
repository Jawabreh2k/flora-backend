using System.Text.Json;
using Flora.Cms.Api.Services;

namespace Flora.Cms.Api.Tests;

public class ContentMergerTests
{
    // ── IsCorrupted ──────────────────────────────────────────────────────────

    [Fact]
    public void IsCorrupted_EmptyString_ReturnsFalse()
        => Assert.False(ContentMerger.IsCorrupted(""));

    [Fact]
    public void IsCorrupted_WhitespaceOnly_ReturnsFalse()
        => Assert.False(ContentMerger.IsCorrupted("   "));

    [Fact]
    public void IsCorrupted_AllQuestionMarks_ReturnsTrue()
        => Assert.True(ContentMerger.IsCorrupted("?????? ??????"));

    [Fact]
    public void IsCorrupted_QuestionMarksAndPunctuation_ReturnsTrue()
        => Assert.True(ContentMerger.IsCorrupted("??, ??, ??!"));

    [Fact]
    public void IsCorrupted_RealEnglishText_ReturnsFalse()
        => Assert.False(ContentMerger.IsCorrupted("A legacy of excellence"));

    [Fact]
    public void IsCorrupted_RealArabicText_ReturnsFalse()
        => Assert.False(ContentMerger.IsCorrupted("إرث من التميز"));

    [Fact]
    public void IsCorrupted_MixedQuestionMarksAndRealChars_ReturnsFalse()
        => Assert.False(ContentMerger.IsCorrupted("??a??"));

    // ── MergeDefaults — empty / null input ──────────────────────────────────

    [Fact]
    public void MergeDefaults_NullElement_ReturnsFullDefaults()
    {
        var result = ContentMerger.MergeDefaults(default);

        Assert.Equal(JsonValueKind.Object, result.ValueKind);
        // Both locales must be present.
        Assert.True(result.TryGetProperty("en", out _));
        Assert.True(result.TryGetProperty("ar", out _));
    }

    [Fact]
    public void MergeDefaults_EmptyObject_ReturnsFullDefaults()
    {
        var empty = JsonDocument.Parse("{}").RootElement;
        var result = ContentMerger.MergeDefaults(empty);

        Assert.True(result.TryGetProperty("en", out var en));
        Assert.True(en.TryGetProperty("hero", out _));
    }

    // ── MergeDefaults — user edits are preserved ─────────────────────────────

    [Fact]
    public void MergeDefaults_ExistingFieldValue_IsPreserved()
    {
        var input = JsonDocument.Parse("""
            {
              "en": { "hero": { "badge": "Custom Badge Value" } },
              "ar": {}
            }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        result.TryGetProperty("en", out var en);
        en.TryGetProperty("hero", out var hero);
        hero.TryGetProperty("badge", out var badge);

        Assert.Equal("Custom Badge Value", badge.GetString());
    }

    // ── MergeDefaults — back-fills missing content ───────────────────────────

    [Fact]
    public void MergeDefaults_MissingGroup_IsBackfilledFromDefaults()
    {
        // Input has "en" but no "footer" group.
        var input = JsonDocument.Parse("""
            {
              "en": { "hero": { "badge": "My Badge" } },
              "ar": {}
            }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        result.TryGetProperty("en", out var en);
        Assert.True(en.TryGetProperty("footer", out var footer));
        // The default footer should have the careersTitle field.
        Assert.True(footer.TryGetProperty("careersTitle", out var careersTitle));
        Assert.False(string.IsNullOrEmpty(careersTitle.GetString()));
    }

    [Fact]
    public void MergeDefaults_MissingField_IsBackfilledFromDefaults()
    {
        // Input has the hero group but is missing the "subtitle" field.
        var input = JsonDocument.Parse("""
            {
              "en": { "hero": { "badge": "My Badge", "titleA": "Flora" } },
              "ar": {}
            }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        result.TryGetProperty("en", out var en);
        en.TryGetProperty("hero", out var hero);
        Assert.True(hero.TryGetProperty("subtitle", out var subtitle));
        Assert.False(string.IsNullOrEmpty(subtitle.GetString()));
    }

    [Fact]
    public void MergeDefaults_MissingLocale_IsBackfilledFromDefaults()
    {
        // Only "en" is present; "ar" is entirely missing.
        var input = JsonDocument.Parse("""
            { "en": { "hero": { "badge": "Badge" } } }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        Assert.True(result.TryGetProperty("ar", out var ar));
        Assert.True(ar.TryGetProperty("hero", out _));
    }

    // ── MergeDefaults — corruption healing ───────────────────────────────────

    [Fact]
    public void MergeDefaults_CorruptedField_IsReplacedWithDefault()
    {
        var input = JsonDocument.Parse("""
            {
              "en": { "hero": { "badge": "?????? ??????", "titleA": "Real Value" } },
              "ar": {}
            }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        result.TryGetProperty("en", out var en);
        en.TryGetProperty("hero", out var hero);
        hero.TryGetProperty("badge", out var badge);

        // Corrupted badge must not be "?????? ??????" — replaced by the default.
        Assert.NotEqual("?????? ??????", badge.GetString());
        Assert.False(string.IsNullOrEmpty(badge.GetString()));

        // Non-corrupted field must be preserved.
        hero.TryGetProperty("titleA", out var titleA);
        Assert.Equal("Real Value", titleA.GetString());
    }

    // ── MergeDefaults — extra user groups are preserved ─────────────────────

    [Fact]
    public void MergeDefaults_ExtraUserGroup_IsCarriedOver()
    {
        var input = JsonDocument.Parse("""
            {
              "en": {
                "hero": { "badge": "Badge" },
                "customGroup": { "field": "custom value" }
              },
              "ar": {}
            }
            """).RootElement;

        var result = ContentMerger.MergeDefaults(input);

        result.TryGetProperty("en", out var en);
        Assert.True(en.TryGetProperty("customGroup", out var custom));
        custom.TryGetProperty("field", out var field);
        Assert.Equal("custom value", field.GetString());
    }

    // ── MergeDefaults — idempotency ──────────────────────────────────────────

    [Fact]
    public void MergeDefaults_CalledTwice_IsIdempotent()
    {
        var input = JsonDocument.Parse("""
            {
              "en": { "hero": { "badge": "My Badge" } },
              "ar": {}
            }
            """).RootElement;

        var first = ContentMerger.MergeDefaults(input);
        var second = ContentMerger.MergeDefaults(first);

        first.TryGetProperty("en", out var en1);
        en1.TryGetProperty("hero", out var hero1);
        hero1.TryGetProperty("badge", out var badge1);

        second.TryGetProperty("en", out var en2);
        en2.TryGetProperty("hero", out var hero2);
        hero2.TryGetProperty("badge", out var badge2);

        Assert.Equal(badge1.GetString(), badge2.GetString());
    }
}
