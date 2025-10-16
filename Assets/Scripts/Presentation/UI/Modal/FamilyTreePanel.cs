using UnityEngine;
using TMPro;
using System.Text;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI.Panels
{
    public class FamilyTreePanel : MonoBehaviour
    {
        [SerializeField] TMP_Text txtTree;
        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        { _engine = e; _loc = l; Refresh(); }

        public void Refresh()
        {
            if (_engine == null) return;
            var sb = new StringBuilder();
            sb.AppendLine(_loc.T("panel.family.title"));
            foreach (var kv in _engine.State.familyMembers)
                sb.AppendLine($"• {kv.Value.name} ({kv.Value.relation})");
            txtTree.text = sb.ToString();
        }
    }
}