
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
        [SerializeField] private TMP_Text businessIncomeText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform employeeContainer;
        [SerializeField] private EmployeeSlotUI employeeSlotPrefab;

        [Header("Child Modals")]
        [SerializeField] private EmployeeSelectionModal employeeSelectionModal;

        private BusinessInstance _currentInstance;
        private GameEngine _engine;
        private ILocalization _loc;

        public void Show(BusinessInstance instance, GameEngine engine, ILocalization loc)
        {
            _currentInstance = instance;
            _engine = engine;
            _loc = loc;

            if (!Database.Businesses.TryGetValue(instance.businessId, out var businessSO))
            {
                Destroy(gameObject);
                return;
            }

            // Header
            nameText.text = loc.T(businessSO.businessNameKey, businessSO.name);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => Destroy(gameObject));

            // Body
            tierText.text = loc.T("ui.business.tier", "Tier {0}", instance.tier);
            RefreshEmployeeSlots();

            // Footer
            businessIncomeText.text = loc.T("ui.business.income", "Income: {0} / mo", engine.GetBusinessIncome(instance.instanceId));

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
        }

        private void RefreshEmployeeSlots()
        {
            foreach (Transform child in employeeContainer) { Destroy(child.gameObject); }
            for (int i = 0; i < _currentInstance.employeeIds.Count; i++)
            {
                var slotInstance = Instantiate(employeeSlotPrefab, employeeContainer);
                slotInstance.Setup(_currentInstance, i, _engine, _loc, OnChangeEmployeeClicked);
            }
        }

        private void OnChangeEmployeeClicked(int slotIndex)
        {
            // Open the selection modal
            if (employeeSelectionModal != null)
            {
                employeeSelectionModal.Show(_currentInstance.instanceId, slotIndex);
            }
            else
            {
                Debug.LogError("EmployeeSelectionModal is not assigned in the inspector!");
            }
        }
    }
}
