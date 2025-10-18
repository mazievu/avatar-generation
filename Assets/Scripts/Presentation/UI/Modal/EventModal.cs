
using UnityEngine;
using TMPro;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Events;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class EventModal : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private ChoiceButton choiceButtonPrefab;

        public void Show(GameEvent gameEvent, GameEngine engine, ILocalization loc)
        {
            // 1. Populate Text
            // Assuming titleKey and bodyKey are valid localization keys
            titleText.text = loc.T(gameEvent.titleKey, "Event Title");
            descriptionText.text = loc.T(gameEvent.bodyKey, "Event Description...");

            // 2. Clear any old choices
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }

            // 3. Instantiate and set up new choice buttons
            foreach (var choice in gameEvent.choices)
            {
                ChoiceButton buttonInstance = Instantiate(choiceButtonPrefab, choicesContainer);
                buttonInstance.Setup(choice, engine, loc);
            }
        }
    }
}
