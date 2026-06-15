using System.Text.Json;

namespace Flora.Cms.Api.Dtos;

/// <summary>
/// The canonical, strongly-typed shape of the UI configuration document that is
/// serialized into <c>UiConfiguration.PayloadJson</c> and returned verbatim to the
/// public site and Admin Portal.
///
/// Adding a new colour or branding field here requires NO database migration —
/// it simply widens the JSON written into the existing nvarchar(max) column.
/// </summary>
public class UiConfigPayload
{
    public ThemeDto Theme { get; set; } = new();
    public BrandingDto Branding { get; set; } = new();

    /// <summary>Per-section visibility toggles for the public homepage.</summary>
    public SectionsDto Sections { get; set; } = new();

    /// <summary>CMS-managed image URLs that override the site's hard-coded assets.</summary>
    public ImagesDto Images { get; set; } = new();

    /// <summary>
    /// Bilingual editable copy that overrides the public site's i18n dictionary.
    /// Deliberately a passthrough JSON value (not strongly typed) so the Admin Portal
    /// and public site can agree on the shape and add fields freely — the backend just
    /// stores and echoes it. Shape: <c>{ "en": { hero: {...}, ... }, "ar": { ... } }</c>.
    /// </summary>
    public JsonElement? Content { get; set; }

    /// <summary>Builds the seeded default payload, including baseline copy + image paths.</summary>
    public static UiConfigPayload CreateDefault() => new()
    {
        Content = JsonDocument.Parse(DefaultContent.Json).RootElement.Clone(),
    };
}

/// <summary>CMS-managed image URLs (relative to the public site or absolute).</summary>
public class ImagesDto
{
    public string Logo { get; set; } = "/images/flora-group-logo.png";
    public string HeroImage { get; set; } = "/images/hero-doha.jpg";
}

/// <summary>
/// Brand colour tokens. Values are CSS colour strings (hex, rgb(), or oklch()) — the
/// public site maps each one onto a CSS custom property (e.g. <c>--primary</c>) so
/// Tailwind updates instantly with no rebuild. Names mirror the design tokens in the
/// site's <c>app/globals.css</c>.
/// </summary>
public class ThemeDto
{
    // Core surface / text
    public string Background { get; set; } = "#fcfbf9";
    public string Foreground { get; set; } = "#3a2329";
    public string Card { get; set; } = "#ffffff";
    public string CardForeground { get; set; } = "#3a2329";

    // Brand primary (deep royal maroon)
    public string Primary { get; set; } = "#4a0e17";
    public string PrimaryForeground { get; set; } = "#fbf6ef";

    public string Secondary { get; set; } = "#f5f2ee";
    public string SecondaryForeground { get; set; } = "#4a0e17";

    public string Muted { get; set; } = "#f5f2ee";
    public string MutedForeground { get; set; } = "#6f6760";

    // Accent (champagne gold)
    public string Accent { get; set; } = "#c5a880";
    public string AccentForeground { get; set; } = "#42121b";

    public string Border { get; set; } = "#e8e3dc";
    public string Input { get; set; } = "#e8e3dc";
    public string Ring { get; set; } = "#c5a880";
    public string Destructive { get; set; } = "#c0392b";

    // Bespoke brand tokens used across the site
    public string Gold { get; set; } = "#c5a880";
    public string GoldSoft { get; set; } = "#ddc9a8";
    public string GoldForeground { get; set; } = "#42121b";
    public string Maroon { get; set; } = "#4a0e17";
    public string MaroonDeep { get; set; } = "#350a11";

    // Shape & type
    public string Radius { get; set; } = "0.625rem";
    public string FontHeading { get; set; } = "Playfair Display";
    public string FontSans { get; set; } = "Inter";
}

/// <summary>
/// Site-level branding and metadata. Mirrors values currently hard-coded in
/// <c>app/layout.tsx</c>.
/// </summary>
public class BrandingDto
{
    public string SiteTitle { get; set; } =
        "Flora Group W.L.L. — A Legacy of Excellence Across Qatar";

    public string MetaDescription { get; set; } =
        "Since 1993, Flora Group has been a trusted Qatari conglomerate spanning " +
        "technology, infrastructure, security, commodities, and premium trade.";

    public string LogoUrl { get; set; } = "/images/flora-group-logo.png";

    /// <summary>Browser theme-color meta tag value.</summary>
    public string ThemeColor { get; set; } = "#4A0E17";
}

/// <summary>Toggles for the optional homepage sections (in render order).</summary>
public class SectionsDto
{
    public bool ShowStats { get; set; } = true;
    public bool ShowSubsidiaries { get; set; } = true;
    public bool ShowSpotlight { get; set; } = true;
}
