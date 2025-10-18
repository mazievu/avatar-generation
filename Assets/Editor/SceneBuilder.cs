#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using LifeSim.Presentation.UI;
using LifeSim.Presentation.UI.Panels;
using LifeSim.Presentation.UI.Avatar;
using LifeSim.Presentation.UI.Effects;
using LifeSim.Presentation.Services;

// NOTE: Script này giả định bạn đã có các script runtime:
// Bootstrap, GameUIController, ModalManager, ComicPanel, ModalBase,
// SchoolChoiceModal, UniversityChoiceModal, MajorChoiceModal, CareerChoiceModal,
// UnderqualifiedChoiceModal, LoanModal, PromotionModal,
// PathOfLifePanel, BusinessPanel, AssetsPanel, GameLogPanel, FamilyTreePanel,
// SaveSystem (tùy chọn).
// Nếu thiếu script nào, bạn có thể comment phần tạo tương ứng.

public static class RectTransformUtil
{
    public static RectTransform FullStretch(this RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }
    public static RectTransform TopStretch(this RectTransform rt, float height)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.sizeDelta = new Vector2(0, height);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }
    public static RectTransform BottomStretch(this RectTransform rt, float height)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.sizeDelta = new Vector2(0, height);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }
    public static RectTransform MiddleBetween(this RectTransform rt, float top, float bottom)
    {
        // anchors theo phần trăm chiều cao canvas
        // top/bottom là độ cao pixel vùng header/bottomNav
        // cách đơn giản: stretch full rồi cộng offset
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, bottom);
        rt.offsetMax = new Vector2(0, -top);
        return rt;
    }
}

public class SceneBuilder : EditorWindow
{
    const float HEADER_H = 120f;
    const float BOTTOM_H = 160f;
    const string THEME_ASSET_PATH = "Assets/UiTheme.asset";

