
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI
{
    public class BusinessManagementModal : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text tierText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform employeeContainer;
        [SerializeField] private EmployeeSlotUI employeeSlotPrefab;

        public void Show(BusinessInstance instance, GameEngine engine, ILocalization loc)
        {
            if (!Database.Businesses.TryGetValue(instance.businessId, out var businessSO))
            {
                Destroy(gameObject);
                return;
            }

            nameText.text = loc.T(businessSO.businessNameKey, businessSO.name);
            tierText.text = loc.T("ui.business.tier", "Tier {0}", instance.tier);

            // Upgrade Button Logic
            var nextTier = businessSO.tiers.FirstOrDefault(t => t.tierLevel == instance.tier + 1);
            if (nextTier != null && engine.State.familyFund >= nextTier.upgradePrice)
            {
                upgradeButton.interactable = true;
                upgradeButton.onClick.RemoveAllListeners();
                upgradeButton.onClick.AddListener(() => {
                    engine.UpgradeBusiness(instance.instanceId);
                    Destroy(gameObject); // Close modal after action
                });
            }
            else
            {
                upgradeButton.interactable = false;
            }

            // Close Button
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => Destroy(gameObject));

            // Display Employee Slots
            foreach (Transform child in employeeContainer) { Destroy(child.gameObject); }
            foreach (var employeeId in instance.employeeIds)
            {
                var slotInstance = Instantiate(employeeSlotPrefab, employeeContainer);
                slotInstance.Setup(employeeId, engine, loc);
            }
        }
    }
}
