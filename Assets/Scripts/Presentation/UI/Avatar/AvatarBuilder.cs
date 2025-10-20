
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Data;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI.Avatar
{
    public class AvatarBuilder : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private AgeAwareAvatarPreview avatarPreview;
        [SerializeField] private Transform layerContainer;
        [SerializeField] private Transform optionContainer;
        [SerializeField] private Button layerButtonPrefab;
        [SerializeField] private Button optionButtonPrefab;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;

        private GameEngine _engine;
        private ILocalization _loc;
        private Character _character;
        private Dictionary<string, string> _tempAvatarState;

        public void Show(Character character, GameEngine engine)
        {
            _engine = engine;
            _character = character;
            
            // Find the localization service as it's not passed in
            var uiController = FindFirstObjectByType<GameUIController>();
            if (uiController != null) { _loc = uiController.loc; }

            gameObject.SetActive(true);

            // Initialize temporary state from character data
            _tempAvatarState = new Dictionary<string, string>();
            for (int i = 0; i < character.avatarState_keys.Count; i++)
            {
                _tempAvatarState[character.avatarState_keys[i]] = character.avatarState_values[i];
            }

            saveButton.onClick.AddListener(OnSave);
            cancelButton.onClick.AddListener(OnCancel);

            PopulateLayers();
            RefreshPreview();
        }

        private void PopulateLayers()
        {
            foreach (Transform child in layerContainer) { Destroy(child.gameObject); }

            foreach (var layerName in Database.AvatarSprites.Keys)
            {
                var btnInstance = Instantiate(layerButtonPrefab, layerContainer);
                var textComponent = btnInstance.GetComponentInChildren<TMPro.TMP_Text>();
                if (_loc != null)
                {
                    textComponent.text = _loc.T($"avatar.layer.{layerName}", layerName);
                }
                else
                {
                    textComponent.text = layerName;
                }
                btnInstance.onClick.AddListener(() => PopulateOptionsForLayer(layerName));
            }
        }

        private void PopulateOptionsForLayer(string layerName)
        {
            foreach (Transform child in optionContainer) { Destroy(child.gameObject); }

            if (Database.AvatarSprites.TryGetValue(layerName, out var options))
            {
                foreach (var optionName in options)
                {
                    var btnInstance = Instantiate(optionButtonPrefab, optionContainer);
                    var textComponent = btnInstance.GetComponentInChildren<TMPro.TMP_Text>();
                    if (_loc != null)
                    {
                        textComponent.text = _loc.T($"avatar.option.{optionName}", optionName);
                    }
                    else
                    {
                        textComponent.text = optionName;
                    }
                    btnInstance.onClick.AddListener(() => OnSelectOption(layerName, optionName));
                }
            }
        }

        private void OnSelectOption(string layer, string option)
        {
            _tempAvatarState[layer] = option;
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            avatarPreview.Render(_tempAvatarState);
        }

        private void OnSave()
        {
            _engine.CustomizeAvatar(
                _character.id, 
                _tempAvatarState.Keys.ToList(), 
                _tempAvatarState.Values.ToList()
            );
            gameObject.SetActive(false);
        }

        private void OnCancel()
        {
            gameObject.SetActive(false);
        }

        void OnDisable()
        {
            saveButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}
