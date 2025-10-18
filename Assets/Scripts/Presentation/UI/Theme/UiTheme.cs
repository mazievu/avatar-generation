using UnityEngine;

[CreateAssetMenu(menuName="LifeSim/UI Theme", fileName="UiTheme")]
public class UiTheme : ScriptableObject
{
    // ==== Color tokens (không hardcode màu trong code UI) ====
    [Header("Base")]
    public Color bgApp        = new Color(0.10f, 0.11f, 0.12f, 1f);  // nền Start menu
    public Color bgScreen     = new Color(0.97f, 0.98f, 0.96f, 1f);  // nền Family screen

    [Header("Primary/Secondary")]
    public Color primary      = new Color32(58, 124, 255, 255);      // nút Start
    public Color primaryText  = Color.white;
    public Color secondary    = new Color32(137, 173, 255, 255);     // How to Play
    public Color secondaryText= Color.white;

    [Header("HUD")]
    public Color hudText      = new Color32(41, 44, 51, 255);        // Jan 2, 2024
    public Color pillBg       = new Color32(236, 237, 241, 255);     // nền pill
    public Color pillStroke   = new Color32(206, 207, 213, 255);
    public Color moneyGreen   = new Color32(35, 168, 79, 255);       // ký hiệu $
    public Color incomeGreen  = new Color32(61, 186, 93, 255);       // (+xxx/mo)

    [Header("Story/Settings buttons")]
    public Color storyBg      = new Color32(232, 93, 93, 255);       // nút Story (đỏ)
    public Color settingsBg   = new Color32(66, 180, 171, 255);      // nút Settings (xanh teal)
    public Color chipText     = Color.white;

    [Header("Modal")]
    public Color modalOverlay = new Color(0,0,0,0.55f);
    public Color modalBg      = Color.white;
    public Color modalBorder  = new Color32(255, 202, 54, 255);      // đường viền vàng
    public Color ctaGreen     = new Color32(48, 194, 101, 255);      // nút Excellent
    public Color ctaText      = Color.white;

    [Header("Bottom Icon Bar")]
    public Color bottomBarBg  = new Color32(163, 184, 246, 255);
    public Color iconTint     = Color.white;

    // ==== Typography ====
    [Header("Typography")]
    public int titleXL        = 44;  // Title modal
    public int title          = 36;  // Header
    public int label          = 28;  // nút
    public int body           = 30;  // nội dung

    [Header("Layout")]
    public Vector2 referenceResolution = new Vector2(1080, 1920);
    public float   headerHeight  = 120;
    public float   bottomHeight  = 160;
}