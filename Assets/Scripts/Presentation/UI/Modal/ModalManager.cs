using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class ModalManager : MonoBehaviour
    {
        [Header("Event Modal")]
        [SerializeField] ComicPanel eventPanel;
        [SerializeField] Transform eventChoicesRoot;
        [SerializeField] Button eventChoiceButtonPrefab;
        [SerializeField] UniversityChoiceModal universityModal;
        [SerializeField] MajorChoiceModal majorModal;
        [SerializeField] CareerChoiceModal careerModal;
        [SerializeField] LoanModal loanModal;
        [SerializeField] PromotionModal promotionModal;

        [Header("Settings Modal")]
        [SerializeField] ComicPanel settingsPanel;

        [Header("School Modal")]
        [SerializeField] SchoolChoiceModal schoolModal;

        [Header("Underqualified Modal")]
        [SerializeField] UnderqualifiedChoiceModal underqualifiedModal;

        public void SyncWithState(GameEngine engine, GameState s, ILocalization loc)
        {
            // Event modal
            if (s.activeEvent != null)
            {
                if (eventPanel && !eventPanel.gameObject.activeSelf)
                {
                    eventPanel.SetText(loc.T(s.activeEvent.titleKey), loc.T(s.activeEvent.bodyKey));
                    BuildEventChoices(engine, s, loc);
                    eventPanel.Open();
                }
            }
            else
            {
                if (eventPanel && eventPanel.gameObject.activeSelf)
                {
                    Clear(eventChoicesRoot);
                    eventPanel.Close();
                }
            }

            // School choice (nếu state có pending)
            if (s.pendingSchoolChoice != null && s.pendingSchoolChoice.Count > 0)
            {
                if (schoolModal && !schoolModal.gameObject.activeSelf)
                {
                    schoolModal.Bind(engine, loc);
                    schoolModal.Refresh(s);
                    schoolModal.Open();
                }
            }
            else
            {
                if (schoolModal && schoolModal.gameObject.activeSelf)
                    schoolModal.Close();
            }

            // TODO: Bước kế tiếp sẽ xử lý các pending modal khác (University/...).
            if (s.pendingUniversityChoice != null) { universityModal.Bind(engine,loc); universityModal.Refresh(s); universityModal.Open(); }
if (s.pendingMajorChoice != null)      { majorModal.Bind(engine,loc); majorModal.Refresh(s); majorModal.Open(); }
if (s.pendingCareerChoice != null)     { careerModal.Bind(engine,loc); careerModal.Refresh(s); careerModal.Open(); }
if (s.pendingLoanChoice != null)        { loanModal.Bind(engine,loc); loanModal.Open(); }
if (s.pendingPromotion != null)   { promotionModal.Bind(engine,loc); promotionModal.Setup("promotion.title","promotion.body"); promotionModal.Open(); }

        }

        public void OpenSettings(GameEngine engine, ILocalization loc)
        {
            if (settingsPanel)
            {
                settingsPanel.SetText(loc.T("settings.title"), loc.T("settings.body"));
                settingsPanel.Open();
            }
        }

        void BuildEventChoices(GameEngine engine, GameState s, ILocalization loc)
        {
            Clear(eventChoicesRoot);
            if (!eventChoicesRoot || !eventChoiceButtonPrefab || s.activeEvent == null) return;

            foreach (var ch in s.activeEvent.choices)
            {
                var btn = Instantiate(eventChoiceButtonPrefab, eventChoicesRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = loc.T(ch.labelKey);
                var id = ch.id;
                btn.onClick.AddListener(()=> engine.HandleEventChoice(id));
            }
        }

        void Clear(Transform root)
        {
            if (!root) return;
            for (int i = root.childCount-1; i>=0; i--) Destroy(root.GetChild(i).gameObject);
        }
    }
}