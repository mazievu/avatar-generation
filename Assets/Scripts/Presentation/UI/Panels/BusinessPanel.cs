using UnityEngine;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class BusinessPanel : MonoBehaviour
    {
        [Header("Prefab & Container")]
        [SerializeField] private BusinessHotspotUI hotspotPrefab;
        [SerializeField] private Transform container; // This would be the map panel

        private GameEngine _engine;
        private ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e;
            _loc = l;
            _engine.OnStateChanged += Refresh;
            RenderAllHotspots();
        }

        void OnDestroy()
        {
            if (_engine != null) _engine.OnStateChanged -= Refresh;
        }

        private void RenderAllHotspots()
        {
            if (_engine == null) return;
            foreach (Transform c in container) Destroy(c.gameObject);

            foreach (var businessSO in Database.Businesses.Values)
            {
                var hotspotInstance = Instantiate(hotspotPrefab, container);
                var ownedInstance = _engine.State.businesses.FirstOrDefault(b => b.businessId == businessSO.name);
                hotspotInstance.Setup(businessSO, ownedInstance, _engine, _loc);
            }
        }

        private void Refresh(GameState state)
        {
            // For now, a full re-render is simplest.
            RenderAllHotspots();
        }
    }
}