    [MenuItem("Tools/LifeSim/Build Main Scene")]
    public static void BuildMainScene()
    {
        if (!EditorUtility.DisplayDialog("LifeSim Scene Builder",
            "Tạo scene mới với đầy đủ Systems, Canvas, MainUI, ModalLayer và auto-gán reference?",
            "Build", "Cancel"))
            return;

        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        scene.name = "Main";

        // --- Systems ---
        var systems = new GameObject("Systems");
        var bootstrap = systems.AddComponent<Bootstrap>();
        SaveSystem saveSys = null;
        // Nếu bạn dùng autosave:
        saveSys = systems.AddComponent<SaveSystem>();

        // --- Canvas ---
        var canvasGO = new GameObject("MainCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        // Background screen image (để theme đổi màu nền)
        var bgScreen = new GameObject("BgScreen", typeof(RectTransform), typeof(Image));
        bgScreen.transform.SetParent(canvasGO.transform, false);
        (bgScreen.transform as RectTransform).anchorMin = Vector2.zero;
        (bgScreen.transform as RectTransform).anchorMax = Vector2.one;
        (bgScreen.transform as RectTransform).offsetMin = Vector2.zero;
        (bgScreen.transform as RectTransform).offsetMax = Vector2.zero;

        // --- MainUI (GameUIController) ---
        var mainUI = CreateEmpty("MainUI", canvasGO.transform);
        var gui = mainUI.AddComponent<GameUIController>();

        // --- Start Menu ---
        var startMenu = CreatePanel("StartMenu", mainUI.transform, new Color(0,0,0,0)); // trong suốt, nền dùng bgScreen
        var smRT = startMenu.GetComponent<RectTransform>().MiddleBetween(0,0);
        var title = CreateTMP("Subtitle", startMenu.transform, "A Life Simulation VN", 36, TextAlignmentOptions.Center);
        ((RectTransform)title.transform).anchoredPosition = new Vector2(0, 300);
        var btnStart = CreateButton("btnStart", startMenu.transform, "Start New Game", out var btnStartComp);
        ((RectTransform)btnStart.transform).anchoredPosition = new Vector2(0, 120);
        var btnHowTo = CreateButton("btnHowTo", startMenu.transform, "How to Play", out var btnHowToComp);
        ((RectTransform)btnHowTo.transform).anchoredPosition = new Vector2(0, -20);
        // EN/VI toggle
        var langGroup = CreatePanel("LangGroup", startMenu.transform, new Color(1,1,1,0));
        var lgRT = langGroup.GetComponent<RectTransform>();
        lgRT.anchorMin = lgRT.anchorMax = new Vector2(0.5f,0.5f); lgRT.sizeDelta = new Vector2(400,110); lgRT.anchoredPosition = new Vector2(0,-200);
        var btnEN = CreateButton("btnEN", langGroup.transform, "EN", out var btnENComp);
        var btnVI = CreateButton("btnVI", langGroup.transform, "VI", out var btnVIComp);
        ((RectTransform)btnEN.transform).anchoredPosition = new Vector2(-90,0);
        ((RectTransform)btnVI.transform).anchoredPosition = new Vector2( 90,0);
        // Controller
        var sm = startMenu.AddComponent<LifeSim.Presentation.UI.StartMenuController>();
        SetSerialized(sm, ("btnStart", btnStartComp), ("btnHowTo", btnHowToComp), ("btnEN", btnENComp), ("btnVI", btnVIComp), ("subtitle", title));

        // Header
        var header = CreatePanel("Header", mainUI.transform, new Color(0,0,0,0.0f)); // màu sẽ do theme
        header.GetComponent<RectTransform>().TopStretch(HEADER_H);
        var txtYear = CreateTMP("txtYear", header.transform, "Jan 2, 2024", 36, TextAlignmentOptions.Left);
        ((RectTransform)txtYear.transform).anchorMin = new Vector2(0, 0.5f);
        ((RectTransform)txtYear.transform).anchorMax = new Vector2(0, 0.5f);
        ((RectTransform)txtYear.transform).pivot = new Vector2(0, 0.5f);
        ((RectTransform)txtYear.transform).anchoredPosition = new Vector2(24, 0);

        var txtFund = CreateTMP("txtFund", header.transform, "0", 36, TextAlignmentOptions.Right);
        ((RectTransform)txtFund.transform).anchorMin = new Vector2(1, 0.5f);
        ((RectTransform)txtFund.transform).anchorMax = new Vector2(1, 0.5f);
        ((RectTransform)txtFund.transform).pivot = new Vector2(1, 0.5f);
        ((RectTransform)txtFund.transform).anchoredPosition = new Vector2(-24, 0);

        // Money Pill (bg + stroke + 2 text)
        var pill = CreatePanel("MoneyPill", header.transform, new Color(1,1,1,0));
        var pillRT = pill.GetComponent<RectTransform>();
        pillRT.anchorMin = new Vector2(0.65f, 0.5f); pillRT.anchorMax = new Vector2(0.98f, 0.5f);
        pillRT.pivot = new Vector2(1,0.5f); pillRT.sizeDelta = new Vector2(0, 80); pillRT.anchoredPosition = new Vector2(-20,0);
        var pillBg = pill.AddComponent<Image>(); // theme sẽ tô màu
        var pillStroke = new GameObject("Stroke", typeof(RectTransform), typeof(Image));
        pillStroke.transform.SetParent(pill.transform, false);
        (pillStroke.transform as RectTransform).FullStretch();
        // 2 text
        var txtMoney = CreateTMP("txtMoney", pill.transform, "$ 50.146", 36, TextAlignmentOptions.Left);
        ((RectTransform)txtMoney.transform).anchoredPosition = new Vector2(-100,0);
        var txtIncome = CreateTMP("txtIncome", pill.transform, "(+146/mo)", 28, TextAlignmentOptions.Right);
        ((RectTransform)txtIncome.transform).anchoredPosition = new Vector2(100,0);

        // Story & Settings chips (bên phải màn hình)
        var chips = CreatePanel("Chips", mainUI.transform, new Color(1,1,1,0));
        var chipsRT = chips.GetComponent<RectTransform>();
        chipsRT.anchorMin = chipsRT.anchorMax = new Vector2(1,0.8f); chipsRT.pivot = new Vector2(1,0.5f); chipsRT.anchoredPosition = new Vector2(-40,0);
        var btnStory = CreateButton("btnStory", chips.transform, "Story", out var btnStoryComp);
        ((RectTransform)btnStory.transform).sizeDelta = new Vector2(220,80);
        var btnSettings = CreateButton("btnSettings", chips.transform, "Settings", out var btnSettingsComp);
        ((RectTransform)btnSettings.transform).anchoredPosition = new Vector2(0,-110);
        ((RectTransform)btnSettings.transform).sizeDelta = new Vector2(220,80);

// --- HUD Container (bao Header + Chips) ---
        var hudGO = new GameObject("HUD", typeof(RectTransform));
        hudGO.transform.SetParent(mainUI.transform, false);
        (hudGO.transform as RectTransform).FullStretch();

        // Move header, chips vào HUD
        header.transform.SetParent(hudGO.transform, false);
        chips.transform.SetParent(hudGO.transform, false);

        // HUD Controller
        var hud = hudGO.AddComponent<LifeSim.Presentation.UI.HudController>();
        SetSerialized(hud,
            ("txtDate", txtYear),
            ("txtMoney", txtMoney),
            ("txtIncome", txtIncome),
            ("btnStory", btnStoryComp),
            ("btnSettings", btnSettingsComp)
        );

        // Bottom Icon Bar
        var bottomNav = CreatePanel("BottomBar", mainUI.transform, new Color(1,1,1,0));
        bottomNav.GetComponent<RectTransform>().BottomStretch(BOTTOM_H);
        var bottomBg = bottomNav.GetComponent<Image>(); // theme tô màu
        // 6 nút icon (tạm là chữ, sau bạn thay sprite)
        var btnTree = CreateButton("btnTree", bottomNav.transform, "", out var _btnTree);
        var btnLog  = CreateButton("btnLog", bottomNav.transform, "", out var _btnLog);
        var btnAssets = CreateButton("btnAssets", bottomNav.transform, "", out var _btnAssets);
        var btnBusiness = CreateButton("btnBusiness", bottomNav.transform, "", out var _btnBusiness);
        var btnPath = CreateButton("btnPath", bottomNav.transform, "", out var _btnPath);
        var btnBook = CreateButton("btnBook", bottomNav.transform, "", out var _btnBook); // icon thư viện

        // bố trí 5 nút ngang
        float w = 140; float space = 24;
        var navRt = bottomNav.GetComponent<RectTransform>();
        float totalW = w*6 + space*5;
        float startX = -totalW/2f + w/2f;
        var navBtns = new[] { btnTree, btnLog, btnAssets, btnBusiness, btnPath, btnBook };
        for (int i=0;i<navBtns.Length;i++)
        {
            var rt = (RectTransform)navBtns[i].transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(w, 90);
            rt.anchoredPosition = new Vector2(startX + i*(w+space), 0);
        }

        // 5 Panels giữa
        var panelTree = CreatePanel("panelTree", mainUI.transform, new Color(1,1,1,0));
        panelTree.GetComponent<RectTransform>().MiddleBetween(HEADER_H, BOTTOM_H);
        var panelLog = CreatePanel("panelLog", mainUI.transform, new Color(1,1,1,0));
        panelLog.GetComponent<RectTransform>().MiddleBetween(HEADER_H, BOTTOM_H);
        var panelAssets = CreatePanel("panelAssets", mainUI.transform, new Color(1,1,1,0));
        panelAssets.GetComponent<RectTransform>().MiddleBetween(HEADER_H, BOTTOM_H);
        var panelBusiness = CreatePanel("panelBusiness", mainUI.transform, new Color(1,1,1,0));
        panelBusiness.GetComponent<RectTransform>().MiddleBetween(HEADER_H, BOTTOM_H);
        var panelPath = CreatePanel("panelPath", mainUI.transform, new Color(1,1,1,0));
        panelPath.GetComponent<RectTransform>().MiddleBetween(HEADER_H, BOTTOM_H);

        // TMP demo text mỗi panel
        CreateTMP("labelTree", panelTree.transform, "Tree Panel", 40, TextAlignmentOptions.Center);
        CreateTMP("labelLog", panelLog.transform, "Game Log Panel", 40, TextAlignmentOptions.Center);
        CreateTMP("labelAssets", panelAssets.transform, "Assets Panel", 40, TextAlignmentOptions.Center);
        CreateTMP("labelBusiness", panelBusiness.transform, "Business Panel", 40, TextAlignmentOptions.Center);
        CreateTMP("labelPath", panelPath.transform, "Path of Life Panel", 40, TextAlignmentOptions.Center);

        // Gán vào GameUIController
        {
            var so = new SerializedObject(gui);
            so.FindProperty("txtYear").objectReferenceValue = txtYear;
            so.FindProperty("txtFund").objectReferenceValue = txtFund;
            so.FindProperty("btnSettings").objectReferenceValue = btnSettings.GetComponent<Button>();

            so.FindProperty("btnTree").objectReferenceValue = btnTree.GetComponent<Button>();
            so.FindProperty("btnLog").objectReferenceValue = btnLog.GetComponent<Button>();
            so.FindProperty("btnAssets").objectReferenceValue = btnAssets.GetComponent<Button>();
            so.FindProperty("btnBusiness").objectReferenceValue = btnBusiness.GetComponent<Button>();
            so.FindProperty("btnPath").objectReferenceValue = btnPath.GetComponent<Button>();

            so.FindProperty("panelTree").objectReferenceValue = panelTree;
            so.FindProperty("panelLog").objectReferenceValue = panelLog;
            so.FindProperty("panelAssets").objectReferenceValue = panelAssets;
            so.FindProperty("panelBusiness").objectReferenceValue = panelBusiness;
            so.FindProperty("panelPath").objectReferenceValue = panelPath;
            so.ApplyModifiedProperties();
        }

        // --- ModalLayer + ModalManager ---
        var modalLayer = CreateEmpty("ModalLayer", canvasGO.transform);
        var modalMgr = modalLayer.AddComponent<ModalManager>();
        ((RectTransform)modalLayer.transform).FullStretch();

        // Tạo EventChoice Button Prefab (nếu chưa tồn tại)
        var btnPrefab = CreateButton("EventChoiceButtonPrefab_TMP", modalLayer.transform, "Choice", out var _choiceBtn);
        // Đưa nó ra Project làm prefab tạm (Assets/Prefabs/UI)
        var prefabsDir = "Assets/Prefabs/UI";
        if (!System.IO.Directory.Exists(prefabsDir))
            System.IO.Directory.CreateDirectory(prefabsDir);
        var prefabPath = $"{prefabsDir}/EventChoiceButton.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(btnPrefab, prefabPath);
        GameObject.DestroyImmediate(btnPrefab); // bỏ bản trong scene, chỉ dùng prefab
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Tạo factory panel modal
        GameObject eventPanel, settingsPanel, schoolPanel, univPanel, majorPanel, careerPanel, underQPanel, loanPanel, promoPanel;

        eventPanel   = CreateComicPanel(modalLayer.transform, "EventPanel", out var eventCG, out var eventComic, out var eventTitle, out var eventBody, out var eventClose, out var eventChoicesRoot);
        settingsPanel= CreateComicPanel(modalLayer.transform, "SettingsPanel", out var settingsCG, out var settingsComic, out var settingsTitle, out var settingsBody, out var settingsClose, out _);

        // Add scripts cho modal còn lại bằng cách duplicate cấu trúc ComicPanel
        schoolPanel  = CreateComicPanel(modalLayer.transform, "SchoolPanel", out _, out var schoolComic, out var schoolTitle, out var schoolBody, out var schoolClose, out var schoolList);
        var school = schoolPanel.AddComponent<SchoolChoiceModal>();
        // optionButtonPrefab dùng lại EventChoice prefab, footerNote thêm text
        var schoolFooter = CreateTMP("footerNote", schoolPanel.transform.Find("Content"), "Pick a school…", 28, TextAlignmentOptions.MidlineLeft);
        SetSerialized(school, ("panel", schoolComic), ("listRoot", schoolList), ("optionButtonPrefab", prefab.GetComponent<Button>()), ("footerNote", schoolFooter));

        univPanel    = CreateComicPanel(modalLayer.transform, "UniversityPanel", out _, out var univComic, out var univTitle, out var univBody, out var univClose, out var univList);
        var univ = univPanel.AddComponent<UniversityChoiceModal>();
        var univFooter = CreateTMP("footerNote", univPanel.transform.Find("Content"), "Pick a major…", 28, TextAlignmentOptions.MidlineLeft);
        SetSerialized(univ, ("panel", univComic), ("listRoot", univList), ("optionButtonPrefab", prefab.GetComponent<Button>()), ("footerNote", univFooter));

        majorPanel   = CreateComicPanel(modalLayer.transform, "MajorPanel", out _, out var majorComic, out var majorTitle, out var majorBody, out var majorClose, out var majorList);
        var major = majorPanel.AddComponent<MajorChoiceModal>();
        var majorFooter = CreateTMP("footerNote", majorPanel.transform.Find("Content"), "Choose your major…", 28, TextAlignmentOptions.MidlineLeft);
        SetSerialized(major, ("panel", majorComic), ("listRoot", majorList), ("optionButtonPrefab", prefab.GetComponent<Button>()), ("footerNote", majorFooter));

        careerPanel  = CreateComicPanel(modalLayer.transform, "CareerPanel", out _, out var careerComic, out var careerTitle, out var careerBody, out var careerClose, out var careerList);
        var career = careerPanel.AddComponent<CareerChoiceModal>();
        // Underqualified modal sẽ tạo phía dưới rồi gán
        SetSerialized(career, ("panel", careerComic), ("listRoot", careerList), ("optionButtonPrefab", prefab.GetComponent<Button>()));

        underQPanel  = CreateComicPanel(modalLayer.transform, "UnderqualifiedPanel", out _, out var uqComic, out var uqTitle, out var uqBody, out var uqClose, out _);
        var uq = underQPanel.AddComponent<UnderqualifiedChoiceModal>();
        // Add txtMessage + btnOk cho Underqualified
        var uqMsg = CreateTMP("txtMessage", underQPanel.transform.Find("Content"), "You do not meet the requirements.", 32, TextAlignmentOptions.Center);
        var uqBtnOk = CreateButton("btnOk", underQPanel.transform.Find("Content"), "OK", out var uqBtnOKComp);
        ((RectTransform)uqBtnOk.transform).anchoredPosition = new Vector2(0, -260);
        SetSerialized(uq, ("panel", uqComic), ("txtMessage", uqMsg), ("btnOk", uqBtnOKComp));

        // Gán underqualifiedModal vào CareerChoiceModal
        SetSerialized(career, ("underqualifiedModal", uq));

        loanPanel    = CreateComicPanel(modalLayer.transform, "LoanPanel", out _, out var loanComic, out var loanTitle, out var loanBody, out var loanClose, out _);
        var loan = loanPanel.AddComponent<LoanModal>();
        var inAmount = CreateTMPInput("inputAmount", loanPanel.transform.Find("Content"), "Amount");
        var inTerm   = CreateTMPInput("inputTerm", loanPanel.transform.Find("Content"), "Term (months)");
        var note     = CreateTMP("txtNote", loanPanel.transform.Find("Content"), "", 28, TextAlignmentOptions.MidlineLeft);
        var btnConfirm = CreateButton("btnConfirm", loanPanel.transform.Find("Content"), "Confirm", out var btnConfirmComp);
        ((RectTransform)btnConfirm.transform).anchoredPosition = new Vector2(0, -260);
        SetSerialized(loan, ("panel", loanComic), ("inputAmount", inAmount), ("inputTerm", inTerm), ("btnConfirm", btnConfirmComp), ("txtNote", note));

        promoPanel   = CreateComicPanel(modalLayer.transform, "PromotionPanel", out _, out var promoComic, out var promoTitle, out var promoBody, out var promoClose, out _);
        var promo = promoPanel.AddComponent<PromotionModal>();
        var btnAccept = CreateButton("btnAccept", promoPanel.transform.Find("Content"), "Accept", out var btnAcceptComp);
        var btnDecline = CreateButton("btnDecline", promoPanel.transform.Find("Content"), "Decline", out var btnDeclineComp);
        ((RectTransform)btnAccept.transform).anchoredPosition = new Vector2(-140, -260);
        ((RectTransform)btnDecline.transform).anchoredPosition = new Vector2(140, -260);
        SetSerialized(promo, ("panel", promoComic), ("txtTitle", promoTitle), ("txtBody", promoBody), ("btnAccept", btnAcceptComp), ("btnDecline", btnDeclineComp));

        // Ẩn tất cả modal mặc định
        foreach (Transform t in modalLayer.transform) t.gameObject.SetActive(false);
        // Riêng EventPanel/SettingsPanel cũng off
        eventPanel.SetActive(false);
        settingsPanel.SetActive(false);

        // --- Gán ModalManager refs ---
        {
            var so = new SerializedObject(modalMgr);
            // Event
            so.FindProperty("eventPanel").objectReferenceValue = eventComic;
            so.FindProperty("eventChoicesRoot").objectReferenceValue = eventChoicesRoot;
            so.FindProperty("eventChoiceButtonPrefab").objectReferenceValue = prefab.GetComponent<Button>();
            // Settings
            so.FindProperty("settingsPanel").objectReferenceValue = settingsComic;
            // Others
            var schoolProp = so.FindProperty("schoolModal"); if (schoolProp != null) schoolProp.objectReferenceValue = school;
            var univProp   = so.FindProperty("universityModal"); if (univProp != null) univProp.objectReferenceValue = univ;
            var majorProp  = so.FindProperty("majorModal"); if (majorProp != null) majorProp.objectReferenceValue = major;
            var careerProp = so.FindProperty("careerModal"); if (careerProp != null) careerProp.objectReferenceValue = career;
            var uqProp     = so.FindProperty("underqualifiedModal"); if (uqProp != null) uqProp.objectReferenceValue = uq;
            var loanProp   = so.FindProperty("loanModal"); if (loanProp != null) loanProp.objectReferenceValue = loan;
            var promoProp  = so.FindProperty("promotionModal"); if (promoProp != null) promoProp.objectReferenceValue = promo;
            so.ApplyModifiedProperties();
        }

        // --- Bind UI into Bootstrap ---
        {
            var so = new SerializedObject(bootstrap);
            so.FindProperty("ui").objectReferenceValue = gui;
            so.ApplyModifiedProperties();
        }

        // --- Sắp xếp layer thứ tự hiển thị ---
        modalLayer.transform.SetAsLastSibling();

        // Auto add PanelAutoBinder
        var binderGO = new GameObject("PanelAutoBinder");
        binderGO.AddComponent<LifeSim.Presentation.UI.PanelAutoBinder>();

        // Auto theme
        var themeAsset = AssetDatabase.LoadAssetAtPath<UiTheme>(THEME_ASSET_PATH);
        if (themeAsset != null)
        {
            var applier = mainUI.AddComponent<UiThemeApplier>();
            applier.theme = themeAsset;
            // Map tham chiếu:
            applier.bgWholeScreen = GameObject.Find("BgScreen")?.GetComponent<Image>();
            applier.bgBottomBar   = bottomNav.GetComponent<Image>();
            applier.txtHeaderLeft = txtYear;
            // Money pill
            applier.txtPillMoney  = GameObject.Find("txtMoney")?.GetComponent<TextMeshProUGUI>();
            applier.txtPillIncome = GameObject.Find("txtIncome")?.GetComponent<TextMeshProUGUI>();
            applier.pillBg        = GameObject.Find("MoneyPill")?.GetComponent<Image>();
            applier.pillStroke    = GameObject.Find("Stroke")?.GetComponent<Image>();
            // Chips
            applier.btnStoryBg    = GameObject.Find("btnStory")?.GetComponent<Image>();
            applier.btnSettingsBg = GameObject.Find("btnSettings")?.GetComponent<Image>();

            // modal overlay/content: lấy từ EventPanel làm mẫu (để sau bạn mở rộng)
            if (eventPanel != null)
            {
                var overlay = eventPanel.transform.Find("Overlay")?.GetComponent<Image>();
                var content = eventPanel.transform.Find("Content")?.GetComponent<Image>();
                applier.modalOverlay   = overlay;
                applier.modalContentBg = content;
            }
            applier.Apply();
        }

        // --- Lưu scene ---
        var path = EditorUtility.SaveFilePanelInProject("Save Scene As", "Main", "unity", "Chọn nơi lưu scene");
        if (!string.IsNullOrEmpty(path))
        {
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), path);
        }

