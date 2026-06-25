namespace EduProgress;

public static class AppTheme
{
    // ── Core palette ──────────────────────────────────────────────────────────
    public static readonly Color Primary    = Color.FromArgb(44,  62,  80);   // #2C3E50
    public static readonly Color Accent     = Color.FromArgb(52,  152, 219);  // #3498DB
    public static readonly Color Success    = Color.FromArgb(39,  174, 96);   // #27AE60
    public static readonly Color Warning    = Color.FromArgb(243, 156, 18);   // #F39C12
    public static readonly Color Danger     = Color.FromArgb(231, 76,  60);   // #E74C3C
    public static readonly Color Purple     = Color.FromArgb(142, 68,  173);  // #8E44AD

    // ── Backgrounds ───────────────────────────────────────────────────────────
    public static readonly Color Background = Color.FromArgb(245, 246, 250);  // #F5F6FA
    public static readonly Color White      = Color.White;

    // ── Sidebar ───────────────────────────────────────────────────────────────
    public static readonly Color SidebarDark   = Color.FromArgb(35, 50, 65);
    public static readonly Color SidebarActive = Color.FromArgb(52, 73, 94);
    public static readonly Color SidebarText   = Color.FromArgb(189, 195, 199);

    // ── Text ──────────────────────────────────────────────────────────────────
    public static readonly Color TextPrimary = Color.FromArgb(44, 62, 80);
    public static readonly Color TextMuted   = Color.FromArgb(127, 140, 141);
    public static readonly Color TextLight   = Color.FromArgb(189, 195, 199);

    // ── UI lines ──────────────────────────────────────────────────────────────
    public static readonly Color Border      = Color.FromArgb(220, 226, 235);
    public static readonly Color BorderLight = Color.FromArgb(236, 240, 245);

    // ── Category colours ──────────────────────────────────────────────────────
    public static readonly Color CatCourse   = Color.FromArgb(52,  152, 219);
    public static readonly Color CatReminder = Color.FromArgb(243, 156, 18);
    public static readonly Color CatSystem   = Color.FromArgb(142, 68,  173);

    // ── Fonts ─────────────────────────────────────────────────────────────────
    public static Font F(float sz, FontStyle st = FontStyle.Regular)
        => new Font("Segoe UI", sz, st);

    public static readonly Font FontTitle      = new Font("Segoe UI", 24f, FontStyle.Bold);
    public static readonly Font FontH1         = new Font("Segoe UI", 16f, FontStyle.Bold);
    public static readonly Font FontH2         = new Font("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FontH3         = new Font("Segoe UI", 12f, FontStyle.Bold);
    public static readonly Font FontBody       = new Font("Segoe UI", 12f);
    public static readonly Font FontBodyBold   = new Font("Segoe UI", 12f, FontStyle.Bold);
    public static readonly Font FontSmall      = new Font("Segoe UI", 10f);
    public static readonly Font FontSmallBold  = new Font("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font FontNav        = new Font("Segoe UI", 12f);
    public static readonly Font FontStat       = new Font("Segoe UI", 28f, FontStyle.Bold);
    public static readonly Font FontLogo       = new Font("Segoe UI", 16f, FontStyle.Bold);
    public static readonly Font FontInput      = new Font("Segoe UI", 12f);
}
