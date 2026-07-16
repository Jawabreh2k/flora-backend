using System.Text.Json;

namespace Flora.Cms.Application.Features.UiConfig.Contracts;

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

    /// <summary>
    /// Independently editable dark-mode palette. Same shape as <see cref="Theme"/> —
    /// the public site applies it scoped under its `.dark` class, the Admin Portal
    /// edits it via the same Appearance editor with a light/dark switch.
    /// </summary>
    public ThemeDto ThemeDark { get; set; } = ThemeDto.CreateDefaultDark();

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

    /// <summary>Managed subsidiaries with full localization support (EN/AR).</summary>
    public SubsidiariesConfigDto? Subsidiaries { get; set; }

    /// <summary>Social media links shown in the site footer.</summary>
    public SocialDto? Social { get; set; }

    /// <summary>Builds the seeded default payload, including baseline copy + image paths.</summary>
    public static UiConfigPayload CreateDefault() => new()
    {
        ThemeDark = ThemeDto.CreateDefaultDark(),
        Content = JsonDocument.Parse(DefaultContent.Json).RootElement.Clone(),
        Subsidiaries = SubsidiariesConfigDto.CreateDefaults(),
        Social = SocialDto.CreateDefault(),
    };
}

/// <summary>CMS-managed image URLs (relative to the public site or absolute).</summary>
public class ImagesDto
{
    public string Logo { get; set; } = "/images/flora-group-logo.png";
    public string HeroImage { get; set; } = "/images/hero-doha.jpg";
    public string ContactHero { get; set; } = "/images/contact-doha.jpg";
    public string CareersHero { get; set; } = "/images/careers-hero.jpg";
}

/// <summary>Social media links for the public site footer.</summary>
public class SocialDto
{
    public bool Enabled { get; set; }
    public List<SocialLinkDto> Links { get; set; } = [];

    public static SocialDto CreateDefault() => new()
    {
        Enabled = false,
        Links = [],
    };
}

public class SocialLinkDto
{
    public string Platform { get; set; } = "linkedin";
    public string Url { get; set; } = "";
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

    /// <summary>
    /// Seeded dark-mode palette — reweights the brand's two colours rather than
    /// inverting to grey: maroon becomes the deep surface colour, gold becomes
    /// the primary interactive accent. Shape/typography intentionally mirror
    /// the light defaults (the public site only applies the colour tokens from
    /// this variant, never radius/fonts, since shape doesn't change with theme).
    /// </summary>
    public static ThemeDto CreateDefaultDark() => new()
    {
        Background = "#16090a",
        Foreground = "#f2eae1",
        Card = "#231212",
        CardForeground = "#f2eae1",
        Primary = "#e2b577",
        PrimaryForeground = "#22080a",
        Secondary = "#2e1a1b",
        SecondaryForeground = "#ede3d6",
        Muted = "#281616",
        MutedForeground = "#a28d82",
        Accent = "#e2b577",
        AccentForeground = "#22080a",
        Border = "#3a2525",
        Input = "#3a2525",
        Ring = "#e2b577",
        Destructive = "#f14442",
        Gold = "#e2b577",
        GoldSoft = "#4b3216",
        GoldForeground = "#22080a",
        Maroon = "#500516",
        MaroonDeep = "#190002",
        Radius = "0.625rem",
        FontHeading = "Playfair Display",
        FontSans = "Inter",
    };
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

/// <summary>Managed subsidiaries container with enabled flag and list of subsidiaries.</summary>
public class SubsidiariesConfigDto
{
    public bool Enabled { get; set; } = true;
    public List<ManagedSubsidiaryDto> Items { get; set; } = new();

    public static SubsidiariesConfigDto CreateDefaults() => new()
    {
        Enabled = true,
        Items = ManagedSubsidiaryDto.GetDefaults(),
    };
}

/// <summary>A single managed subsidiary with full localization (EN/AR).</summary>
public class ManagedSubsidiaryDto
{
    public string Slug { get; set; } = string.Empty;
    public string Icon { get; set; } = "Package";
    public LocalizedStringDto Name { get; set; } = new();
    public LocalizedStringDto Tag { get; set; } = new();
    public LocalizedStringDto Short { get; set; } = new();
    public LocalizedStringDto Established { get; set; } = new();
    public VisualDto Visual { get; set; } = new();
    public string? Image { get; set; }
    public string? Logo { get; set; }
    public LocalizedArrayDto<string> Paragraphs { get; set; } = new();
    public LocalizedArrayDto<HighlightDto> Highlights { get; set; } = new();
    public LocalizedArrayDto<MetricDto> Metrics { get; set; } = new();
    public string Website { get; set; } = string.Empty;
    public bool HasProfile { get; set; } = false;
    public SubsidiaryContactDto Contact { get; set; } = new();

