
using UnityEngine;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class EmployeeSlotUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text employeeNameText;

        public void Setup(string employeeId, GameEngine engine, ILocalization loc)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                employeeNameText.text = loc.T("ui.business.empty_slot");
            }
            else if (employeeId == "robot")
            {
                employeeNameText.text = loc.T("ui.business.robot");
            }
            else if (engine.State.familyMembers.TryGetValue(employeeId, out var employee))
            {
                employeeNameText.text = employee.name;
            }
            else
            {
                employeeNameText.text = "Error: Unknown Employee";
            }

            // TODO: Add a button to this slot to open an assignment modal.
        }
    }
}
