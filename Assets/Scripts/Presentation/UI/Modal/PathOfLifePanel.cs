using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class PathOfLifePanel : MonoBehaviour
    {
        [SerializeField] Transform contentRoot;
        [SerializeField] Button featureButtonPrefab;
        [SerializeField] TMP_Text txtTitle;

        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        { _engine = e; _loc = l; Refresh(); }

        public void Refresh()
        {
            if (_engine == null) return;
            foreach (Transform c in contentRoot) Destroy(c.gameObject);
            txtTitle.text = _loc.T("panel.path.title");
            foreach (var f in Database.UnlockableFeatures)
            {
                var btn = Instantiate(featureButtonPrefab, contentRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                label.text = _loc.T(f.nameKey);
                var cap = f;
                btn.onClick.AddListener(() => Unlock(cap.id));
            }
        }

        void Unlock(string key)
        {
            _engine.State.claimedFeatures.Add(key);
            _engine.Save();
        }
    }
}