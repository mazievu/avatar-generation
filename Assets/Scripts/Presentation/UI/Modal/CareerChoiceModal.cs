using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Data.SO; // Changed
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class CareerChoiceModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] AgeAwareAvatarPreview avatarPreview;
        [SerializeField] Transform listRoot;
        [SerializeField] Button optionButtonPrefab;
        [SerializeField] TMP_Text footerNote;

        [Header("Underqualified Panel (optional)")]
        [SerializeField] UnderqualifiedChoiceModal underqualifiedModal;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l){ _engine=e; _loc=l; }

        public void Refresh(GameState s)
        {
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                avatarPreview.Render(me.GetAvatarState());
            }

            Clear(listRoot);
            var tracks = Database.Careers.Values;
            foreach (var t in tracks)
            {
                var btn = Object.Instantiate(optionButtonPrefab, listRoot);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt) txt.text = $"{_loc.T(t.trackNameKey)} • {_loc.T("career.req")}: IQ≥{t.requiredIq}, EQ≥{t.requiredEq}";
                var cap = t;
                btn.onClick.AddListener(()=>Choose(cap));
            }
            if (footerNote) footerNote.text = _loc.T("career.footerNote");
        }

        void Choose(CareerSO t)
        {
            if (!_engine.State.familyMembers.TryGetValue("me", out var me)) return;
            if (me.stats.iq < t.requiredIq || me.stats.eq < t.requiredEq)
            {
                if (underqualifiedModal)
                {
                    underqualifiedModal.Bind(_engine, _loc);
                    underqualifiedModal.SetMessage("career.underqualified");
                    underqualifiedModal.Open();
                }
                return;
            }

            me.careerTrackId = t.name;
            me.careerLevel = 0;
            Close();
            _engine.Save();
        }

        void Clear(Transform root){ for (int i=root.childCount-1;i>=0;i--) Object.Destroy(root.GetChild(i).gameObject); }

        public override void Open(){ if(panel) panel.Open(); }
        public override void Close(){ if(panel) panel.Close(); }
    }
}
