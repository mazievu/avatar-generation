
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    [RequireComponent(typeof(Button))]
    public class AssetSlotUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private GameObject ownedIndicator;

        public AssetSO AssetData { get; private set; }
        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Setup(AssetSO asset, bool isOwned, GameEngine engine, ILocalization loc)
        {
            AssetData = asset;
            nameText.text = loc.T(asset.assetNameKey);
            UpdateOwnedStatus(isOwned);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => {
                // Find the ModalManager in the scene to open the detail modal
                var modalManager = FindObjectOfType<ModalManager>();
                if (modalManager != null)
                {
                    modalManager.OpenAssetDetailModal(asset, engine, loc);
                }
            });
        }

        public void UpdateOwnedStatus(bool isOwned)
        {
            if (ownedIndicator != null) ownedIndicator.SetActive(isOwned);
            // Disable button if already owned
            _button.interactable = !isOwned;
        }
    }
}
