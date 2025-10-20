using UnityEngine;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Presentation.UI
{
    public class ModalManager : MonoBehaviour
    {
        [Header("Modal Prefabs")]
        [SerializeField] private EventModal eventModalPrefab;
        [SerializeField] private CareerChoiceModal careerChoiceModalPrefab;
        [SerializeField] private PromotionModal promotionModalPrefab;
        [SerializeField] private UniversityChoiceModal universityChoiceModalPrefab;
        [SerializeField] private MajorChoiceModal majorChoiceModalPrefab;
        [SerializeField] private SchoolChoiceModal schoolChoiceModalPrefab;
        [SerializeField] private SettingsModal settingsModalPrefab;
        [SerializeField] private AssetDetailModal assetDetailModalPrefab;
        [SerializeField] private BusinessPurchaseModal businessPurchaseModalPrefab;
        [SerializeField] private BusinessManagementModal businessManagementModalPrefab;

        // Active instances
        private EventModal _activeEventModal;
        private CareerChoiceModal _activeCareerChoiceModal;
        private PromotionModal _activePromotionModal;
        private UniversityChoiceModal _activeUniversityChoiceModal;
        private MajorChoiceModal _activeMajorChoiceModal;
        private SchoolChoiceModal _activeSchoolChoiceModal;
        private SettingsModal _activeSettingsModal;
        private AssetDetailModal _activeAssetModal;
        private BusinessPurchaseModal _activePurchaseModal;
        private BusinessManagementModal _activeManagementModal;

        public void SyncWithState(GameEngine engine, GameState state, ILocalization loc)
        {
            // Event Modal
            HandleModal(state.activeEvent, ref _activeEventModal, eventModalPrefab, (modal, pending) => {
                Character character = state.familyMembers[pending.characterId];
                modal.Show(pending, character, engine, loc);
            });

            // Career Choice Modal
            HandleModal(state.pendingCareerChoice, ref _activeCareerChoiceModal, careerChoiceModalPrefab, (modal, pending) => {
                modal.Bind(engine, loc);
                modal.Refresh(state);
                modal.Open();
            });

            // University Choice Modal
            HandleModal(state.pendingUniversityChoice, ref _activeUniversityChoiceModal, universityChoiceModalPrefab, (modal, pending) => {
                modal.Bind(engine, loc);
                modal.Refresh(state);
                modal.Open();
            });

            // Major Choice Modal
            HandleModal(state.pendingMajorChoice, ref _activeMajorChoiceModal, majorChoiceModalPrefab, (modal, pending) => {
                modal.Bind(engine, loc);
                modal.Refresh(state);
                modal.Open();
            });

            // School Choice Modal (Note: pendingSchoolChoice is a List)
            if (state.pendingSchoolChoice.Count > 0 && _activeSchoolChoiceModal == null)
            {
                _activeSchoolChoiceModal = Instantiate(schoolChoiceModalPrefab, transform);
                _activeSchoolChoiceModal.Bind(engine, loc);
                _activeSchoolChoiceModal.Refresh(state);
                _activeSchoolChoiceModal.Open();
            }
            else if (state.pendingSchoolChoice.Count == 0 && _activeSchoolChoiceModal != null)
            {
                _activeSchoolChoiceModal.Close();
                Destroy(_activeSchoolChoiceModal.gameObject);
                _activeSchoolChoiceModal = null;
            }

            // Promotion Modal
            HandleModal(state.pendingPromotion, ref _activePromotionModal, promotionModalPrefab, (modal, pending) => {
                modal.Bind(engine, loc);
                if (state.familyMembers.TryGetValue(pending.characterId, out var character) && LifeSim.Core.Data.Database.Careers.TryGetValue(character.careerTrackId, out var track))
                {
                    if (character.careerLevel < track.levels.Count)
                    {
                        var newLevelInfo = track.levels[character.careerLevel];
                        modal.Setup("promotion.title", pending.newTitleKey, loc.T(newLevelInfo.titleKey));
                    }
                }
                modal.Open();
            });
        }

        private void HandleModal<TModal, TState>(TState pendingState, ref TModal activeModal, TModal prefab, System.Action<TModal, TState> showAction) where TModal : Component
        {
            if (pendingState != null && activeModal == null)
            {
                activeModal = Instantiate(prefab, transform);
                showAction(activeModal, pendingState);
            }
            else if (pendingState == null && activeModal != null)
            {
                if (activeModal is ModalBase mb) { mb.Close(); }
                Destroy(activeModal.gameObject);
                activeModal = null;
            }
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
            if (_activeSettingsModal == null)
            {
                _activeSettingsModal = Instantiate(settingsModalPrefab, transform);
                _activeSettingsModal.Bind(engine, loc);
            }
            _activeSettingsModal.Open();
        }
    }
}