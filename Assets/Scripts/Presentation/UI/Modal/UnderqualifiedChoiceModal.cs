using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class UnderqualifiedChoiceModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] AgeAwareAvatarPreview avatarPreview;
        [SerializeField] TMP_Text txtMessage;
        [SerializeField] Button btnOk;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l) { _engine = e; _loc = l; }
        public void SetMessage(string key) { if (txtMessage) txtMessage.text = _loc.T(key); }

        void Awake() { if (btnOk) btnOk.onClick.AddListener(() => { Close(); /* có thể đặt pending học bổ sung ở đây */ }); }

        public override void Open()
        {
            if (_engine.State.familyMembers.TryGetValue("me", out var me))
            {
                avatarPreview.Render(me.GetAvatarState());
            }
            if (panel) panel.Open();
        }
        public override void Close() { if (panel) panel.Close(); }
    }
}