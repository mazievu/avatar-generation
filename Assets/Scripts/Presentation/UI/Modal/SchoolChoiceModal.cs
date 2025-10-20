// FILE: Assets/Scripts/Presentation/UI/Modal/SchoolChoiceModal.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // <-- THÊM DÒNG NÀY
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Data.SO; // <-- THÊM DÒNG NÀY
using LifeSim.Core.Domain.Education; // Cần cho EducationService (nếu có)
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class SchoolChoiceModal : ModalBase
    {
        [Header("View")]
        [SerializeField] private ComicPanel panel;
        [SerializeField] private AgeAwareAvatarPreview avatarPreview;
        [SerializeField] private Transform listRoot;
        [SerializeField] private Button optionButtonPrefab;
        [SerializeField] private TMP_Text footerNote;

        private GameEngine _engine;
        private ILocalization _loc;

        public void Bind(GameEngine engine, ILocalization loc)
        {
            _engine = engine; _loc = loc;
        }

        public void Refresh(GameState s)
        {
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                avatarPreview.Render(me.GetAvatarState());
            }

            if (listRoot == null || optionButtonPrefab == null) return;
            ClearChildren(listRoot);

            // --- SỬA LỖI TẠI ĐÂY ---
            // Thay vì `Database.SchoolOptions`, chúng ta lọc từ `EducationOptions`
            var options = Database.EducationOptions.Values
                .Where(edu => edu.type == EducationSO.EducationType.School)
                .ToList();

            foreach (var opt in options)
            {
                var btn = Instantiate(optionButtonPrefab, listRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = $"{_loc.T(opt.NameKey)}  •  {_loc.T("ui.cost")}: {opt.Cost:n0}";

                var captured = opt;
                btn.onClick.AddListener(() => OnChooseSchool(captured));
            }

            if (footerNote) footerNote.text = _loc.T("school.footerNote");
        }

        // Sửa kiểu tham số thành EducationSO
        private void OnChooseSchool(EducationSO opt)
        {
            var s = _engine.State;
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                s.familyFund -= opt.Cost;
                // Giả sử bạn có một lớp EducationService để áp dụng hiệu ứng
                // EducationService.ApplySchoolEffects(me, opt); 
                // Nếu không, bạn cần thêm logic áp dụng hiệu ứng ở đây.
            }

            s.pendingSchoolChoice?.Clear();
            Close();
            _engine.Save();
        }

        private void ClearChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                GameObject.Destroy(root.GetChild(i).gameObject);
        }

        public override void Open() { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}