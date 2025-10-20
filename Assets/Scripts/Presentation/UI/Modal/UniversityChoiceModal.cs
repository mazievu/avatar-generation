// FILE: Assets/Scripts/Presentation/UI/Modal/UniversityChoiceModal.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // <-- THÊM DÒNG NÀY
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Data.SO; // <-- THÊM DÒNG NÀY
using LifeSim.Core.Domain.Education;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    // LƯU Ý: Dựa trên code, file này có vẻ giống hệt MajorChoiceModal.
    // Tôi sẽ sửa nó theo đúng logic của MajorChoiceModal.
    // Nếu nó chỉ là modal Yes/No để quyết định có học ĐH không, bạn cần báo lại.
    public class UniversityChoiceModal : ModalBase
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

            Clear(listRoot);

            // --- SỬA LỖI 1 TẠI ĐÂY ---
            // Lọc các chuyên ngành từ `EducationOptions`
            var options = Database.EducationOptions.Values
                .Where(edu => edu.type == EducationSO.EducationType.UniversityMajor)
                .ToList();

            foreach (var major in options)
            {
                var btn = Instantiate(optionButtonPrefab, listRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = $"{_loc.T(major.NameKey)} • {_loc.T("ui.cost")}: {major.Cost:n0}";
                
                var cap = major;
                btn.onClick.AddListener(() => Choose(cap));
            }

            if (footerNote) footerNote.text = _loc.T("university.footerNote");
        }

        // Sửa kiểu tham số thành EducationSO
        void Choose(EducationSO major)
        {
            var s = _engine.State;
            if (s.familyMembers.TryGetValue("me", out var me))
            {
                s.familyFund -= major.Cost;
                
                // --- SỬA LỖI 2 TẠI ĐÂY ---
                // Gán ID của chuyên ngành vào `educationMajorId`
                me.educationMajorId = major.name; // `major.name` là ID của ScriptableObject
            }
            Close();
            _engine.Save();
        }

        void Clear(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--) Destroy(root.GetChild(i).gameObject);
        }

        public override void Open() { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}