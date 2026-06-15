namespace Flora.Cms.Api.Dtos;

/// <summary>
/// Seed copy for the CMS-managed content overrides. Mirrors the public site's i18n
/// dictionary (lib/i18n/messages.ts) for all editable sections, in both English and
/// Arabic. Stored verbatim into the JSON payload so the Admin Portal is pre-populated
/// with the current site copy on first run.
///
/// Groups are flat (key → string value) per section. The public site's i18n-provider
/// maps them back to the nested Messages structure before deep-merging:
///   footer.*         → t.footer.*
///   contact.*        → t.contact.*
///   careersHero.*    → t.careers.{ eyebrow, title, … }
///   careersCulture.* → t.careers.culture.*
///   careersBenefits.*→ t.careers.benefits.*
///   careersPositions.*→ t.careers.positions.*
///   careersCta.*     → t.careers.cta.*
/// </summary>
public static class DefaultContent
{
    public const string Json = """
    {
      "en": {
        "hero": {
          "badge": "Qatari Conglomerate · Since 1993",
          "titleA": "A legacy of excellence,",
          "titleHighlight": "redefining industries",
          "titleB": "across Qatar.",
          "subtitle": "Since 1993, Flora Group has been a trusted cornerstone of innovation — spanning technology, infrastructure, security, commodities and premium trade.",
          "ctaExplore": "Explore Subsidiaries",
          "ctaProfile": "Corporate Profile",
          "caption": "Doha, Qatar · Established 1993",
          "cardVerticals": "Strategic Verticals",
          "cardYears": "Years of Trust"
        },
        "subsidiaries": {
          "eyebrow": "The Subsidiary Matrix",
          "title": "Six distinct businesses, one standard of excellence.",
          "description": "Each Flora Group company operates as a specialised leader in its field, united by a shared commitment to integrity, quality and long-term partnership. Select any company to explore its full story.",
          "explore": "Explore"
        },
        "spotlight": {
          "eyebrow": "Core Spotlight",
          "title": "A closer look at our flagship divisions.",
          "description": "Two of the Group's most advanced businesses — engineered for Qatar's most demanding public and private institutions."
        },
        "footer": {
          "careersTitle": "Build your career with Qatar's trusted conglomerate.",
          "careersBody": "Join a multidisciplinary group shaping technology, security, trade and infrastructure across the region.",
          "exploreCareers": "Explore Careers",
          "about": "A prestigious Qatari conglomerate established in 1993, spanning commodities, technology, certified security, international floral supply and civil construction.",
          "address": "Flora Building, Rawdat Al Khail St, Bldg 127, Street 330, Doha, Qatar · P.O. Box 16118",
          "newsletter": "Subscribe for corporate updates and partnership opportunities.",
          "tagline": "Doha · Qatar · Established 1993"
        },
        "contact": {
          "eyebrow": "Get In Touch",
          "title": "Let's build something enduring together.",
          "subtitle": "Whether you're exploring a partnership, a project, or a career with Qatar's trusted conglomerate, our team is ready to assist.",
          "headOfficeName": "Flora Group — Head Office",
          "flowersOfficeName": "Flora Flowers & Sibonne",
          "hoursDays": "Sunday – Thursday",
          "hoursTime": "8:00 AM – 6:00 PM (AST)",
          "sendTitle": "Send us a message",
          "sendHelp": "Fill out the form and a member of our team will respond shortly."
        },
        "careersHero": {
          "eyebrow": "Careers",
          "title": "Shape Qatar's future with a legacy brand.",
          "subtitle": "For over three decades, Flora Group has attracted exceptional talent to build technology, secure infrastructure, and premium trade across the region. Your next chapter starts here.",
          "viewRoles": "View open roles",
          "generalApply": "General application"
        },
        "careersCulture": {
          "eyebrow": "Why Flora Group",
          "title": "A place where ambition meets integrity.",
          "description": "We combine the reach of a diversified conglomerate with the ownership culture of specialist companies — giving you room to grow without losing what makes your work meaningful."
        },
        "careersBenefits": {
          "eyebrow": "Life at Flora",
          "title": "Benefits designed for lasting careers."
        },
        "careersPositions": {
          "eyebrow": "Open positions",
          "title": "Current opportunities.",
          "description": "Explore roles across our subsidiaries. Select a position to pre-fill your application — full integration with our HR system is coming soon."
        },
        "careersCta": {
          "title": "Don't see the right role?",
          "body": "Send your CV to our talent team — we're always looking for exceptional people across all verticals.",
          "email": "careers@floragroup.net",
          "button": "Email our HR team"
        }
      },
      "ar": {
        "hero": {
          "badge": "مجموعة قطرية · منذ ١٩٩٣",
          "titleA": "إرثٌ من التميّز،",
          "titleHighlight": "يُعيد تعريف الصناعات",
          "titleB": "في قطر.",
          "subtitle": "منذ عام ١٩٩٣، ظلّت مجموعة فلورا ركيزةً موثوقةً للابتكار — تمتدّ عبر التقنية والبنية التحتية والأمن والسلع والتجارة الراقية.",
          "ctaExplore": "استكشف الشركات",
          "ctaProfile": "الملف التعريفي",
          "caption": "الدوحة، قطر · تأسست عام ١٩٩٣",
          "cardVerticals": "قطاعات استراتيجية",
          "cardYears": "عاماً من الثقة"
        },
        "subsidiaries": {
          "eyebrow": "منظومة الشركات",
          "title": "ستّ شركاتٍ متميّزة، ومعيارٌ واحد للتميّز.",
          "description": "تعمل كلّ شركةٍ من مجموعة فلورا كرائدةٍ متخصّصة في مجالها، تجمعها التزاماتٌ مشتركة بالنزاهة والجودة والشراكة طويلة الأمد. اختر أيّ شركةٍ لاستكشاف قصّتها كاملة.",
          "explore": "استكشف"
        },
        "spotlight": {
          "eyebrow": "تسليط الضوء",
          "title": "نظرةٌ أقرب على أقسامنا الرائدة.",
          "description": "اثنتان من أكثر شركات المجموعة تقدّماً — مصمّمتان لأكثر المؤسسات القطرية الحكومية والخاصة تطلّباً."
        },
        "footer": {
          "careersTitle": "ابنِ مسيرتك المهنية مع المجموعة القطرية الموثوقة.",
          "careersBody": "انضمّ إلى مجموعةٍ متعدّدة التخصّصات تُشكّل التقنية والأمن والتجارة والبنية التحتية في المنطقة.",
          "exploreCareers": "استكشف الوظائف",
          "about": "مجموعةٌ قطرية مرموقة تأسّست عام ١٩٩٣، تمتدّ عبر السلع والتقنية والأمن المعتمد وتوريد الزهور الدولي والمقاولات المدنية.",
          "address": "مبنى فلورا، شارع روضة الخيل، مبنى ١٢٧، شارع ٣٣٠، الدوحة، قطر · ص.ب ١٦١١٨",
          "newsletter": "اشترك لتصلك تحديثات الشركة وفرص الشراكة.",
          "tagline": "الدوحة · قطر · تأسست عام ١٩٩٣"
        },
        "contact": {
          "eyebrow": "تواصل معنا",
          "title": "لنبنِ معاً شيئاً يدوم.",
          "subtitle": "سواء كنت تستكشف شراكةً أو مشروعاً أو مسيرةً مهنية مع المجموعة القطرية الموثوقة، فإنّ فريقنا جاهزٌ لمساعدتك.",
          "headOfficeName": "مجموعة فلورا — المكتب الرئيسي",
          "flowersOfficeName": "فلورا للزهور وسيبون",
          "hoursDays": "الأحد – الخميس",
          "hoursTime": "٨:٠٠ ص – ٦:٠٠ م (بتوقيت قطر)",
          "sendTitle": "أرسل لنا رسالة",
          "sendHelp": "املأ النموذج وسيردّ عليك أحد أعضاء فريقنا قريباً."
        },
        "careersHero": {
          "eyebrow": "الوظائف",
          "title": "اصنع مستقبل قطر مع علامةٍ عريقة.",
          "subtitle": "على مدى أكثر من ثلاثة عقود، جذبت مجموعة فلورا المواهب الاستثنائية لبناء التقنية وتأمين البنية التحتية والتجارة الراقية في المنطقة. فصلك القادم يبدأ هنا.",
          "viewRoles": "عرض الوظائف الشاغرة",
          "generalApply": "طلب عام"
        },
        "careersCulture": {
          "eyebrow": "لماذا مجموعة فلورا",
          "title": "حيث يلتقي الطموح بالنزاهة.",
          "description": "نجمع بين امتداد مجموعةٍ متنوّعة وثقافة الملكية في الشركات المتخصّصة — منحك مساحةً للنمو دون أن تفقد معنى عملك."
        },
        "careersBenefits": {
          "eyebrow": "الحياة في فلورا",
          "title": "مزايا صُمّمت لمساراتٍ مهنيةٍ دائمة."
        },
        "careersPositions": {
          "eyebrow": "الوظائف الشاغرة",
          "title": "الفرص الحالية.",
          "description": "استكشف الأدوار عبر شركاتنا التابعة. اختر وظيفةً لملء طلبك مسبقاً — سيتم ربط النظام الكامل بقسم الموارد البشرية قريباً."
        },
        "careersCta": {
          "title": "لم تجد الدور المناسب؟",
          "body": "أرسل سيرتك الذاتية إلى فريق المواهب — نبحث دائماً عن أشخاصٍ استثنائيين في جميع القطاعات.",
          "email": "careers@floragroup.net",
          "button": "راسل فريق الموارد البشرية"
        }
      }
    }
    """;
}
