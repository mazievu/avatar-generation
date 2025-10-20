using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Services;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI
{
    public class CandidateSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text candidateNameText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private Button selectButton;

        public void Setup(Character candidate, ILocalization loc, System.Action<string> onSelected)
        {
            candidateNameText.text = candidate.name;
            int age = candidate.ageDays / Constants.DaysPerYear;
            statsText.text = loc.T("ui.candidate.stats", "Age: {0}, Skill: {1}", age, candidate.stats.skill);

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke(candidate.id));
        }
    }
}
