
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Events;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    [RequireComponent(typeof(Button))]
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;

        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Setup(EventChoice choice, GameEngine engine, ILocalization loc)
        {
            // Set the text for the button
            buttonText.text = loc.T(choice.labelKey, "Choice Text");

            // Remove any previous listeners to prevent duplicates
            _button.onClick.RemoveAllListeners();

            // Add the new listener that calls the game engine
            _button.onClick.AddListener(() => 
            {
                engine.HandleEventChoice(choice.id);
            });
        }
    }
}
