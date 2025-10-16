using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Domain.Education;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI
{
    public class MajorChoiceModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] Transform listRoot;
        [SerializeField] Button optionButtonPrefab;
        [SerializeField] TMP_Text footerNote;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l){ _engine=e; _loc=l; }

        public void Refresh(GameState s)
        {
            Clear(listRoot);
            var majors = Database.UniversityMajors ?? new List<UniversityMajor>();
            foreach (var m in majors)
            {
                var btn = Object.Instantiate(optionButtonPrefab, listRoot);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt) txt.text = $"{_loc.T(m.nameKey)}";
                var cap = m;
                btn.onClick.AddListener(()=>Choose(cap));
            }
            if (footerNote) footerNote.text = _loc.T("major.footerNote");
        }

        void Choose(UniversityMajor m)
        {
            if (_engine.State.familyMembers.TryGetValue("me", out var me))
                me.education.currentMajor = m.key;
            Close();
            _engine.Save();
        }

        void Clear(Transform root){ for (int i=root.childCount-1;i>=0;i--) Object.Destroy(root.GetChild(i).gameObject); }

        public override void Open(){ if(panel) panel.Open(); }
        public override void Close(){ if(panel) panel.Close(); }
    }
}