    /// <summary>
    /// Hides the subsidiary from the public site (homepage grid, footer, sitemap,
    /// and its own /subsidiaries/{slug} page) without deleting its data. Defaults
    /// to true so existing stored JSON without this field deserializes as visible.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Optional list of named clients/partners shown as a "Trusted By" wordmark
    /// strip on the subsidiary's profile page. Null/empty hides the section.
    /// </summary>
    public LocalizedArrayDto<string>? Clients { get; set; }

    /// <summary>
    /// Optional override for the "Visit Website" button label (e.g. "Shop the
    /// Online Store" for a subsidiary with a live storefront). Falls back to the
    /// public site's default translation when null/empty.
    /// </summary>
    public LocalizedStringDto? WebsiteLabel { get; set; }

    /// <summary>Returns all 6 default subsidiaries from hardcoded data.</summary>
    public static List<ManagedSubsidiaryDto> GetDefaults()
    {
        return new()
        {
            new()
            {
                Slug = "flora-trading",
                Icon = "Package",
                Name = new() { En = "Flora Trading Center", Ar = "مركز فلورا التجاري" },
                Tag = new() { En = "FMCG & Commodities", Ar = "السلع الاستهلاكية والتجارية" },
                Short = new()
                {
                    En = "Home of the celebrated AMEERA spice brand — processed to strict international quality standards since 1993.",
                    Ar = "موطن علامة «أميرة» الشهيرة للبهارات — تُصنَّع وفق أعلى معايير الجودة العالمية منذ عام ١٩٩٣.",
                },
                Established = new() { En = "1993", Ar = "١٩٩٣" },
                Visual = new() { From = "#5a1020", To = "#8a2a17", Glow = "#e0a23a" },
                Image = "/images/flora-trading-cover.jpg",
                Logo = "/images/flora-trading-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Flora Trading Center began operations in 1993 as a processor of AMEERA spices in Qatar. Over the decades it has grown in stature while contributing meaningfully to the wider socio-economic development of the Flora Group.",
                        "\"AMEERA\" is today one of the most admired spice brands among the people of Qatar — a household name built on consistency, taste and trust.",
                        "Every AMEERA product is produced to international standards, maintaining the highest level of quality at each stage of the production process. The range now spans more than 20 spice products across a wide variety of pack sizes.",
                    },
                    Ar = new()
                    {
                        "بدأ مركز فلورا التجاري عملياته عام ١٩٩٣ كمُصنّعٍ لبهارات «أميرة» في قطر. وعلى مدى العقود، نمت مكانته وأسهم إسهاماً ملموساً في التنمية الاجتماعية والاقتصادية الأوسع لمجموعة فلورا.",
                        "تُعدّ «أميرة» اليوم من أكثر علامات البهارات تقديراً لدى أهل قطر — اسمٌ مألوفٌ بُني على الاتساق والمذاق والثقة.",
                        "يُنتَج كلّ منتجٍ من «أميرة» وفق المعايير الدولية، مع الحفاظ على أعلى مستويات الجودة في كلّ مرحلةٍ من مراحل الإنتاج. وتضمّ التشكيلة الآن أكثر من ٢٠ منتجاً من البهارات بأحجام تعبئةٍ متنوّعة.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "The AMEERA Brand", Body = "A beloved household spice brand with deep equity across the Qatari market and the wider Gulf." },
                        new() { Title = "International Quality", Body = "Production governed by international food-safety and quality standards at every stage." },
                        new() { Title = "20+ Product Lines", Body = "More than twenty distinct spice products offered across a broad range of pack weights." },
                        new() { Title = "Three Decades of Trust", Body = "Operating continuously since 1993, anchoring the Group's commodities vertical." },
                    },
                    Ar = new()
                    {
                        new() { Title = "علامة «أميرة»", Body = "علامة بهاراتٍ محبوبة وراسخة المكانة في السوق القطري والخليج الأوسع." },
                        new() { Title = "جودة عالمية", Body = "إنتاجٌ يخضع لمعايير سلامة الغذاء والجودة الدولية في كلّ مرحلة." },
                        new() { Title = "أكثر من ٢٠ منتجاً", Body = "أكثر من عشرين منتجاً مختلفاً من البهارات بمجموعةٍ واسعة من أوزان التعبئة." },
                        new() { Title = "ثلاثة عقودٍ من الثقة", Body = "عملٌ متواصل منذ عام ١٩٩٣، يشكّل ركيزة قطاع السلع في المجموعة." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "1993", Label = "Established" },
                        new() { Value = "20+", Label = "Spice Products" },
                        new() { Value = "#1", Label = "Admired Local Brand" },
                    },
                    Ar = new()
                    {
                        new() { Value = "١٩٩٣", Label = "التأسيس" },
                        new() { Value = "+٢٠", Label = "منتج بهارات" },
                        new() { Value = "الأولى", Label = "علامة محلية محبوبة" },
                    },
                },
                Website = "https://floratradingcenter.net",
                HasProfile = false,
                Contact = new()
                {
                    Address = new() { En = "Industrial Area St. 1, Doha, Qatar. P.O. Box: 16118.", Ar = "المنطقة الصناعية، شارع ١، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44607703 / 44607704",
                },
            },
            new()
            {
                Slug = "flora-technology",
                Icon = "Cpu",
                Name = new() { En = "Flora Technology", Ar = "فلورا للتكنولوجيا" },
                Tag = new() { En = "Enterprise IT Solutions", Ar = "حلول تقنية المعلومات للمؤسسات" },
                Short = new()
                {
                    En = "State-of-the-art, end-to-end technology solutions for Qatar's governmental and private institutions.",
                    Ar = "حلولٌ تقنية متكاملة وحديثة للمؤسسات الحكومية والخاصة في قطر.",
                },
                Established = new() { En = "Group Branch", Ar = "فرع المجموعة" },
                Visual = new() { From = "#360a16", To = "#1b1633", Glow = "#c9a227" },
                Image = "/images/flora-technology-cover.jpg",
                Logo = "/images/flora-technology-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Flora Technology Co. was established as a branch of the Flora Group with a clear focus: to deliver state-of-the-art, end-to-end technology solutions and services to Qatari governmental and private entities.",
                        "Over the years we have developed the expertise to provide highly customised advanced technology solutions — a full lifecycle of services from initial requirements gathering and systems analysis through implementation, installation, programming and custom software development, all the way to end-user training, maintenance and support.",
                        "As a Flora Group company we remain strictly committed to the Group's values — placing customer satisfaction at the centre of everything and striving for the highest quality in every product and service. Our experienced professionals drive innovation with the discipline of the Group's high professional and ethical standards.",
                        "Recent engagements include delivering the technology infrastructure behind the opening of Leabaib Health Center and engineering the systems for the Government Service Complex in Messamir — real public-sector deployments supporting Qatar's expanding institutional footprint.",
                    },
                    Ar = new()
                    {
                        "تأسّست شركة فلورا للتكنولوجيا كفرعٍ من مجموعة فلورا بهدفٍ واضح: تقديم حلولٍ وخدماتٍ تقنية متكاملة وحديثة للجهات الحكومية والخاصة في قطر.",
                        "على مرّ السنين طوّرنا الخبرة لتقديم حلولٍ تقنية متقدّمة ومُخصّصة — دورة خدماتٍ كاملة من جمع المتطلبات وتحليل الأنظمة، مروراً بالتنفيذ والتركيب والبرمجة وتطوير البرمجيات المُخصّصة، وصولاً إلى تدريب المستخدمين والصيانة والدعم.",
                        "بصفتنا إحدى شركات مجموعة فلورا، نلتزم التزاماً صارماً بقيم المجموعة — واضعين رضا العميل في صميم كلّ ما نقوم به، وساعين لأعلى جودةٍ في كلّ منتجٍ وخدمة. ويقود خبراؤنا الابتكار بانضباط معايير المجموعة المهنية والأخلاقية العالية.",
                        "من أبرز المشاريع الحديثة تجهيز البنية التقنية لافتتاح مركز اللبيب الصحي، وهندسة أنظمة مجمّع الخدمات الحكومي في مسيمير — نشرٌ فعليٌّ يدعم التوسّع المتنامي للقطاع المؤسسي في قطر.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "Requirements & Analysis", Body = "In-depth assessment of organisational needs, infrastructure audits and tailored solution architecture." },
                        new() { Title = "Custom Software Development", Body = "Bespoke application engineering and systems integration across public and private platforms." },
                        new() { Title = "Implementation & Installation", Body = "Disciplined deployment, programming and configuration delivered to specification." },
                        new() { Title = "Training & Support", Body = "End-user enablement plus full maintenance and managed support for the long term." },
                    },
                    Ar = new()
                    {
                        new() { Title = "المتطلبات والتحليل", Body = "تقييمٌ معمّق لاحتياجات المؤسسة ومراجعةٌ للبنية التحتية وتصميمُ حلولٍ مُخصّصة." },
                        new() { Title = "تطوير برمجيات مُخصّصة", Body = "هندسة تطبيقاتٍ مُخصّصة وتكاملُ أنظمة عبر المنصّات الحكومية والخاصة." },
                        new() { Title = "التنفيذ والتركيب", Body = "نشرٌ منضبط وبرمجةٌ وتهيئةٌ تُسلَّم وفق المواصفات." },
                        new() { Title = "التدريب والدعم", Body = "تأهيلُ المستخدمين إضافةً إلى صيانةٍ ودعمٍ مُدار على المدى الطويل." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "End-to-End", Label = "Solution Lifecycle" },
                        new() { Value = "Gov + Private", Label = "Sectors Served" },
                        new() { Value = "24/7", Label = "Support & Maintenance" },
                    },
                    Ar = new()
                    {
                        new() { Value = "متكامل", Label = "دورة الحلول" },
                        new() { Value = "حكومي وخاص", Label = "القطاعات المخدومة" },
                        new() { Value = "٢٤/٧", Label = "الدعم والصيانة" },
                    },
                },
                Website = "https://www.floratechnology.net",
                HasProfile = true,
                Contact = new()
                {
                    Address = new() { En = "Flora Building, Rawdat Al Khail St, Al Helal, Bldg No 127, Street 330, Doha, Qatar. P.O. Box: 16118.", Ar = "مبنى فلورا، شارع روضة الخيل، الهلال، مبنى رقم ١٢٧، شارع ٣٣٠، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44810674",
                    Fax = "(+974) 44314133",
                    Email = "info@floragroup.net",
                },
            },
            new()
            {
                Slug = "flora-security",
                Icon = "ShieldCheck",
                Name = new() { En = "Flora Security Systems", Ar = "فلورا للأنظمة الأمنية" },
                Tag = new() { En = "Certified Security Infrastructure", Ar = "بنية أمنية معتمدة" },
                Short = new()
                {
                    En = "An MOI-certified specialist in access control, video surveillance and physical risk mitigation, registered in 2013.",
                    Ar = "متخصّصة معتمدة من وزارة الداخلية في أنظمة التحكّم بالوصول والمراقبة بالفيديو والحدّ من المخاطر المادية، مُسجّلة عام ٢٠١٣.",
                },
                Established = new() { En = "2013", Ar = "٢٠١٣" },
                Visual = new() { From = "#290a13", To = "#222d3f", Glow = "#b8893e" },
                Image = "/images/flora-security-cover.jpg",
                Logo = "/images/flora-security-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Security has become a necessity across every sector worldwide. With rising risks of theft, break-ins, accidents, vandalism, sabotage and other physical hazards, protecting people and property — and controlling the flow of people — is now essential to business continuity and profitability.",
                        "Conscious of that need, Flora Group officially registered Flora Security Systems in 2013 as a Qatari, MOI-certified security specialist. With access control, video surveillance and other innovative products — delivered with the same service and quality that has defined Flora Group since 1993 — the company helps clients meet a fast-growing need for protection and consistently exceed expectations in this sensitive field.",
                        "We offer needs-focused security solutions that deliver the greatest possible benefit in terms of safety, organisational efficiency and convenience — helping clients protect the things that matter most.",
                    },
                    Ar = new()
                    {
                        "أصبح الأمن ضرورةً في كلّ القطاعات حول العالم. ومع تزايد مخاطر السرقة والاقتحام والحوادث والتخريب وغيرها من الأخطار المادية، باتت حماية الأشخاص والممتلكات — وضبط حركة الأفراد — أمراً جوهرياً لاستمرارية الأعمال وربحيتها.",
                        "إدراكاً لتلك الحاجة، سجّلت مجموعة فلورا رسمياً شركة فلورا للأنظمة الأمنية عام ٢٠١٣ كشركةٍ قطرية متخصّصة ومعتمدة من وزارة الداخلية. وبأنظمة التحكّم بالوصول والمراقبة بالفيديو ومنتجاتٍ مبتكرة أخرى — تُقدَّم بنفس الخدمة والجودة التي ميّزت مجموعة فلورا منذ ١٩٩٣ — تساعد الشركة عملاءها على تلبية حاجةٍ متنامية للحماية وتتجاوز توقّعاتهم في هذا المجال الحسّاس.",
                        "نقدّم حلولاً أمنية تركّز على الاحتياجات وتحقّق أكبر فائدةٍ ممكنة من حيث السلامة والكفاءة التنظيمية والراحة — لمساعدة العملاء على حماية ما يهمّهم أكثر.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "MOI Certified", Body = "Official Ministry of Interior certification ensuring deployments meet Qatar's most rigorous standards." },
                        new() { Title = "Advanced Access Control", Body = "Biometric, card-based and multi-factor systems for high-security commercial and government sites." },
                        new() { Title = "Video Surveillance", Body = "Enterprise-grade CCTV, intelligent analytics and centralised command for total situational awareness." },
                        new() { Title = "Physical Risk Mitigation", Body = "Threat assessment, perimeter protection and risk management across critical assets." },
                    },
                    Ar = new()
                    {
                        new() { Title = "معتمدة من الداخلية", Body = "اعتمادٌ رسمي من وزارة الداخلية يضمن مطابقة التطبيقات لأكثر المعايير صرامةً في قطر." },
                        new() { Title = "تحكّم متقدّم بالوصول", Body = "أنظمة بيومترية وبطاقات وتحقّق متعدّد العوامل للمواقع التجارية والحكومية عالية الأمان." },
                        new() { Title = "المراقبة بالفيديو", Body = "كاميرات مراقبة احترافية وتحليلاتٌ ذكية وقيادةٌ مركزية لوعيٍ شاملٍ بالموقف." },
                        new() { Title = "الحدّ من المخاطر المادية", Body = "تقييمٌ للتهديدات وحمايةٌ للمحيط وإدارةٌ للمخاطر عبر الأصول الحيوية." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "2013", Label = "Registered" },
                        new() { Value = "MOI", Label = "Certified" },
                        new() { Value = "360°", Label = "Protection Coverage" },
                    },
                    Ar = new()
                    {
                        new() { Value = "٢٠١٣", Label = "التسجيل" },
                        new() { Value = "الداخلية", Label = "معتمدة" },
                        new() { Value = "°٣٦٠", Label = "تغطية الحماية" },
                    },
                },
                Website = "https://www.florasecurity.net",
                HasProfile = true,
                Contact = new()
                {
                    Address = new() { En = "Flora Building, Rawdat Al Khail St, Al Helal, Bldg No 127, Doha, Qatar. P.O. Box: 16118.", Ar = "مبنى فلورا، شارع روضة الخيل، الهلال، مبنى رقم ١٢٧، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44810674",
                    Fax = "(+974) 44314133",
                    Email = "info@floragroup.net",
                },
            },
            new()
            {
                Slug = "sibonne",
                Icon = "Sprout",
                Name = new() { En = "Sibonne", Ar = "سيبون" },
                Tag = new() { En = "International Floral Supply", Ar = "توريد الزهور الدولي" },
                Short = new()
                {
                    En = "Fresh cut flowers, plants and floral products, sourced through strategic partnerships across 26+ countries.",
                    Ar = "زهورٌ طازجة ونباتاتٌ ومنتجاتٌ زهرية، تُورَّد عبر شراكاتٍ استراتيجية في أكثر من ٢٦ دولة.",
                },
                Established = new() { En = "Group Company", Ar = "شركة المجموعة" },
                Visual = new() { From = "#4a0e17", To = "#7a1f3a", Glow = "#d98c6a" },
                Image = "/images/sibonne-cover.jpg",
                Logo = "/images/sibonne-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Si Bonne was established in Qatar as part of the Flora Group, specialising in fresh cut flowers, plants and floral products. Its facilities deliver a high level of service built on deep commercial experience and knowledge.",
                        "Since its establishment, Si Bonne has achieved steady growth and broadened its network and strategic partnerships across more than 26 countries — including Holland, Italy, Ecuador, Colombia, South Africa, Kenya, Ethiopia, Egypt, Saudi Arabia, Jordan, New Zealand, China, Thailand, India and Taiwan, among others.",
                    },
                    Ar = new()
                    {
                        "تأسّست سيبون في قطر كجزءٍ من مجموعة فلورا، متخصّصةً في الزهور الطازجة والنباتات والمنتجات الزهرية. وتقدّم منشآتها مستوى خدمةٍ عالياً مبنياً على خبرةٍ تجارية ومعرفةٍ عميقة.",
                        "منذ تأسيسها، حقّقت سيبون نمواً مطّرداً ووسّعت شبكتها وشراكاتها الاستراتيجية عبر أكثر من ٢٦ دولة — من بينها هولندا وإيطاليا والإكوادور وكولومبيا وجنوب أفريقيا وكينيا وإثيوبيا ومصر والسعودية والأردن ونيوزيلندا والصين وتايلاند والهند وتايوان وغيرها.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "Global Sourcing Network", Body = "Strategic partnerships spanning more than 26 countries across four continents." },
                        new() { Title = "Fresh Cut Flowers", Body = "A continuously replenished supply of premium fresh flowers, plants and floral products." },
                        new() { Title = "Commercial Expertise", Body = "Service grounded in years of floral trade experience and market knowledge." },
                        new() { Title = "Steady Growth", Body = "A consistently expanding network and footprint since its founding." },
                    },
                    Ar = new()
                    {
                        new() { Title = "شبكة توريدٍ عالمية", Body = "شراكاتٌ استراتيجية تمتدّ عبر أكثر من ٢٦ دولة في أربع قارّات." },
                        new() { Title = "زهورٌ طازجة", Body = "إمدادٌ متجدّد باستمرار من الزهور الطازجة الفاخرة والنباتات والمنتجات الزهرية." },
                        new() { Title = "خبرة تجارية", Body = "خدمةٌ راسخة في سنواتٍ من خبرة تجارة الزهور ومعرفة السوق." },
                        new() { Title = "نموٌّ مطّرد", Body = "شبكةٌ وحضورٌ في توسّعٍ مستمر منذ التأسيس." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "26+", Label = "Partner Countries" },
                        new() { Value = "4", Label = "Continents Sourced" },
                        new() { Value = "Fresh", Label = "Daily Supply" },
                    },
                    Ar = new()
                    {
                        new() { Value = "+٢٦", Label = "دول شريكة" },
                        new() { Value = "٤", Label = "قارّات التوريد" },
                        new() { Value = "طازج", Label = "إمداد يومي" },
                    },
                },
                Website = "https://www.sibonneqatar.net",
                HasProfile = false,
                Contact = new()
                {
                    Address = new() { En = "Old Airport, near Lulu Hypermarket, Ahmad Bin Hanbal St, Building No. 47, Doha, Qatar. P.O. Box: 16118.", Ar = "المطار القديم، قرب لولو هايبرماركت، شارع أحمد بن حنبل، مبنى رقم ٤٧، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44607703",
                    Fax = "(+974) 44504480",
                    Email = "flora@qatar.net.qa",
                },
                Clients = new()
                {
                    En = new()
                    {
                        "The St. Regis Doha",
                        "The Ritz-Carlton, Doha",
                        "Commercial Bank",
                        "Bateel",
                        "Holiday Villa",
                        "The Diplomatic Club",
                    },
                    Ar = new()
                    {
                        "The St. Regis Doha",
                        "The Ritz-Carlton, Doha",
                        "Commercial Bank",
                        "Bateel",
                        "Holiday Villa",
                        "The Diplomatic Club",
                    },
                },
            },
            new()
            {
                Slug = "flora-flowers",
                Icon = "Flower2",
                Name = new() { En = "Flora Flowers", Ar = "فلورا للزهور" },
                Tag = new() { En = "Online Flowers & Gifts", Ar = "زهور وهدايا عبر الإنترنت" },
                Short = new()
                {
                    En = "One of Qatar's most dependable floral and gift delivery stores — we don't just deliver products, we deliver value.",
                    Ar = "من أكثر متاجر الزهور والهدايا موثوقيةً في قطر — لا نوصّل منتجاتٍ فحسب، بل نوصّل قيمة.",
                },
                Established = new() { En = "Group Company", Ar = "شركة المجموعة" },
                Visual = new() { From = "#6a132a", To = "#a33a5a", Glow = "#e8b07a" },
                Image = "/images/flora-flowers-cover.jpg",
                Logo = "/images/flora-flowers-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Flora Flowers has emerged as one of the most successful, reliable and dependable flower stores in Qatar, delivering floral and gift items throughout the country. We believe our work carries real significance in people's lives — we don't merely deliver flowers, we deliver happiness; we don't deliver products only, we deliver value.",
                        "The sensitivity of our work demands high ethical standards, punctuality, strict adherence to schedules and sheer professionalism. Today we have won our customers' hearts and become one of the leading florists in Qatar — thanks to our loyal customers, their word-of-mouth and their continued patronage.",
                    },
                    Ar = new()
                    {
                        "برزت فلورا للزهور كواحدةٍ من أنجح متاجر الزهور وأكثرها موثوقيةً واعتماديةً في قطر، توصّل الزهور والهدايا في جميع أنحاء البلاد. نؤمن أنّ لعملنا أثراً حقيقياً في حياة الناس — فنحن لا نوصّل الزهور فحسب، بل نوصّل السعادة؛ ولا نوصّل منتجاتٍ فقط، بل نوصّل قيمة.",
                        "تتطلّب حساسية عملنا معايير أخلاقية عالية والتزاماً بالمواعيد وانضباطاً صارماً بالجداول واحترافيةً تامّة. واليوم كسبنا قلوب عملائنا وأصبحنا من أبرز محلّات الزهور في قطر — بفضل عملائنا الأوفياء وتوصياتهم وثقتهم المستمرة.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "Same-Day Delivery", Body = "Reliable floral and gift delivery across Qatar, on schedule and with care." },
                        new() { Title = "Curated Arrangements", Body = "Thoughtfully designed bouquets and gifts for every occasion and sentiment." },
                        new() { Title = "Trusted Reputation", Body = "One of the leading florists in Qatar, grown largely through word of mouth." },
                        new() { Title = "Service First", Body = "Punctuality, ethics and professionalism at the heart of every order." },
                    },
                    Ar = new()
                    {
                        new() { Title = "توصيلٌ في اليوم نفسه", Body = "توصيلٌ موثوق للزهور والهدايا في أنحاء قطر، في الموعد وبعناية." },
                        new() { Title = "تنسيقاتٌ مُختارة", Body = "باقاتٌ وهدايا مصمّمة بعناية لكلّ مناسبةٍ وشعور." },
                        new() { Title = "سمعةٌ موثوقة", Body = "من أبرز محلّات الزهور في قطر، نمت أساساً عبر التوصية الشفهية." },
                        new() { Title = "الخدمة أولاً", Body = "الالتزام بالمواعيد والأخلاق والاحترافية في صميم كلّ طلب." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "Qatar-wide", Label = "Delivery Reach" },
                        new() { Value = "Same-Day", Label = "Fulfilment" },
                        new() { Value = "5★", Label = "Customer Loyalty" },
                    },
                    Ar = new()
                    {
                        new() { Value = "كل قطر", Label = "نطاق التوصيل" },
                        new() { Value = "نفس اليوم", Label = "التنفيذ" },
                        new() { Value = "★٥", Label = "ولاء العملاء" },
                    },
                },
                Website = "https://www.flora.qa",
                HasProfile = false,
                Contact = new()
                {
                    Address = new() { En = "Old Airport, near Lulu Hypermarket, Ahmad Bin Hanbal St, Building No. 47, Doha, Qatar. P.O. Box: 16118.", Ar = "المطار القديم، قرب لولو هايبرماركت، شارع أحمد بن حنبل، مبنى رقم ٤٧، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44607703",
                    Fax = "(+974) 44504480",
                    Email = "flora@qatar.net.qa",
                },
                WebsiteLabel = new() { En = "Shop the Online Store", Ar = "تسوّق من المتجر الإلكتروني" },
            },
            new()
            {
                Slug = "royal-stone",
                Icon = "Building2",
                Name = new() { En = "Royal Stone Trading & Contracting", Ar = "رويال ستون للتجارة والمقاولات" },
                Tag = new() { En = "Civil Construction & Materials", Ar = "الإنشاءات المدنية والمواد" },
                Short = new()
                {
                    En = "A well-established trading and contracting company, leading general contracting and material supply in Qatar since 2014.",
                    Ar = "شركة تجارةٍ ومقاولاتٍ راسخة، رائدة في المقاولات العامة وتوريد المواد في قطر منذ عام ٢٠١٤.",
                },
                Established = new() { En = "2014", Ar = "٢٠١٤" },
                Visual = new() { From = "#3a1410", To = "#6a3a22", Glow = "#d9a441" },
                Image = "/images/royal-stone-cover.jpg",
                Logo = "/images/royal-stone-logo.png",
                Paragraphs = new()
                {
                    En = new()
                    {
                        "Royal Stone Trading and Contracting is a well-established company formed in early 2014, with strong domain expertise and professional experience. Today it operates across four specialised divisions — General Contracting, Trading, MEP and Steel & Aluminium — focused on excelling as a leading construction partner in the State of Qatar.",
                        "What sets Royal Stone apart is its partnership with Flora Group — one of Qatar's leading and most reputed organisations, with a strong presence in the country built over decades.",
                        "As a subsidiary of Flora Group, Royal Stone integrates the Group's decades of experience, market relationships, facilities, standards, professionalism and reputation with a construction team carrying more than a decade of domain expertise — delivering projects on time, to quality, and to the highest levels of client satisfaction.",
                    },
                    Ar = new()
                    {
                        "رويال ستون للتجارة والمقاولات شركةٌ راسخة تأسّست مطلع عام ٢٠١٤، تتمتّع بخبرةٍ متخصّصة عميقة. وتعمل اليوم عبر أربعة أقسامٍ متخصّصة — المقاولات العامة، والتجارة، والأنظمة الكهروميكانيكية، والحديد والألمنيوم — سعياً للتميّز كشريكٍ رائدٍ في قطاع الإنشاءات بدولة قطر.",
                        "ما يميّز رويال ستون هو شراكتها مع مجموعة فلورا — إحدى أعرق المؤسسات القطرية وأكثرها سمعةً، بحضورٍ قويّ في البلاد بُني على مدى عقود.",
                        "بصفتها شركةً تابعة لمجموعة فلورا، تدمج رويال ستون عقوداً من خبرة المجموعة وعلاقاتها السوقية ومنشآتها ومعاييرها واحترافيتها وسمعتها مع فريق بناءٍ يحمل أكثر من عقدٍ من الخبرة المتخصّصة — لتسليم المشاريع في موعدها وبالجودة المطلوبة وأعلى مستويات رضا العملاء.",
                    },
                },
                Highlights = new()
                {
                    En = new()
                    {
                        new() { Title = "General Contracting", Body = "Turnkey civil construction — buildings, roads and infrastructure finishing works for commercial, industrial and institutional clients." },
                        new() { Title = "Trading Division", Body = "Supply of building materials, finishing items and infrastructure products from trusted regional and international manufacturers." },
                        new() { Title = "MEP Systems", Body = "Mechanical, electrical and plumbing works delivered end-to-end alongside the main contracting scope." },
                        new() { Title = "Steel & Aluminium", Body = "Fabrication and installation of structural steel and aluminium works for façades, structures and finishing." },
                    },
                    Ar = new()
                    {
                        new() { Title = "المقاولات العامة", Body = "مقاولات إنشاءات مدنية متكاملة — تسليم مفتاح — تشمل المباني والطرق والبنية التحتية وأعمال التشطيبات للقطاعات التجارية والصناعية والمؤسسية." },
                        new() { Title = "قسم التجارة", Body = "توريد مواد البناء ومستلزمات التشطيبات ومنتجات البنية التحتية من مصنّعين إقليميين ودوليين موثوقين." },
                        new() { Title = "الأنظمة الكهروميكانيكية", Body = "أعمال ميكانيكية وكهربائية وصحية متكاملة تُنفَّذ إلى جانب نطاق المقاولات الرئيسي." },
                        new() { Title = "الحديد والألمنيوم", Body = "تصنيع وتركيب أعمال الحديد الإنشائي والألمنيوم للواجهات والهياكل والتشطيبات." },
                    },
                },
                Metrics = new()
                {
                    En = new()
                    {
                        new() { Value = "2014", Label = "Founded" },
                        new() { Value = "11+ yrs", Label = "Team Expertise" },
                        new() { Value = "On-Time", Label = "Project Delivery" },
                    },
                    Ar = new()
                    {
                        new() { Value = "٢٠١٤", Label = "التأسيس" },
                        new() { Value = "+١١ سنة", Label = "خبرة الفريق" },
                        new() { Value = "في الموعد", Label = "تسليم المشاريع" },
                    },
                },
                Website = "https://www.royal-stone.net",
                HasProfile = true,
                Contact = new()
                {
                    Address = new() { En = "Flora Building, Rawdat Al Khail St, Al Helal, Bldg No 127, Street 330, Zone 41, Nuaija (Al Hilal West), Doha, Qatar. P.O. Box: 16118.", Ar = "مبنى فلورا، شارع روضة الخيل، الهلال، مبنى رقم ١٢٧، شارع ٣٣٠، المنطقة ٤١، النعيجة (الهلال الغربي)، الدوحة، قطر. ص.ب: ١٦١١٨." },
                    Phone = "(+974) 44810674",
                    Fax = "(+974) 44314133",
                    Email = "royal@floragroup.net",
                },
            },
        };
    }
}

/// <summary>Localized string pair (English and Arabic).</summary>
public class LocalizedStringDto
{
    [System.Text.Json.Serialization.JsonPropertyName("en")]
    public string En { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("ar")]
    public string Ar { get; set; } = string.Empty;
}

/// <summary>Localized array container for T values.</summary>
public class LocalizedArrayDto<T>
{
    [System.Text.Json.Serialization.JsonPropertyName("en")]
    public List<T> En { get; set; } = new();

    [System.Text.Json.Serialization.JsonPropertyName("ar")]
    public List<T> Ar { get; set; } = new();
}

/// <summary>Visual branding: gradient colors.</summary>
public class VisualDto
{
    public string From { get; set; } = "#5a1020";
    public string To { get; set; } = "#8a2a17";
    public string Glow { get; set; } = "#e0a23a";
}

/// <summary>A highlight key-value pair.</summary>
public class HighlightDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

/// <summary>A metric key-value pair.</summary>
public class MetricDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

/// <summary>Subsidiary contact information.</summary>
public class SubsidiaryContactDto
{
    public LocalizedStringDto Address { get; set; } = new();
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
}
