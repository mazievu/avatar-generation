using UnityEngine;
using System.Collections.Generic;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI.Panels
{
    public class AssetsPanel : MonoBehaviour
    {
        [Header("Prefab & Container")]
        [SerializeField] private AssetSlotUI assetSlotPrefab;
        [SerializeField] private Transform container;

        private GameEngine _engine;
        private ILocalization _loc;
        private List<AssetSlotUI> _slots = new List<AssetSlotUI>();

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e;
            _loc = l;
            // Initial render
            RenderAllAssets();
            // Subsequent updates will be handled by Refresh
            _engine.OnStateChanged += Refresh;
        }

        void OnDestroy()
        {
            if (_engine != null) _engine.OnStateChanged -= Refresh;
        }

        private void RenderAllAssets()
        {
            foreach (var slot in _slots) { Destroy(slot.gameObject); }
            _slots.Clear();

            foreach (var assetSO in Database.Assets.Values)
            {
                AssetSlotUI instance = Instantiate(assetSlotPrefab, container);
                bool isOwned = _engine.State.assets.Contains(assetSO.name);
                instance.Setup(assetSO, isOwned, _engine, _loc);
                _slots.Add(instance);
            }
        }

        public void Refresh(GameState state)
        {
            // For now, we just update the owned status of each slot.
            // A full re-render is also an option but less efficient.
            foreach (var slot in _slots)
            {
                bool isOwned = state.assets.Contains(slot.AssetData.name);
                slot.UpdateOwnedStatus(isOwned);
            }
        }
    }
}
