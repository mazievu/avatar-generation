
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class AssetDetailModal : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;

        public void Show(AssetSO asset, GameEngine engine, ILocalization loc)
        {
            nameText.text = loc.T(asset.assetNameKey, asset.name);
            descriptionText.text = loc.T(asset.descriptionKey, "(Description not available)");
            priceText.text = $"Price: ${asset.price:n0}";

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => {
                engine.PurchaseAsset(asset.name);
                Destroy(gameObject);
            });

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => {
                Destroy(gameObject);
            });

            // Disable buy button if player can't afford it
            buyButton.interactable = engine.State.familyFund >= asset.price;
        }
    }
}
