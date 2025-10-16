using UnityEngine;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class AssetsPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text txtFund;
        [SerializeField] TMP_Text txtSummary;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        { _engine = e; _loc = l; Refresh(); }

        public void Refresh()
        {
            if (_engine == null) return;
            var s = _engine.State;
            txtFund.text = $"{_loc.T("ui.fund")}: ${s.familyFund:n0}";
            txtSummary.text = $"{_loc.T("ui.assetsCount")}: {s.purchasedAssets.Count}";
        }
    }
}