        EditorUtility.DisplayDialog("LifeSim Scene Builder", "✅ Hoàn tất! Mở scene và bấm Play để test.", "OK");
    }

    // ---------- Helpers ----------
    static GameObject CreateEmpty(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        (go.transform as RectTransform).FullStretch();
        return go;
    }

    static GameObject CreatePanel(string name, Transform parent, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>().FullStretch();
        var img = go.GetComponent<Image>();
        img.color = bg;
        return go;
    }

    static TextMeshProUGUI CreateTMP(string name, Transform parent, string text, int fontSize, TextAlignmentOptions align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(800, 120);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        return tmp;
    }

    static GameObject CreateButton(string name, Transform parent, string label, out Button button)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 80);
        var img = go.GetComponent<Image>(); img.color = new Color(1,1,1,0.15f);
        button = go.GetComponent<Button>();
        var txt = CreateTMP("Label", go.transform, label, 28, TextAlignmentOptions.Center);
        (txt.transform as RectTransform).FullStretch();
        return go;
    }

    static Transform CreateVertical(string name, Transform parent, float spacing = 12, RectOffset padding = null)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(800, 800);
        var vlg = go.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = spacing;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        if (padding == null) padding = new RectOffset(24,24,24,24);
        vlg.padding = padding;
        var fitter = go.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return go.transform;
    }

    static GameObject CreateComicPanel(Transform parent, string name,
        out CanvasGroup cg, out ComicPanel comic,
        out TMP_Text txtTitle, out TMP_Text txtBody, out Button btnClose, out Transform choicesRootOrNull)
    {
        var panel = CreatePanel(name, parent, new Color(0,0,0,0));
        var rt = panel.GetComponent<RectTransform>().FullStretch();

        // Overlay
        var overlayGO = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
        overlayGO.transform.SetParent(panel.transform, false);
        var overlayRT = (RectTransform)overlayGO.transform;
        overlayRT.FullStretch();
        var ovImg = overlayGO.GetComponent<Image>();
        ovImg.color = new Color(0,0,0,0.5f);

        // Content
        var content = CreatePanel("Content", panel.transform, new Color(1,1,1,0.92f));
        var crt = content.GetComponent<RectTransform>();
        crt.anchorMin = crt.anchorMax = new Vector2(0.5f, 0.5f);
        crt.sizeDelta = new Vector2(900, 1200);

        txtTitle = CreateTMP("txtTitle", content.transform, "Title", 44, TextAlignmentOptions.Center);
        ((RectTransform)txtTitle.transform).anchoredPosition = new Vector2(0, 460);

        txtBody = CreateTMP("txtBody", content.transform, "Body...", 30, TextAlignmentOptions.Top);
        ((RectTransform)txtBody.transform).sizeDelta = new Vector2(820, 700);
        ((RectTransform)txtBody.transform).anchoredPosition = new Vector2(0, 140);

        var closeGO = CreateButton("btnClose", content.transform, "×", out btnClose);
        ((RectTransform)closeGO.transform).anchorMin = new Vector2(1,1);
        ((RectTransform)closeGO.transform).anchorMax = new Vector2(1,1);
        ((RectTransform)closeGO.transform).pivot = new Vector2(1,1);
        ((RectTransform)closeGO.transform).anchoredPosition = new Vector2(-16, -16);
        ((RectTransform)closeGO.transform).sizeDelta = new Vector2(72, 72);

        // Optional: list root (cho Event/School/University/Major/Career)
        var choicesRoot = CreateVertical("ChoicesRoot", content.transform);
        ((RectTransform)choicesRoot).anchoredPosition = new Vector2(0, -220);

        // Components
        cg = panel.AddComponent<CanvasGroup>();
        comic = panel.AddComponent<ComicPanel>();
        // serialize refs
        var so = new SerializedObject(comic);
        so.FindProperty("overlay").objectReferenceValue = overlayGO.GetComponent<Image>();
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("btnClose").objectReferenceValue = btnClose;
        so.FindProperty("txtTitle").objectReferenceValue = txtTitle;
        so.FindProperty("txtBody").objectReferenceValue = txtBody;
        so.ApplyModifiedProperties();

        choicesRootOrNull = choicesRoot;

        // Off mặc định
        panel.SetActive(false);
        return panel;
    }

    static TMP_InputField CreateTMPInput(string name, Transform parent, string placeholder)
    {
        var root = new GameObject(name, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        var rt = (RectTransform)root.transform;
        rt.sizeDelta = new Vector2(700, 80);
        rt.anchoredPosition = new Vector2(0, -140);

        var bg = root.AddComponent<Image>();
        bg.color = new Color(1,1,1,0.15f);

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(root.transform, false);
        var textRT = (RectTransform)textGO.transform;
        textRT.FullStretch();
        var text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 28;

        var phGO = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
        phGO.transform.SetParent(root.transform, false);
        var phRT = (RectTransform)phGO.transform;
        phRT.FullStretch();
        var ph = phGO.GetComponent<TextMeshProUGUI>();
        ph.text = placeholder;
        ph.fontStyle = FontStyles.Italic;
        ph.color = new Color(1,1,1,0.5f);
        ph.fontSize = 28;

        var input = root.AddComponent<TMP_InputField>();
        input.textViewport = textRT;
        input.textComponent = text;
        input.placeholder = ph;

        return input;
    }

    static void SetSerialized(Object target, params (string prop, Object value)[] pairs)
    {
        var so = new SerializedObject(target);
        foreach (var (prop, value) in pairs)
        {
            var p = so.FindProperty(prop);
            if (p != null) p.objectReferenceValue = value;
        }
        so.ApplyModifiedProperties();
    }
}
#endif
