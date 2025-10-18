
using UnityEngine;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;
using LifeSim.Core.Data.SO;

namespace LifeSim.Presentation.UI
{
    public class ModalManager : MonoBehaviour
    {
        [Header("Modal Prefabs")]
        [SerializeField] private EventModal eventModalPrefab;
        [SerializeField] private AssetDetailModal assetDetailModalPrefab;
        [SerializeField] private BusinessPurchaseModal businessPurchaseModalPrefab;
        [SerializeField] private BusinessManagementModal businessManagementModalPrefab;

        private EventModal _activeEventModal;
        private AssetDetailModal _activeAssetModal;
        private BusinessPurchaseModal _activePurchaseModal;
        private BusinessManagementModal _activeManagementModal;

        public void SyncWithState(GameEngine engine, GameState state, ILocalization loc)
        {
            // Handle Event Modal
            if (state.activeEvent != null && _activeEventModal == null)
            {
                _activeEventModal = Instantiate(eventModalPrefab, transform);
                _activeEventModal.Show(state.activeEvent, engine, loc);
            }
            else if (state.activeEvent == null && _activeEventModal != null)
            {
                Destroy(_activeEventModal.gameObject);
                _activeEventModal = null;
            }

            // TODO: Handle other modals based on game state (e.g., pending choices)
        }

        public void OpenAssetDetailModal(AssetSO asset, GameEngine engine, ILocalization loc)
        {
            if (_activeAssetModal != null) return;
            _activeAssetModal = Instantiate(assetDetailModalPrefab, transform);
            _activeAssetModal.Show(asset, engine, loc);
        }

        public void OpenBusinessPurchaseModal(BusinessSO business, GameEngine engine, ILocalization loc)
        {
            if (_activePurchaseModal != null) return;
            _activePurchaseModal = Instantiate(businessPurchaseModalPrefab, transform);
            _activePurchaseModal.Show(business, engine, loc);
        }

        public void OpenBusinessManagementModal(BusinessInstance instance, GameEngine engine, ILocalization loc)
        {
            if (_activeManagementModal != null) return;
            _activeManagementModal = Instantiate(businessManagementModalPrefab, transform);
            _activeManagementModal.Show(instance, engine, loc);
        }

        public void OpenSettings(GameEngine engine, ILocalization loc)
        {
            // TODO: Instantiate and show a settings modal
            Debug.Log("Settings button clicked, but modal not implemented yet.");
        }
    }
}
