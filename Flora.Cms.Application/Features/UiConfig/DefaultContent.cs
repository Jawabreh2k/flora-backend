namespace Flora.Cms.Application.Features.UiConfig;

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
          "description": "Two of the Group's most advanced businesses — engineered for Qatar's most demanding public and private institutions.",
          "technologyImage": "/images/flora-technology-cover.jpg",
          "technologyLogo": "/images/flora-technology-logo.png",
          "securityImage": "/images/flora-security-cover.jpg",
          "securityLogo": "/images/flora-security-logo.png"
        },
        "nav": {
          "home": "Home",
          "legacy": "Our Legacy",
          "subsidiaries": "Subsidiaries",
          "spotlight": "Spotlight",
          "careers": "Careers",
          "contact": "Contact",
          "getInTouch": "Get in Touch",
          "openMenu": "Open menu",
          "closeMenu": "Close menu",
          "languageName": "العربية",
          "switchTo": "Switch to Arabic"
        },
        "stats": {
          "established": "Established History",
          "verticals": "Strategic Verticals",
          "certified": "Certified Security & Governance",
          "countries": "Global Partner Countries",
          "establishedValue": "1993",
          "verticalsValue": "6",
          "certifiedValue": "MOI",
          "countriesValue": "26+"
        },
        "pages": {
          "contactMetaTitle": "Contact Us — Flora Group W.L.L.",
          "contactMetaDescription": "Get in touch with Flora Group W.L.L. in Doha, Qatar. Head office, Flora Flowers & Sibonne offices, phone, fax, and email.",
          "careersMetaTitle": "Careers — Flora Group W.L.L.",
          "careersMetaDescription": "Join Flora Group W.L.L. in Doha, Qatar. Explore open roles across technology, security, trading, floral supply, and construction."
        },
        "footer": {
          "careersTitle": "Build your career with Qatar's trusted conglomerate.",
          "careersBody": "Join a multidisciplinary group shaping technology, security, trade and infrastructure across the region.",
          "exploreCareers": "Explore Careers",
          "about": "A prestigious Qatari conglomerate established in 1993, spanning commodities, technology, certified security, international floral supply and civil construction.",
          "address": "Flora Building, Rawdat Al Khail St, Bldg 127, Street 330, Doha, Qatar · P.O. Box 16118",
          "phone": "+97444810674",
          "phoneDisplay": "+974 4481 0674",
          "email": "info@floragroup.net",
          "mapsUrl": "https://maps.google.com/?q=Flora+Building+Rawdat+Al+Khail+Doha+Qatar",
          "companyName": "Flora Group W.L.L.",
          "subsidiariesHeading": "Subsidiaries",
          "headOfficeHeading": "Head Office",
          "openInMaps": "Open in Maps",
          "globalInquiries": "Global Inquiries",
          "rightsText": "All rights reserved.",
          "emailPlaceholder": "Your email",
          "subscribed": "Thank you — we'll be in touch.",
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
          "sendHelp": "Fill out the form and a member of our team will respond shortly.",
          "ourOffices": "Our Offices",
          "phone": "Phone",
          "fax": "Fax",
          "headOfficeAddress": "Flora Building, Rawdat Al Khail St, Al Helal, Bldg No 127, Street no. 330, Zone no. 41, Nuaija (Al Hilal West), Doha, Qatar. P.O. Box: 16118.",
          "headOfficePhone": "(+974) 44810674",
          "headOfficeFax": "(+974) 44314133",
          "headOfficeEmail": "info@floragroup.net",
          "flowersOfficeAddress": "Old Airport, Near Lulu Hypermarket, Ahmad Bin Hanbal St, Building No. 47, Doha, Qatar. P.O. Box: 16118.",
          "flowersOfficePhone": "(+974) 44607703",
          "flowersOfficeFax": "(+974) 44504480",
          "flowersOfficeEmail": "flora@qatar.net.qa",
          "mapLocation": "Flora Group Co, Rawdat Al Khail St, Doha, Qatar",
          "mapEmbedUrl": "https://www.google.com/maps?q=Flora+Group+Co+Rawdat+Al+Khail+Doha+Qatar&output=embed",
          "mapTitle": "Flora Group location in Doha, Qatar",
          "formRecipientEmail": "info@floragroup.net",
          "fullName": "Full Name",
          "email": "Email",
          "company": "Company",
          "subject": "Subject",
          "message": "Message",
          "messagePlaceholder": "How can we help you?",
          "send": "Send Message",
          "formSuccessTitle": "Thank you",
          "formSuccessBody": "Your message has been received. We'll be in touch soon.",
          "sendAnother": "Send another message",
          "formErrorMessage": "Something went wrong. Please try again.",
          "required": "required"
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
        },
        "careersStats": {
          "years": "Years of excellence",
          "verticals": "Strategic verticals",
          "partners": "Global partners",
          "team": "Team members",
          "yearsValue": "30+",
          "verticalsValue": "6",
          "partnersValue": "50+",
          "teamValue": "70+"
        },
        "careersCultureValues": {
          "value1Title": "Purposeful growth",
          "value1Body": "Develop across six verticals with mentorship, training, and clear paths to leadership.",
          "value2Title": "Uncompromising standards",
          "value2Body": "MOI-certified security, international quality benchmarks, and ethics at every level.",
          "value3Title": "Regional impact",
          "value3Body": "Contribute to projects that shape Qatar's public and private sectors for decades.",
          "value4Title": "People first",
          "value4Body": "A collaborative culture that rewards initiative, accountability, and long-term thinking."
        },
        "careersUi": {
          "filterLocation": "Location type",
          "filterJobType": "Job type",
          "searchPlaceholder": "Search roles or companies",
          "showingRoles": "Showing {shown} of {total} roles",
          "previous": "Previous",
          "next": "Next",
          "loading": "Loading open roles…",
          "loadError": "Failed to load jobs"
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
          "description": "اثنتان من أكثر شركات المجموعة تقدّماً — مصمّمتان لأكثر المؤسسات القطرية الحكومية والخاصة تطلّباً.",
          "technologyImage": "/images/flora-technology-cover.jpg",
          "technologyLogo": "/images/flora-technology-logo.png",
          "securityImage": "/images/flora-security-cover.jpg",
          "securityLogo": "/images/flora-security-logo.png"
        },
        "nav": {
          "home": "الرئيسية",
          "legacy": "إرثنا",
          "subsidiaries": "الشركات التابعة",
          "spotlight": "الأضواء",
          "careers": "الوظائف",
          "contact": "تواصل معنا",
          "getInTouch": "تواصل معنا",
          "openMenu": "فتح القائمة",
          "closeMenu": "إغلاق القائمة",
          "languageName": "English",
          "switchTo": "التبديل إلى الإنجليزية"
        },
        "stats": {
          "established": "تاريخ التأسيس",
          "verticals": "قطاعات استراتيجية",
          "certified": "أمن وحوكمة معتمدة",
          "countries": "دول شريكة حول العالم",
          "establishedValue": "1993",
          "verticalsValue": "6",
          "certifiedValue": "الداخلية",
          "countriesValue": "26+"
        },
        "pages": {
          "contactMetaTitle": "تواصل معنا — مجموعة فلورا ذ.م.م",
          "contactMetaDescription": "تواصل مع مجموعة فلورا في الدوحة، قطر. المكتب الرئيسي، مكاتب فلورا للزهور وسيبون، الهاتف والفاكس والبريد الإلكتروني.",
          "careersMetaTitle": "الوظائف — مجموعة فلورا ذ.م.م",
          "careersMetaDescription": "انضمّ إلى مجموعة فلورا في الدوحة، قطر. استكشف الوظائف الشاغرة في التقنية والأمن والتجارة وتوريد الزهور والمقاولات."
        },
        "footer": {
          "careersTitle": "ابنِ مسيرتك المهنية مع المجموعة القطرية الموثوقة.",
          "careersBody": "انضمّ إلى مجموعةٍ متعدّدة التخصّصات تُشكّل التقنية والأمن والتجارة والبنية التحتية في المنطقة.",
          "exploreCareers": "استكشف الوظائف",
          "about": "مجموعةٌ قطرية مرموقة تأسّست عام ١٩٩٣، تمتدّ عبر السلع والتقنية والأمن المعتمد وتوريد الزهور الدولي والمقاولات المدنية.",
          "address": "مبنى فلورا، شارع روضة الخيل، مبنى ١٢٧، شارع ٣٣٠، الدوحة، قطر · ص.ب ١٦١١٨",
          "phone": "+97444810674",
          "phoneDisplay": "+974 4481 0674",
          "email": "info@floragroup.net",
          "mapsUrl": "https://maps.google.com/?q=Flora+Building+Rawdat+Al+Khail+Doha+Qatar",
          "companyName": "مجموعة فلورا ذ.م.م",
          "subsidiariesHeading": "الشركات التابعة",
          "headOfficeHeading": "المكتب الرئيسي",
          "openInMaps": "افتح في الخرائط",
          "globalInquiries": "الاستفسارات الدولية",
          "rightsText": "جميع الحقوق محفوظة.",
          "emailPlaceholder": "بريدك الإلكتروني",
          "subscribed": "شكراً لك — سنتواصل معك قريباً.",
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
          "sendHelp": "املأ النموذج وسيردّ عليك أحد أعضاء فريقنا قريباً.",
          "ourOffices": "مكاتبنا",
          "phone": "الهاتف",
          "fax": "الفاكس",
          "headOfficeAddress": "مبنى فلورا، شارع روضة الخيل، الهلال، مبنى رقم ١٢٧، شارع ٣٣٠، المنطقة ٤١، النعيجة (الهلال الغربي)، الدوحة، قطر. ص.ب: ١٦١١٨.",
          "headOfficePhone": "(+٩٧٤) ٤٤٨١٠٦٧٤",
          "headOfficeFax": "(+٩٧٤) ٤٤٣١٤١٣٣",
          "headOfficeEmail": "info@floragroup.net",
          "flowersOfficeAddress": "المطار القديم، قرب لولو هايبرماركت، شارع أحمد بن حنبل، مبنى رقم ٤٧، الدوحة، قطر. ص.ب: ١٦١١٨.",
          "flowersOfficePhone": "(+٩٧٤) ٤٤٦٠٧٧٠٣",
          "flowersOfficeFax": "(+٩٧٤) ٤٤٥٠٤٤٨٠",
          "flowersOfficeEmail": "flora@qatar.net.qa",
          "mapLocation": "Flora Group Co, Rawdat Al Khail St, Doha, Qatar",
          "mapEmbedUrl": "https://www.google.com/maps?q=Flora+Group+Co+Rawdat+Al+Khail+Doha+Qatar&output=embed",
          "mapTitle": "موقع مجموعة فلورا في الدوحة، قطر",
          "formRecipientEmail": "info@floragroup.net",
          "fullName": "الاسم الكامل",
          "email": "البريد الإلكتروني",
          "company": "الشركة",
          "subject": "الموضوع",
          "message": "الرسالة",
          "messagePlaceholder": "كيف يمكننا مساعدتك؟",
          "send": "إرسال الرسالة",
          "formSuccessTitle": "شكراً لك",
          "formSuccessBody": "تم استلام رسالتك. سنتواصل معك قريباً.",
          "sendAnother": "إرسال رسالة أخرى",
          "formErrorMessage": "حدث خطأ. يرجى المحاولة مرة أخرى.",
          "required": "مطلوب"
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
        },
        "careersStats": {
          "years": "عاماً من التميّز",
          "verticals": "قطاعات استراتيجية",
          "partners": "شريكاً عالمياً",
          "team": "عضواً في الفريق",
          "yearsValue": "30+",
          "verticalsValue": "6",
          "partnersValue": "50+",
          "teamValue": "70+"
        },
        "careersCultureValues": {
          "value1Title": "نموّ هادف",
          "value1Body": "تطوّر عبر ستّة قطاعات مع إرشاد وتدريب ومسارات واضحة نحو القيادة.",
          "value2Title": "معايير لا تُساوم",
          "value2Body": "أمن معتمد من الداخلية، معايير جودة دولية، وأخلاقيات في كلّ مستوى.",
          "value3Title": "أثر إقليمي",
          "value3Body": "ساهم في مشاريع تُشكّل القطاعين العام والخاص في قطر لعقود.",
          "value4Title": "الإنسان أولاً",
          "value4Body": "ثقافة تعاونية تكافئ المبادرة والمساءلة والتفكير طويل الأمد."
        },
        "careersUi": {
          "filterLocation": "نوع الموقع",
          "filterJobType": "نوع الوظيفة",
          "searchPlaceholder": "ابحث عن وظائف أو شركات",
          "showingRoles": "عرض {shown} من {total} وظيفة",
          "previous": "السابق",
          "next": "التالي",
          "loading": "جارٍ تحميل الوظائف…",
          "loadError": "تعذّر تحميل الوظائف"
        }
      }
    }
    """;
}
