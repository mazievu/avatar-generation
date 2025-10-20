// Assets/Editor/SceneBuilderv3_Fixed_VLG.cs
#if UNITY_EDITOR
using System;                  // AppDomain, Type
using System.IO;               // Path
using System.Reflection;       // ReflectionTypeLoadException
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneBuilderv3_Fixed_VLG : EditorWindow
{
    [MenuItem("LifeSim/Build Main Scene (VLG Layout)")]
    public static void BuildFromMenu()
    {
        var win = GetWindow<SceneBuilderv3_Fixed_VLG>("LifeSim Scene Builder (VLG)");
        win.minSize = new Vector2(420, 300);
        win.BuildAll();
    }

    // ===== CONFIG =====
    static readonly Color ColorBackground    = new Color(0.08f, 0.09f, 0.12f);
    static readonly Color ColorMoneyPillBg   = new Color(0.12f, 0.16f, 0.22f, 0.95f);
    static readonly Color ColorButtonRed     = new Color(0.93f, 0.24f, 0.24f);
    static readonly Color ColorButtonTeal    = new Color(0.10f, 0.72f, 0.70f);
    static readonly Color ColorNavBackground = new Color(0.11f, 0.12f, 0.16f, 0.96f);
    static readonly Color PanelColor         = new Color(0.15f, 0.16f, 0.20f, 0.98f);
    static readonly Color AvatarBgColor      = new Color(0.20f, 0.22f, 0.26f, 1f);
    static readonly Color BadgeColor         = new Color(0.95f, 0.76f, 0.22f, 1f);
    static readonly Vector2 ReferenceResolution = new Vector2(1080, 2340);

    // chiều cao cố định của các vùng khi dùng VLG
    const float HEADER_H = 220f;
    const float TITLE_H  = 140f;
    const float BOTTOM_H = 180f;

    // Tên field (sửa nếu runtime scripts khác tên)
    static class FieldNames
    {
        public const string ChoiceButton_labelField = "buttonText";
        public const string CharacterNode_nameField = "nameText";
        public const string CharacterNode_ageField = "ageText";
        public const string CharacterNode_avatarField = "avatarImage";

        public const string GameUIController_modalManager = "modalManager";
        public const string Bootstrap_ui = "ui";

        public const string FamilyTreePanel_nodePrefab = "nodePrefab";
        public const string GameLogPanel_logEntryPrefab = "logEntryPrefab";
        public const string AssetsPanel_assetSlotPrefab = "assetSlotPrefab";
        public const string BusinessPanel_hotspotPrefab = "hotspotPrefab";
        public const string PathOfLifePanel_milestonePrefab = "milestonePrefab";

        public const string CareerChoice_underqualified = "underqualifiedModal";

        public const string ModalManager_eventModal = "eventModal";
        public const string ModalManager_schoolModal = "schoolModal";
        public const string ModalManager_universityModal = "universityModal";
        public const string ModalManager_majorModal = "majorModal";
        public const string ModalManager_careerModal = "careerModal";
        public const string ModalManager_loanModal = "loanModal";
        public const string ModalManager_promotionModal = "promotionModal";
    }

    // ===== ENTRY =====
    void BuildAll()
    {
        // Scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "Main";

        // Systems
        var systems = CreateGameObject("Systems", null);
        var bootstrap = systems.AddOrGetComponentByName("Bootstrap"); // runtime script theo tên

        // Canvas Root
        var canvasGO = CreateCanvasRoot("MainCanvas");
        var bg = CreatePanel("Background", canvasGO.transform, ColorBackground);
        StretchToParent(bg.GetComponent<RectTransform>(), new RectOffset(0,0,0,0));

        var uiRoot = CreateGameObject("GameUI_Controller", canvasGO.transform);
        var gameUIController = uiRoot.AddOrGetComponentByName("GameUIController");

        // SafeArea
        var safeArea = CreateGameObject("SafeArea", uiRoot.transform);
        safeArea.AddComponent<SafeAreaFitter>();
        var safeRT = safeArea.GetComponent<RectTransform>();
        SetRectTransform(safeRT, anchorMin:new Vector2(0,0), anchorMax:new Vector2(1,1), pivot:new Vector2(0.5f,0.5f));

        // ===== VerticalLayoutGroup trên SafeArea =====
        var vlg = safeArea.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(0,0,0,0);
        vlg.spacing = 0;
        vlg.childControlHeight = true;      // tôn trọng preferredHeight/minHeight
        vlg.childForceExpandHeight = false; // KHÔNG ép con dãn hết

        // ==== Header (fixed 220) ====
        var header = CreateGameObject("Header", safeArea.transform);
        var headerRT = header.GetComponent<RectTransform>();
        // khi dùng VLG, anchor/pos không còn quan trọng — để mặc định stretch theo chiều ngang
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(1, 1);
        headerRT.pivot     = new Vector2(0.5f, 1f);
        headerRT.sizeDelta = new Vector2(0, HEADER_H);
        var headerLE = header.AddComponent<LayoutElement>();
        headerLE.minHeight = HEADER_H;
        headerLE.preferredHeight = HEADER_H;

        var headerHL = header.AddComponent<HorizontalLayoutGroup>();
        headerHL.padding = new RectOffset(56,56,24,24);
        headerHL.childAlignment = TextAnchor.MiddleLeft;
        headerHL.childControlWidth = false;
        headerHL.childControlHeight = false;
        headerHL.spacing = 24;

        var txtDate = CreateTMP("Txt_Date", header.transform, "Jan 2, 2024", 48, TextAlignmentOptions.Left, Color.white, out _);
        txtDate.enableAutoSizing = true; txtDate.fontSizeMin = 32; txtDate.fontSizeMax = 56;

        var moneyPill = CreateShadowedPanel("MoneyPill", header.transform, ColorMoneyPillBg, 50f, out var pillBG, out var pillContent);
        var moneyRT = moneyPill.GetComponent<RectTransform>();
        moneyRT.sizeDelta = new Vector2(600, 120);
        var pillHL = pillContent.gameObject.AddComponent<HorizontalLayoutGroup>();
        pillHL.padding = new RectOffset(24,24,12,12);
        pillHL.spacing = 16;
        pillHL.childAlignment = TextAnchor.MiddleCenter;

        var moneyIcon = CreateTMP("Icon", pillContent, "$", 56, TextAlignmentOptions.Center, Color.white, out _);
        var txtMoney  = CreateTMP("Txt_Money", pillContent, "50,146", 56, TextAlignmentOptions.MidlineLeft, Color.white, out _);
        var txtIncome = CreateTMP("Txt_Income", pillContent, "(+146/mo)", 36, TextAlignmentOptions.MidlineLeft, new Color(1,1,1,0.7f), out _);

        // ==== TitleRow (fixed 140) ====
        var titleRow = CreateGameObject("TitleRow", safeArea.transform);
        var titleRT = titleRow.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot     = new Vector2(0.5f, 1f);
        titleRT.sizeDelta = new Vector2(0, TITLE_H);
        var titleLE = titleRow.AddComponent<LayoutElement>();
        titleLE.minHeight = TITLE_H;
        titleLE.preferredHeight = TITLE_H;

        var titleHL = titleRow.AddComponent<HorizontalLayoutGroup>();
        titleHL.padding = new RectOffset(56,56,0,0);
        titleHL.childAlignment = TextAnchor.MiddleLeft;
        titleHL.spacing = 24;

        var familyName = CreateTMP("FamilyName", titleRow.transform, "Family", 72, TextAlignmentOptions.Left, ColorButtonRed, out _);
        familyName.enableAutoSizing = true; familyName.fontSizeMin = 48; familyName.fontSizeMax = 80;

        var spacer = CreateGameObject("Spacer", titleRow.transform);
        var spacerLE = spacer.AddComponent<LayoutElement>(); spacerLE.flexibleWidth = 1;

        var btnStory    = CreateChipButton("Btn_Story", titleRow.transform, "Story",    ColorButtonRed,  out var btnStoryComp);
        var btnSettings = CreateChipButton("Btn_Settings", titleRow.transform, "Settings", ColorButtonTeal, out var btnSettingsComp);

        // ==== Panels_Container (giãn chiếm phần còn lại) ====
        var panelsContainer = CreateGameObject("Panels_Container", safeArea.transform);
        var pcRT = panelsContainer.GetComponent<RectTransform>();
        pcRT.sizeDelta = Vector2.zero; // cho VLG quản lý
        var panelsLE = panelsContainer.AddComponent<LayoutElement>();
        panelsLE.minHeight = 0f;
        panelsLE.flexibleHeight = 1f; // GIÃN

        // Panels + attach scripts by name
        var panelTree     = CreatePanelWithComponent("Panel_Tree", panelsContainer.transform, "FamilyTreePanel", out var treeScript);
        var panelLog      = CreatePanelWithComponent("Panel_Log", panelsContainer.transform, "GameLogPanel", out var logScript);
        var panelAssets   = CreatePanelWithComponent("Panel_Assets", panelsContainer.transform, "AssetsPanel", out var assetsScript);
        var panelBusiness = CreatePanelWithComponent("Panel_Business", panelsContainer.transform, "BusinessPanel", out var businessScript);
        var panelPath     = CreatePanelWithComponent("Panel_Path", panelsContainer.transform, "PathOfLifePanel", out var pathScript);

        panelLog.SetActive(false);
        panelAssets.SetActive(false);
        panelBusiness.SetActive(false);
        panelPath.SetActive(false);

        // ==== BottomNav (fixed 180) ====
        var bottomNav = CreatePanel("BottomNav_Panel", safeArea.transform, ColorNavBackground);
        var bottomRT = bottomNav.GetComponent<RectTransform>();
        bottomRT.sizeDelta = new Vector2(0, BOTTOM_H);
        var bottomLE = bottomNav.AddComponent<LayoutElement>();
        bottomLE.minHeight = BOTTOM_H;
        bottomLE.preferredHeight = BOTTOM_H;

        var bHL = bottomNav.AddComponent<HorizontalLayoutGroup>();
        bHL.padding = new RectOffset(32, 32, 24, 32); // trên=24, dưới=32 (gesture bar)
        bHL.childAlignment = TextAnchor.MiddleCenter;
        bHL.spacing = 28;

        var btnTree     = CreateIconButton("Btn_Tree", bottomNav.transform, "Family Tree", out var btnTreeComp);
        var btnLog      = CreateIconButton("Btn_Log", bottomNav.transform, "Log", out var btnLogComp);
        var btnAssets   = CreateIconButton("Btn_Assets", bottomNav.transform, "Assets", out var btnAssetsComp);
        var btnBusiness = CreateIconButton("Btn_Business", bottomNav.transform, "Business", out var btnBusinessComp);
        var btnPath     = CreateIconButton("Btn_Path", bottomNav.transform, "Path", out var btnPathComp);

        // ===== MODAL LAYER =====
        var modalLayer = CreateGameObject("ModalLayer", canvasGO.transform);
        modalLayer.transform.SetAsLastSibling();
        var modalManager = modalLayer.AddOrGetComponentByName("ModalManager");
        var modalOverlay = CreatePanel("Overlay", modalLayer.transform, new Color(0,0,0,0.6f));
        StretchToParent(modalOverlay.GetComponent<RectTransform>(), new RectOffset(0,0,0,0));

        // ===== PREFABS =====
        string prefabDir = "Assets/Prefabs/UI";
        EnsureFolder(prefabDir);

        // ChoiceButton prefab
        var temp = CreateGameObject("TempPrefabContainer", null);
        var choiceBtnGO = CreateButton("ChoiceButton_Prefab", temp.transform, "Default Choice", out var choiceBtnComp_Button);
        var choiceBtnScript = choiceBtnGO.AddOrGetComponentByName("ChoiceButton");
        var choiceBtnLabel = choiceBtnGO.transform.Find("Label")?.GetComponent<TMP_Text>();
        AssignSerialized(choiceBtnScript, FieldNames.ChoiceButton_labelField, choiceBtnLabel);
        var choiceBtnPath = Path.Combine(prefabDir, "ChoiceButton_Prefab.prefab").Replace("\\", "/");
        SaveAsPrefab(choiceBtnGO, choiceBtnPath);
        DestroyImmediate(temp);

        // CharacterNode prefab
        temp = CreateGameObject("TempPrefabContainer", null);
        var charNodeGO = CreatePanel("CharacterNode_Prefab", temp.transform, PanelColor);
        SetRectTransform(charNodeGO.GetComponent<RectTransform>(), sizeDelta:new Vector2(220,260));
        var charNodeScript = charNodeGO.AddOrGetComponentByName("CharacterNode");

        var avatarImg = CreatePanel("Avatar", charNodeGO.transform, AvatarBgColor);
        SetRectTransform(avatarImg.GetComponent<RectTransform>(), sizeDelta:new Vector2(190,190), anchoredPosition:new Vector2(0,24));

        var nameTxt = CreateTMP("Name", charNodeGO.transform, "Character Name", 36, TextAlignmentOptions.Center, Color.white, out _);
        SetRectTransform(nameTxt.GetComponent<RectTransform>(), sizeDelta:new Vector2(200,56), anchoredPosition:new Vector2(0,-88));

        var ageBadge = CreatePanel("AgeBadge", avatarImg.transform, BadgeColor);
        SetRectTransform(ageBadge.GetComponent<RectTransform>(),
            anchorMin:new Vector2(1,1), anchorMax:new Vector2(1,1), pivot:new Vector2(1,1),
            sizeDelta:new Vector2(50,50), anchoredPosition:new Vector2(-6,-6));
        var ageTxt = CreateTMP("Age", ageBadge.transform, "24", 28, TextAlignmentOptions.Center, Color.black, out _);

        AssignSerialized(charNodeScript, FieldNames.CharacterNode_nameField, nameTxt);
        AssignSerialized(charNodeScript, FieldNames.CharacterNode_ageField, ageTxt);
        AssignSerialized(charNodeScript, FieldNames.CharacterNode_avatarField, avatarImg.GetComponent<Image>());

        var charNodePath = Path.Combine(prefabDir, "CharacterNode_Prefab.prefab").Replace("\\", "/");
        SaveAsPrefab(charNodeGO, charNodePath);
        DestroyImmediate(temp);

        // Other simple UI prefabs
        CreateSimpleUIPrefab(prefabDir, "LogEntry_Prefab", "Log Entry");
        CreateSimpleUIPrefab(prefabDir, "AssetSlot_Prefab", "Asset Slot");
        CreateSimpleUIPrefab(prefabDir, "BusinessHotspot_Prefab", "Business");
        CreateSimpleUIPrefab(prefabDir, "PathMilestone_Prefab", "Milestone");

        // Load prefabs
        var choiceBtnPrefab       = AssetDatabase.LoadAssetAtPath<GameObject>(choiceBtnPath);
        var nodePrefab            = AssetDatabase.LoadAssetAtPath<GameObject>(charNodePath);
        var logEntryPrefab        = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabDir, "LogEntry_Prefab.prefab"));
        var assetSlotPrefab       = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabDir, "AssetSlot_Prefab.prefab"));
        var businessHotspotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabDir, "BusinessHotspot_Prefab.prefab"));
        var pathMilestonePrefab   = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabDir, "PathMilestone_Prefab.prefab"));

        // Wire prefabs to panels
        AssignSerialized(treeScript,      FieldNames.FamilyTreePanel_nodePrefab,      nodePrefab);
        AssignSerialized(logScript,       FieldNames.GameLogPanel_logEntryPrefab,     logEntryPrefab);
        AssignSerialized(assetsScript,    FieldNames.AssetsPanel_assetSlotPrefab,     assetSlotPrefab);
        AssignSerialized(businessScript,  FieldNames.BusinessPanel_hotspotPrefab,     businessHotspotPrefab);
        AssignSerialized(pathScript,      FieldNames.PathOfLifePanel_milestonePrefab, pathMilestonePrefab);

        // ===== MODALS =====
        var eventModal          = CreateEventModal(modalLayer.transform, choiceBtnPrefab);
        var schoolModal         = CreateListModal(modalLayer.transform, "SchoolChoiceModal", "SchoolChoiceModal", choiceBtnPrefab, out var schoolComp);
        var univModal           = CreateListModal(modalLayer.transform, "UniversityChoiceModal", "UniversityChoiceModal", choiceBtnPrefab, out var univComp);
        var majorModal          = CreateListModal(modalLayer.transform, "MajorChoiceModal", "MajorChoiceModal", choiceBtnPrefab, out var majorComp);
        var careerModal         = CreateListModal(modalLayer.transform, "CareerChoiceModal", "CareerChoiceModal", choiceBtnPrefab, out var careerComp);
        var underqualifiedModal = CreateSimpleMessageModal(modalLayer.transform, "UnderqualifiedModal", "UnderqualifiedChoiceModal", "You are underqualified.");
        var promoModal          = CreateSimpleMessageModal(modalLayer.transform, "PromotionModal", "PromotionModal", "Congrats! Promotion!");
        var loanModal           = CreateLoanModal(modalLayer.transform);

        // Hide all modal children by default
        for (int i = 0; i < modalLayer.transform.childCount; i++)
            modalLayer.transform.GetChild(i).gameObject.SetActive(false);

        // ===== FINAL WIRING =====
        AssignSerialized(gameUIController, FieldNames.GameUIController_modalManager, modalManager);
        AssignSerialized(bootstrap,         FieldNames.Bootstrap_ui,                 uiRoot.GetComponentByName("GameUIController"));

        AssignSerialized(careerComp, FieldNames.CareerChoice_underqualified, underqualifiedModal.GetComponentByName("UnderqualifiedChoiceModal"));

        AssignSerialized(modalManager, FieldNames.ModalManager_eventModal,       eventModal.GetComponentByName("EventModal"));
        AssignSerialized(modalManager, FieldNames.ModalManager_schoolModal,      schoolComp);
        AssignSerialized(modalManager, FieldNames.ModalManager_universityModal,  univComp);
        AssignSerialized(modalManager, FieldNames.ModalManager_majorModal,       majorComp);
        AssignSerialized(modalManager, FieldNames.ModalManager_careerModal,      careerComp);
        AssignSerialized(modalManager, FieldNames.ModalManager_loanModal,        loanModal.GetComponentByName("LoanModal"));
        AssignSerialized(modalManager, FieldNames.ModalManager_promotionModal,   promoModal.GetComponentByName("PromotionModal"));

        // Save scene
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[LifeSim] Main Scene built successfully (VLG Layout).");
    }

    // ===== HELPERS =====
    GameObject CreateCanvasRoot(string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return go;
    }

    public static GameObject CreateGameObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        if (parent) go.transform.SetParent(parent, false);
        return go;
    }

    public static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        var go = CreateGameObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    public static TMP_Text CreateTMP(string name, Transform parent, string text, float size, TextAlignmentOptions align, Color color, out GameObject go)
    {
        go = CreateGameObject(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = align;
        return tmp;
    }

    public static GameObject CreateButton(string name, Transform parent, string label, out Button button)
    {
        var go = CreatePanel(name, parent, new Color(1,1,1,0.08f));
        var img = go.GetComponent<Image>(); img.raycastTarget = true;

        var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(420, 120);
        button = go.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(1,1,1,0.15f);
        colors.pressedColor = new Color(1,1,1,0.25f);
        button.colors = colors;

        var labelTMP = CreateTMP("Label", go.transform, label, 48, TextAlignmentOptions.Center, Color.white, out _);
        StretchToParent(labelTMP.GetComponent<RectTransform>(), new RectOffset(24,24,12,12));

        var outline = go.AddComponent<Outline>(); outline.effectColor = new Color(1,1,1,0.06f);
        return go;
    }

    public static GameObject CreateScrollView(string name, Transform parent, out RectTransform contentTransform)
    {
        var root = CreatePanel(name, parent, new Color(1,1,1,0.03f));

        var viewport = CreatePanel("Viewport", root.transform, new Color(1,1,1,0));
        StretchToParent(viewport.GetComponent<RectTransform>(), new RectOffset(8,8,8,8));
        var mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false;

        var content = CreateGameObject("Content", viewport.transform);
        contentTransform = content.GetComponent<RectTransform>();
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(16,16,16,16);
        vlg.spacing = 12; vlg.childControlWidth = true; vlg.childForceExpandWidth = true;

        var fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var sv = root.AddComponent<ScrollRect>();
        sv.viewport = viewport.GetComponent<RectTransform>();
        sv.content = contentTransform;
        sv.horizontal = false;

        return root;
    }

    public static GameObject CreateShadowedPanel(string name, Transform parent, Color bgColor, float cornerRadius, out Image bg, out RectTransform inner)
    {
        var root = CreatePanel(name, parent, new Color(1,1,1,0));
        var innerPanel = CreatePanel("Background", root.transform, bgColor);
        inner = innerPanel.GetComponent<RectTransform>();
        StretchToParent(inner, new RectOffset(0,0,0,0));

        var img = innerPanel.GetComponent<Image>();
        img.type = Image.Type.Sliced; // cần 9-slice sprite nếu có

        var shadow = innerPanel.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(0,6);
        shadow.effectColor = new Color(0,0,0,0.35f);

        bg = img;
        return root;
    }

    public static GameObject CreateChipButton(string name, Transform parent, string label, Color color, out Button btn)
    {
        var go = CreateButton(name, parent, label, out btn);
        var img = go.GetComponent<Image>(); img.color = color;
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 96);
        var text = go.transform.Find("Label").GetComponent<TextMeshProUGUI>(); text.fontSize = 44;
        return go;
    }

    public static GameObject CreateIconButton(string name, Transform parent, string label, out Button btn)
    {
        var go = CreateButton(name, parent, label, out btn);
        var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 112);
        var le = go.AddComponent<LayoutElement>(); le.preferredWidth = 0; le.flexibleWidth = 1;
        return go;
    }

    public static GameObject CreateComicPanel(string name, Transform parent, Vector2 size, string title, string body, out RectTransform contentArea)
    {
        var modal = CreateShadowedPanel(name, parent, new Color(0.13f,0.14f,0.18f,1f), 32f, out _, out var inner);
        var rt = modal.GetComponent<RectTransform>();
        SetRectTransform(rt, anchorMin:new Vector2(0.5f,0.5f), anchorMax:new Vector2(0.5f,0.5f), pivot:new Vector2(0.5f,0.5f), sizeDelta:size);

        var titleTMP = CreateTMP("Title", inner, title, 64, TextAlignmentOptions.Center, Color.white, out _);
        SetRectTransform(titleTMP.GetComponent<RectTransform>(), anchorMin:new Vector2(0,1), anchorMax:new Vector2(1,1), pivot:new Vector2(0.5f,1f),
            sizeDelta:new Vector2(0,140), anchoredPosition:new Vector2(0,-16));

        var bodyTMP = CreateTMP("Body", inner, body, 40, TextAlignmentOptions.TopJustified, new Color(1,1,1,0.9f), out _);
        SetRectTransform(bodyTMP.GetComponent<RectTransform>(), anchorMin:new Vector2(0,1), anchorMax:new Vector2(1,1), pivot:new Vector2(0.5f,1f),
            sizeDelta:new Vector2(0,260), anchoredPosition:new Vector2(0,-160));

        var scroll = CreateScrollView("Choices", inner, out var content);
        SetRectTransform(scroll.GetComponent<RectTransform>(), anchorMin:new Vector2(0,0), anchorMax:new Vector2(1,0), pivot:new Vector2(0.5f,0),
            sizeDelta:new Vector2(0,750), anchoredPosition:new Vector2(0,40));

        contentArea = content;
        return modal;
    }

    public static void SetRectTransform(RectTransform rt, Vector2? anchorMin=null, Vector2? anchorMax=null, Vector2? pivot=null, Vector2? sizeDelta=null, Vector2? anchoredPosition=null)
    {
        if (anchorMin.HasValue) rt.anchorMin = anchorMin.Value;
        if (anchorMax.HasValue) rt.anchorMax = anchorMax.Value;
        if (pivot.HasValue) rt.pivot = pivot.Value;
        if (sizeDelta.HasValue) rt.sizeDelta = sizeDelta.Value;
        if (anchoredPosition.HasValue) rt.anchoredPosition = anchoredPosition.Value;
    }

    public static void StretchToParent(RectTransform rt, RectOffset padding)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f,0.5f);
        rt.offsetMin = new Vector2(padding.left,  padding.bottom);
        rt.offsetMax = new Vector2(-padding.right, -padding.top);
    }

    static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;
        var parts = folderPath.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }

    static void SaveAsPrefab(GameObject go, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(go, path);
        AssetDatabase.Refresh();
    }

    public static void AssignSerialized(UnityEngine.Object targetComponent, string propertyName, UnityEngine.Object value)
    {
        if (!targetComponent) { Debug.LogWarning($"AssignSerialized: target null for {propertyName}"); return; }
        var so = new SerializedObject(targetComponent);
        var sp = so.FindProperty(propertyName);
        if (sp == null)
        {
            Debug.LogWarning($"Property '{propertyName}' not found on {targetComponent.GetType().Name}.");
            return;
        }
        sp.objectReferenceValue = value;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetComponent);
    }

    // ===== Non-generic helpers (tránh phụ thuộc type compile-time) =====
    GameObject CreatePanelWithComponent(string goName, Transform parent, string componentTypeName, out Component script)
    {
        var go = CreatePanel(goName, parent, PanelColor);
        StretchToParent(go.GetComponent<RectTransform>(), new RectOffset(56,56,16,16));
        script = go.AddOrGetComponentByName(componentTypeName);
        return go;
    }

    Component GetComp(GameObject go, string typeName)
    {
        var c = go.GetComponentByName(typeName);
        if (c == null) Debug.LogWarning($"Component '{typeName}' not present on {go.name}");
        return c;
    }

    GameObject CreateEventModal(Transform parent, GameObject choiceBtnPrefab)
    {
        var modal = CreateComicPanel("EventModal", parent, new Vector2(900,1400), "Event", "Something happens...", out var content);
        for (int i = 0; i < 3; i++)
        {
            var item = (GameObject)PrefabUtility.InstantiatePrefab(choiceBtnPrefab);
            item.name = $"Choice_{i+1}";
            item.transform.SetParent(content, false);
        }
        modal.AddOrGetComponentByName("EventModal");
        return modal;
    }

    GameObject CreateListModal(Transform parent, string goName, string componentTypeName, GameObject choiceBtnPrefab, out Component typedComp)
    {
        var modal = CreateComicPanel(goName, parent, new Vector2(900,1400), goName, "Pick one:", out var content);
        for (int i = 0; i < 8; i++)
        {
            var item = (GameObject)PrefabUtility.InstantiatePrefab(choiceBtnPrefab);
            item.name = $"Option_{i+1}";
            item.transform.SetParent(content, false);
        }
        typedComp = modal.AddOrGetComponentByName(componentTypeName);
        return modal;
    }

    GameObject CreateSimpleMessageModal(Transform parent, string goName, string componentTypeName, string message)
    {
        var modal = CreateComicPanel(goName, parent, new Vector2(900,1000), goName, message, out var content);
        var ok = CreateButton("OK", content, "OK", out _);
        modal.AddOrGetComponentByName(componentTypeName);
        return modal;
    }

    GameObject CreateLoanModal(Transform parent)
    {
        var modal = CreateComicPanel("LoanModal", parent, new Vector2(900,1200), "Loan", "Choose loan terms:", out var content);
        var rate = CreateTMP("Rate", content, "Interest: 5.5%", 44, TextAlignmentOptions.Left, Color.white, out _);
        var accept = CreateButton("Accept", content, "Accept", out _);
        var cancel = CreateButton("Cancel", content, "Cancel", out _);
        modal.AddOrGetComponentByName("LoanModal");
        return modal;
    }

    // prefab placeholder đơn giản
    void CreateSimpleUIPrefab(string dir, string name, string label)
    {
        var temp = CreateGameObject("Temp", null);
        var root = CreatePanel(name, temp.transform, new Color(1, 1, 1, 0.05f));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(800, 180);

        var txt = CreateTMP("Label", root.transform, label, 48, TextAlignmentOptions.Center, Color.white, out _);
        StretchToParent(txt.GetComponent<RectTransform>(), new RectOffset(16, 16, 16, 16));

        var path = System.IO.Path.Combine(dir, $"{name}.prefab").Replace("\\", "/");
        SaveAsPrefab(root, path);
        UnityEngine.Object.DestroyImmediate(temp);
    }
}

