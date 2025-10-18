
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Services;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI
{
    public class CharacterNode : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private Image avatarImage; // TODO: Hook up to a real avatar system
        [SerializeField] private Image highlightBorder;

        public Character CharacterData { get; private set; }

        public void Setup(Character character, ILocalization loc)
        {
            CharacterData = character;

            // Populate UI
            nameText.text = character.name;
            int age = character.ageDays / Constants.DaysPerYear;
            ageText.text = string.Format(loc.T("ui.age"), age);

            // TODO: Set avatarImage based on character's appearance data

            // TODO: Set highlightBorder.enabled based on whether this is the player character
        }
    }
}
