// FILE: Assets/Scripts/Presentation/UI/Modal/PromotionModal.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters; // THÊM DÒNG NÀY

namespace LifeSim.Presentation.UI
{
    public class PromotionModal : ModalBase
    {
        [SerializeField] private ComicPanel panel;
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private TMP_Text txtBody;
        [SerializeField] private Button btnAccept;
        [SerializeField] private Button btnDecline;

        private GameEngine _engine; 
        private ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l) { _engine = e; _loc = l; }

        void Awake()
        {
            if (btnAccept) btnAccept.onClick.AddListener(Accept);
            if (btnDecline) btnDecline.onClick.AddListener(Close);
        }

        public void Setup(string titleKey, string bodyKey, params object[] args)
        {
            if (_loc == null) return;
            if (txtTitle) txtTitle.text = _loc.T(titleKey);
            if (txtBody) txtBody.text = _loc.T(bodyKey, args); // Sửa để nhận tham số
        }

        void Accept()
        {
            if (_engine.State.familyMembers.TryGetValue("me", out var me))
            {
                // --- SỬA LỖI TẠI ĐÂY ---
                me.careerLevel++;
            }
            Close();
            _engine.Save();
        }

        public override void Open() { if (panel) panel.Open(); }
        public override void Close() { if (panel) panel.Close(); }
    }
}