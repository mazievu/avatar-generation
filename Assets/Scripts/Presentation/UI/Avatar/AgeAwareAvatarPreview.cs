using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LifeSim.Presentation.UI.Avatar
{
    /// <summary>
    /// Manages the rendering of a character's avatar from multiple layers.
    /// Assumes sprites are located in a path like "Resources/Avatars/[LayerName]/[SpriteName]".
    /// Example: "Resources/Avatars/Face/face_01.png"
    /// </summary>
    public class AgeAwareAvatarPreview : MonoBehaviour
    {
        [Header("Avatar Layers (Order Matters)")]
        [Tooltip("Image for the background layer")]
        [SerializeField] private Image background;
        [Tooltip("Image for the back hair layer")]
        [SerializeField] private Image hair_back;
        [Tooltip("Image for the face/skin layer")]
        [SerializeField] private Image face;
        [Tooltip("Image for the eyes layer")]
        [SerializeField] private Image eye;
        [Tooltip("Image for the beard layer")]
        [SerializeField] private Image beard;
        [Tooltip("Image for the mouth layer")]
        [SerializeField] private Image mouth;
        [Tooltip("Image for the eyebrows layer")]
        [SerializeField] private Image eyebrows;
        [Tooltip("Image for the front hair layer")]
        [SerializeField] private Image hair_front;

        private Dictionary<string, Image> _layerMap;

        void Awake()
        {
            // Create a mapping from layer name to Image component for quick lookups
            _layerMap = new Dictionary<string, Image>
            {
                { "background", background },
                { "hair_back", hair_back },
                { "face", face },
                { "eye", eye },
                { "beard", beard },
                { "mouth", mouth },
                { "eyebrows", eyebrows },
                { "hair_front", hair_front }
            };
        }

        /// <summary>
        /// Renders the avatar based on the provided state dictionary.
        /// </summary>
        /// <param name="avatarState">A dictionary where the key is the layer name (e.g., "face") and the value is the sprite name (e.g., "face_01").</param>
        public void Render(Dictionary<string, string> avatarState)
        {
            if (_layerMap == null) Awake();

            // First, disable all layers
            foreach (var image in _layerMap.Values)
            {
                if (image != null) image.enabled = false;
            }

            // Then, enable and set the sprite for the specified layers
            foreach (var layerInfo in avatarState)
            {
                string layerName = layerInfo.Key;
                string spriteName = layerInfo.Value;

                if (_layerMap.TryGetValue(layerName, out Image imageComponent) && imageComponent != null)
                {
                    // Construct the path to the sprite in the Resources folder
                    string resourcePath = $"Avatars/{layerName}/{spriteName}";
                    Sprite newSprite = Resources.Load<Sprite>(resourcePath);

                    if (newSprite != null)
                    {
                        imageComponent.sprite = newSprite;
                        imageComponent.enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Avatar sprite not found at path: {resourcePath}");
                    }
                }
            }
        }

        /// <summary>
        /// Overload to render an avatar directly from a Character object.
        /// </summary>
        public void Render(Core.Domain.Characters.Character character)
        {
            if (character != null)
            {
                Render(character.GetAvatarState());
            }
        }
    }
}