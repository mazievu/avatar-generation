
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class BusinessPurchaseModal : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;

        public void Show(BusinessSO business, GameEngine engine, ILocalization loc)
        {
            var tier1 = business.tiers[0]; // Assume tier 1 for purchase

            nameText.text = loc.T(business.businessNameKey, business.name);
            descriptionText.text = loc.T(business.businessType, "(Type)"); // Using type as description for now
            priceText.text = $"Price: ${tier1.purchasePrice:n0}";

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => {
                engine.PurchaseBusiness(business.name);
                Destroy(gameObject);
            });

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => {
                Destroy(gameObject);
            });

            buyButton.interactable = engine.State.familyFund >= tier1.purchasePrice;
        }
    }
}
