using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI
{
    public class LoanModal : ModalBase
    {
        [SerializeField] ComicPanel panel;
        [SerializeField] TMP_InputField inputAmount;
        [SerializeField] TMP_InputField inputTerm;
        [SerializeField] Button btnConfirm;
        [SerializeField] TMP_Text txtNote;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l){ _engine=e; _loc=l; }

        void Awake()
        {
            if (btnConfirm) btnConfirm.onClick.AddListener(Confirm);
        }

        void Confirm()
        {
            if (_engine == null) return;
            if (float.TryParse(inputAmount.text, out var amount))
            {
                var s = _engine.State;
                s.familyFund += (int)amount;
                // bạn có thể lưu kỳ hạn, lãi suất...
                txtNote.text = _loc.T("loan.confirmed");
                _engine.Save();
            }
            Close();
        }

        public override void Open(){ if(panel) panel.Open(); }
        public override void Close(){ if(panel) panel.Close(); }
    }
}
