using UnityEngine;
using TMPro;
using System.Collections.Generic;
using LifeSim.Core.Domain.Characters;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class ChoiceModal : ModalBase
    {
        [Header("UI References")]
        [SerializeField] private ComicPanel panel;
        [SerializeField] private AgeAwareAvatarPreview avatarPreview;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private ChoiceButton choiceButtonPrefab;

        public void Show(Character character, string title, string description, List<ChoiceOptionData> options)
        {
            // 1. Configure Avatar
            if (avatarPreview != null)
            {
                if (character != null)
                {
                    avatarPreview.gameObject.SetActive(true);
                    avatarPreview.Render(character);
                }
                else
                {
                    avatarPreview.gameObject.SetActive(false);
                }
            }

            // 2. Configure Text
            if (titleText) titleText.text = title;
            if (descriptionText) descriptionText.text = description;

            // 3. Configure Options
            // Clear any old options
            foreach (Transform child in optionsContainer)
            {
                Destroy(child.gameObject);
            }

            // Instantiate and set up new choice buttons
            if (choiceButtonPrefab != null)
            {
                foreach (var optionData in options)
                {
                    ChoiceButton buttonInstance = Instantiate(choiceButtonPrefab, optionsContainer);
                    buttonInstance.Setup(optionData);
                }
            }

            // 4. Open the panel
            Open();
        }

        public override void Open()
        {
            if (panel) panel.Open();
        }

        public override void Close()
        {
            if (panel) panel.Close();
        }
    }
}
