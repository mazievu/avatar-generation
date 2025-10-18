// FILE: Assets/Scripts/Presentation/UI/MajorChoiceModal.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // <-- THÊM DÒNG NÀY để sử dụng .Where() và .ToList()
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Data.SO; // <-- THÊM DÒNG NÀY để nhận diện EducationSO
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters; // <-- THÊM DÒNG NÀY để nhận diện Character

namespace LifeSim.Presentation.UI
{
    public class MajorChoiceModal : ModalBase
    {
        [SerializeField] private ComicPanel panel;
        [SerializeField] private Transform listRoot;
        [SerializeField] private Button optionButtonPrefab;
        [SerializeField] private TMP_Text footerNote;

        private GameEngine _engine; 
        private ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l) { _engine = e; _loc = l; }

        public void Refresh(GameState s)
        {
            Clear(listRoot);

            // --- SỬA LỖI 1: LẤY DỮ LIỆU TỪ DATABASE MỚI ---
            // Lấy tất cả giá trị từ Dictionary, sau đó lọc ra những cái là UniversityMajor
            var majors = Database.EducationOptions.Values
                                 .Where(edu => edu.type == EducationSO.EducationType.UniversityMajor)
                                 .ToList();
            
            foreach (var majorSO in majors) // Đổi tên biến để rõ ràng hơn
            {
                var btn = Instantiate(optionButtonPrefab, listRoot);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt)
                {
                    // Giả sử EducationSO có thuộc tính NameKey
                    txt.text = $"{_loc.T(majorSO.NameKey)}"; 
                }
                
                // Tạo một bản sao của biến để tránh lỗi closure trong lambda
                var capturedMajor = majorSO; 
                btn.onClick.AddListener(() => Choose(capturedMajor));
            }

            if (footerNote)
            {
                footerNote.text = _loc.T("major.footerNote");
            }
        }

        // Sửa kiểu dữ liệu của tham số thành EducationSO
        void Choose(EducationSO major) 
        {
            // --- SỬA LỖI 2: SỬA THUỘC TÍNH CỦA CHARACTER ---
            if (_engine.State.familyMembers.TryGetValue("me", out var me))
            {
                // Gán ID của chuyên ngành (tên của ScriptableObject) vào `educationMajorId`
                // Giả sử EducationSO có thuộc tính Id hoặc dùng `major.name`
                me.educationMajorId = major.name; 
            }
            
            Close();
            
            // --- SỬA LỖI 3: GỌI HÀM SAVE() ---
            _engine.Save();
        }

        void Clear(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }

        public override void Open() { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}