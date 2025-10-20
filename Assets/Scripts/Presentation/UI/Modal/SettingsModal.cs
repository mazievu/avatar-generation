using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class SettingsModal : ModalBase
    {
        [Header("Panel")][SerializeField] private ComicPanel panel;

        [Header("Buttons")]
        [SerializeField] private Button speedSlowButton;
        [SerializeField] private Button speedNormalButton;
        [SerializeField] private Button speedFastButton;
        [SerializeField] private Button speedVeryFastButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button switchLanguageButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button closeButton;

        [Header("Text")][SerializeField] private TMP_Text pauseButtonText;

        private GameEngine _engine;
        private ILocalization _loc;

        public void Bind(GameEngine engine, ILocalization loc)
        {
            _engine = engine;
            _loc = loc;
        }

        void Awake()
        {
            // Time Scale
            speedSlowButton?.onClick.AddListener(() => _engine.SetTimeScale(0.5f));
            speedNormalButton?.onClick.AddListener(() => _engine.SetTimeScale(1.0f));
            speedFastButton?.onClick.AddListener(() => _engine.SetTimeScale(2.0f));
            speedVeryFastButton?.onClick.AddListener(() => _engine.SetTimeScale(4.0f));

            // Actions
            pauseButton?.onClick.AddListener(() => _engine.ToggleManualPause());
            switchLanguageButton?.onClick.AddListener(() => _engine.SwitchLanguage());
            exitButton?.onClick.AddListener(() => Application.Quit());
            closeButton?.onClick.AddListener(Close);
        }

        void Update()
        {
            if (_engine == null || _loc == null || pauseButtonText == null) return;

            // Update pause button text
            if (_engine.State.isManuallyPaused)
            {
                pauseButtonText.text = _loc.T("settings.resume", "Resume");
            }
            else
            {
                pauseButtonText.text = _loc.T("settings.pause", "Pause");
            }
        }

        public override void Open()
        {
            if (panel) panel.Open();
            gameObject.SetActive(true);
        }

        public override void Close()
        { 
            if (panel) panel.Close();
            // For externally managed modals, we just hide it.
            // The manager will destroy it if needed.
            gameObject.SetActive(false);
        }
    }
}
