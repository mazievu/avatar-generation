using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Bind by Bootstrap")]
        public GameEngine engine;
        public ILocalization loc;

        [Header("Header")]
        [SerializeField] TMP_Text txtYear;
        [SerializeField] TMP_Text txtFund;
        [SerializeField] Button btnSettings;

        [Header("Bottom Nav")]
        [SerializeField] Button btnTree;
        [SerializeField] Button btnLog;
        [SerializeField] Button btnAssets;
        [SerializeField] Button btnBusiness;
        [SerializeField] Button btnPath;

        [Header("Panels")]
        [SerializeField] GameObject panelTree;
        [SerializeField] GameObject panelLog;
        [SerializeField] GameObject panelAssets;
        [SerializeField] GameObject panelBusiness;
        [SerializeField] GameObject panelPath;

        [Header("Modal Manager")]
        [SerializeField] ModalManager modalManager;

        string _active = "tree";

        public void Bind(GameEngine e, ILocalization l)
        {
            engine = e; loc = l;
            if (engine != null) engine.OnStateChanged += OnStateChanged;
            LocalizeStaticLabels();
        }

        void OnDestroy(){ if (engine != null) engine.OnStateChanged -= OnStateChanged; }

        void Awake()
        {
            if (btnTree)     btnTree.onClick.AddListener(()=> Show("tree"));
            if (btnLog)      btnLog.onClick.AddListener(()=> Show("log"));
            if (btnAssets)   btnAssets.onClick.AddListener(()=> Show("assets"));
            if (btnBusiness) btnBusiness.onClick.AddListener(()=> Show("business"));
            if (btnPath)     btnPath.onClick.AddListener(()=> Show("path"));
            if (btnSettings) btnSettings.onClick.AddListener(()=> { if (modalManager) modalManager.OpenSettings(engine, loc); });
        }

        void Start(){ Show(_active); }

        void OnStateChanged(GameState s)
        {
            if (txtYear) txtYear.text = $"{loc?.T("ui.year") ?? "Year"} {s.currentDate.year}";
            if (txtFund) txtFund.text = $"{s.familyFund:n0}";
            if (modalManager) modalManager.SyncWithState(engine, s, loc);
        }

        void Show(string key)
        {
            _active = key;
            if (panelTree)     panelTree.SetActive(key=="tree");
            if (panelLog)      panelLog.SetActive(key=="log");
            if (panelAssets)   panelAssets.SetActive(key=="assets");
            if (panelBusiness) panelBusiness.SetActive(key=="business");
            if (panelPath)     panelPath.SetActive(key=="path");
        }

        void LocalizeStaticLabels()
        {
            // nếu có text cố định trên nav/header, set ở đây (ví dụ tooltip, label)
            // ví dụ: btnTree.GetComponentInChildren<TMP_Text>().text = loc.T("nav.tree");
        }
    }
}