// ===== type utils (NO LINQ) =====
static class EditorTypeUtil
{
    public static Type FindTypeByName(string typeName)
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < asms.Length; i++)
        {
            Type[] types = null;
            try { types = asms[i].GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types; }
            if (types == null) continue;
            for (int j = 0; j < types.Length; j++)
            {
                var t = types[j];
                if (t == null) continue;
                if (string.Equals(t.Name, typeName, StringComparison.Ordinal) ||
                    string.Equals(t.FullName, typeName, StringComparison.Ordinal))
                    return t;
            }
        }
        return null;
    }
}

static class GameObjectByNameExtensions
{
    public static Component AddOrGetComponentByName(this GameObject go, string typeName)
    {
        var c = go.GetComponentByName(typeName);
        if (c != null) return c;
        return go.AddComponentByName(typeName);
    }

    public static Component AddComponentByName(this GameObject go, string typeName)
    {
        var t = EditorTypeUtil.FindTypeByName(typeName);
        if (t == null)
        {
            Debug.LogWarning($"[AddComponentByName] Type '{typeName}' not found. Create the runtime script or check the name.");
            return null;
        }
        return go.AddComponent(t);
    }

    public static Component GetComponentByName(this GameObject go, string typeName)
    {
        var comps = go.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            var c = comps[i];
            if (c == null) continue;
            var t = c.GetType();
            if (t.Name == typeName || t.FullName == typeName) return c;
        }
        return null;
    }
}

// ===== SafeArea fitter (nhẹ, chạy trong Editor không cắt) =====
public class SafeAreaFitter : MonoBehaviour
{
    void OnEnable() => Apply();
    void OnRectTransformDimensionsChange() => Apply();

    void Apply()
    {
#if UNITY_EDITOR
        var rt = GetComponent<RectTransform>();
        if (rt == null) return;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
#else
        var sa = Screen.safeArea;
        var canvas = GetComponentInParent<Canvas>();
        var rt = GetComponent<RectTransform>();
        if (canvas == null || rt == null) return;

        var scaler = canvas.rootCanvas.GetComponent<CanvasScaler>();
        Vector2 refRes = scaler ? scaler.referenceResolution : new Vector2(1080,2340);

        Vector2 min = sa.position;
        Vector2 max = sa.position + sa.size;
        float scaleX = refRes.x / Screen.width;
        float scaleY = refRes.y / Screen.height;

        rt.offsetMin = new Vector2(min.x * scaleX, min.y * scaleY);
        rt.offsetMax = new Vector2(-(Screen.width - max.x) * scaleX, -(Screen.height - max.y) * scaleY);
#endif
    }
}
#endif
