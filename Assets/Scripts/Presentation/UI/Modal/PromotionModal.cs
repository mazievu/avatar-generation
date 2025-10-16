using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI
{
    public class PromotionModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] TMP_Text txtTitle;
        [SerializeField] TMP_Text txtBody;
        [SerializeField] Button btnAccept;
        [SerializeField] Button btnDecline;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l){ _engine=e; _loc=l; }

        void Awake()
        {
            if (btnAccept) btnAccept.onClick.AddListener(Accept);
            if (btnDecline) btnDecline.onClick.AddListener(Close);
        }

        public void Setup(string titleKey, string bodyKey)
        {
            if (_loc==null) return;
            if (txtTitle) txtTitle.text = _loc.T(titleKey);
            if (txtBody) txtBody.text  = _loc.T(bodyKey);
        }

        void Accept()
        {
            if (_engine.State.familyMembers.TryGetValue("me", out var me))
                me.career.level++;
            Close();
            _engine.Save();
        }

        public override void Open(){ if(panel) panel.Open(); }
        public override void Close(){ if(panel) panel.Close(); }
    }
}
