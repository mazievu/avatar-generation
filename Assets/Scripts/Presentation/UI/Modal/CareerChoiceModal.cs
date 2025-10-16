using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Domain.Careers;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI
{
    public class CareerChoiceModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] Transform listRoot;
        [SerializeField] Button optionButtonPrefab;
        [SerializeField] TMP_Text footerNote;

        [Header("Underqualified Panel (optional)")]
        [SerializeField] UnderqualifiedChoiceModal underqualifiedModal;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l){ _engine=e; _loc=l; }

        public void Refresh(GameState s)
        {
            Clear(listRoot);
            var tracks = Database.CareerTracks ?? new List<CareerTrack>();
            foreach (var t in tracks)
            {
                var btn = Object.Instantiate(optionButtonPrefab, listRoot);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt) txt.text = $"{_loc.T(t.nameKey)} • {_loc.T("career.req")}: IQ≥{(t.ladder.Count > 0 ? t.ladder[0].requiredIQ : 0)}, EQ≥{(t.ladder.Count > 0 ? t.ladder[0].requiredEQ : 0)}";
                var cap = t;
                btn.onClick.AddListener(()=>Choose(cap));
            }
            if (footerNote) footerNote.text = _loc.T("career.footerNote");
        }

        void Choose(CareerTrack t)
        {
            if (!_engine.State.familyMembers.TryGetValue("me", out var me)) return;
            if (me.stats.iq < (t.ladder.Count > 0 ? t.ladder[0].requiredIQ : 0) || me.stats.eq < (t.ladder.Count > 0 ? t.ladder[0].requiredEQ : 0))
            {
                if (underqualifiedModal)
                {
                    underqualifiedModal.Bind(_engine, _loc);
                    underqualifiedModal.SetMessage("career.underqualified");
                    underqualifiedModal.Open();
                }
                return;
            }

            me.career.currentTrack = t.key;
            me.career.level = 0;
            Close();
            _engine.Save();
        }

        void Clear(Transform root){ for (int i=root.childCount-1;i>=0;i--) Object.Destroy(root.GetChild(i).gameObject); }

        public override void Open(){ if(panel) panel.Open(); }
        public override void Close(){ if(panel) panel.Close(); }
    }
}
