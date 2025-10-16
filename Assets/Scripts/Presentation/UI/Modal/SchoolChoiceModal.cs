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
    public class SchoolChoiceModal : ModalBase
    {
        [Header("View")]
        [SerializeField] ComicPanel panel;                 // sử dụng ComicPanel để mở/đóng
        [SerializeField] Transform listRoot;               // VerticalLayout
        [SerializeField] Button optionButtonPrefab;        // prefab 1 dòng (Label + Cost)
        [SerializeField] TMP_Text footerNote;              // text giải thích

        private GameEngine _engine;
        private ILocalization _loc;

        public void Bind(GameEngine engine, ILocalization loc)
        {
            _engine = engine; _loc = loc;
        }

        public void Refresh(GameState s)
        {
            if (listRoot == null || optionButtonPrefab == null) return;
            ClearChildren(listRoot);

            // lấy danh sách school options từ Database
            List<SchoolOption> options = Database.SchoolOptions ?? new List<SchoolOption>();
            foreach (var opt in options)
            {
                var btn = Instantiate(optionButtonPrefab, listRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = $"{_loc.T(opt.nameKey)}  •  {_loc.T("ui.cost")}: {opt.cost:n0}";

                var captured = opt;
                btn.onClick.AddListener(() => OnChooseSchool(captured));
            }

            if (footerNote) footerNote.text = _loc.T("school.footerNote");
        }

        private void OnChooseSchool(SchoolOption opt)
        {
            // Tối thiểu: trừ tiền + đẩy effect vào nhân vật chính (demo)
            var s = _engine.State;

            // giả sử character "me" là chủ thể đang đi học
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                s.familyFund -= opt.cost;
                // áp effect đơn giản (iq/eq/...)
                EducationService.ApplySchoolEffects(me, opt);
            }

            // clear pending school choice nếu bạn dùng pending state (ở GameState)
            s.pendingSchoolChoice?.Clear();

            // đóng modal + phát event state change
            Close();
            _engine.Save(); // Save and emit
        }

        private void ClearChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                GameObject.Destroy(root.GetChild(i).gameObject);
        }

        public override void Open()  { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}