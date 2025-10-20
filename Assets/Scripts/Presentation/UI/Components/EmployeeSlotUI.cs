
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Data;
using System.Linq;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class EmployeeSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private AgeAwareAvatarPreview avatarPreview;
        [SerializeField] private TMP_Text employeeNameText;
        [SerializeField] private TMP_Text salaryText;
        [SerializeField] private Button changeEmployeeButton;

        public void Setup(BusinessInstance business, int slotIndex, GameEngine engine, ILocalization loc, System.Action<int> onChangeClicked)
        {
            string employeeId = business.employeeIds[slotIndex];

            // Set listener for the change button
            changeEmployeeButton.onClick.RemoveAllListeners();
            changeEmployeeButton.onClick.AddListener(() => onChangeClicked?.Invoke(slotIndex));

            if (string.IsNullOrEmpty(employeeId))
            {
                // Empty Slot
                avatarPreview.gameObject.SetActive(false);
                salaryText.gameObject.SetActive(false);
                employeeNameText.text = loc.T("ui.business.empty_slot", "Empty Slot");
            }
            else if (employeeId == "robot")
            {
                // Robot
                avatarPreview.gameObject.SetActive(false); // Or show a robot icon
                employeeNameText.text = loc.T("ui.business.robot", "Robot");
                if (Database.Businesses.TryGetValue(business.businessId, out var businessSO))
                {
                    var tier = businessSO.tiers.FirstOrDefault(t => t.tierLevel == business.tier);
                    if (tier != null)
                    {
                        salaryText.text = loc.T("ui.business.salary", "Salary: {0}", tier.robotMonthlyCost);
                        salaryText.gameObject.SetActive(true);
                    }
                }
            }
            else if (engine.State.familyMembers.TryGetValue(employeeId, out var employee))
            {
                // Family Member Employee
                avatarPreview.gameObject.SetActive(true);
                avatarPreview.Render(employee.GetAvatarState());
                employeeNameText.text = employee.name;

                // Calculate Salary
                if (Database.Businesses.TryGetValue(business.businessId, out var businessSO))
                {
                    var tier = businessSO.tiers.FirstOrDefault(t => t.tierLevel == business.tier);
                    if (tier != null)
                    {
                        int salary = employee.stats.skill * tier.employeeSalaryPerSkillPoint;
                        salaryText.text = loc.T("ui.business.salary", "Salary: {0} / mo", salary);
                        salaryText.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                // Error case
                avatarPreview.gameObject.SetActive(false);
                salaryText.gameObject.SetActive(false);
                employeeNameText.text = "Error: Unknown Employee";
            }
        }
    }
}
