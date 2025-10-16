using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class BusinessPanel : MonoBehaviour
    {
        [SerializeField] Transform listRoot;
        [SerializeField] Button businessButtonPrefab;
        [SerializeField] TMP_Text txtTitle;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        { _engine = e; _loc = l; Refresh(); }

        public void Refresh()
        {
            if (_engine == null) return;
            foreach (Transform c in listRoot) Destroy(c.gameObject);
            txtTitle.text = _loc.T("panel.business.title");

            foreach (var b in _engine.State.businesses)
            {
                var btn = Instantiate(businessButtonPrefab, listRoot);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                txt.text = $"{b.name} • ${b.income:n0}/y";
                var cap = b;
                btn.onClick.AddListener(() => View(cap));
            }
        }

        void View(BusinessInstance b)
        {
            Debug.Log($"Viewing business: {b.name}");
        }
    }
}