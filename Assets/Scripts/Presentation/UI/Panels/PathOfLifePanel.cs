using UnityEngine;
using System.Linq;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class PathOfLifePanel : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private PathMilestoneUI milestonePrefab;
        [SerializeField] private TMP_Text txtTitle;

        private GameEngine _engine;
        private ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e;
            _loc = l;
            _engine.OnStateChanged += Refresh;
            RenderAllMilestones();
        }

        void OnDestroy()
        {
            if (_engine != null) _engine.OnStateChanged -= Refresh;
        }

        private void RenderAllMilestones()
        {
            if (_engine == null) return;
            foreach (Transform c in container) Destroy(c.gameObject);
            txtTitle.text = _loc.T("panel.path.title");

            var milestones = Database.PathOfLife.Values.OrderBy(m => m.childrenRequired);

            foreach (var milestoneSO in milestones)
            {
                var uiInstance = Instantiate(milestonePrefab, container);
                bool isClaimed = _engine.State.unlockedFeatures.Contains(milestoneSO.featureId);
                bool canClaim = _engine.State.totalChildrenBorn >= milestoneSO.childrenRequired;
                uiInstance.Setup(milestoneSO, canClaim, isClaimed, _engine, _loc);
            }
        }

        private void Refresh(GameState state)
        {
            // A simple refresh for now, could be optimized to only update changed milestones
            RenderAllMilestones();
        }
    }
}