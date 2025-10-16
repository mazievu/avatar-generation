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
    public class UniversityChoiceModal : ModalBase
    {
        [Header("View")]
        [SerializeField] ComicPanel panel;
        [SerializeField] Transform listRoot;
        [SerializeField] Button optionButtonPrefab;
        [SerializeField] TMP_Text footerNote;

        GameEngine _engine;
        ILocalization _loc;

        public void Bind(GameEngine engine, ILocalization loc)
        {
            _engine = engine; _loc = loc;
        }

        public void Refresh(GameState s)
        {
            Clear(listRoot);
            var options = Database.UniversityMajors ?? new List<UniversityMajor>();

            foreach (var major in options)
            {
                var btn = Object.Instantiate(optionButtonPrefab, listRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = $"{_loc.T(major.nameKey)} • {_loc.T("ui.cost")}: {major.cost:n0}";
                var cap = major;
                btn.onClick.AddListener(() => Choose(cap));
            }

            if (footerNote) footerNote.text = _loc.T("university.footerNote");
        }

        void Choose(UniversityMajor major)
        {
            var s = _engine.State;
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                s.familyFund -= major.cost;
                me.education.currentMajor = major.key;
            }
            Close();
            _engine.Save();
        }

        void Clear(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--) Object.Destroy(root.GetChild(i).gameObject);
        }

        public override void Open()  { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}
