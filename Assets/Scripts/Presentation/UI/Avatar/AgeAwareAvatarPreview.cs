
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LifeSim.Presentation.UI.Avatar
{
    public class AgeAwareAvatarPreview : MonoBehaviour
    {
        [Header("Layer Images")]
        [SerializeField] private Image body;
        [SerializeField] private Image hair_back;
        [SerializeField] private Image eyes;
        [SerializeField] private Image eyebrows;
        [SerializeField] private Image nose;
        [SerializeField] private Image mouth;
        [SerializeField] private Image facial_hair;
        [SerializeField] private Image hair_front;

        private Dictionary<string, Image> _layerImages;

        void Awake()
        {
            _layerImages = new Dictionary<string, Image>
            {
                { "body", body },
                { "hair_back", hair_back },
                { "eyes", eyes },
                { "eyebrows", eyebrows },
                { "nose", nose },
                { "mouth", mouth },
                { "facial_hair", facial_hair },
                { "hair_front", hair_front },
            };
        }

        public void Render(Dictionary<string, string> avatarState)
        {
            foreach (var layer in _layerImages.Keys)
            {
                if (avatarState.TryGetValue(layer, out var spriteName) && !string.IsNullOrEmpty(spriteName))
                {
                    var spritePath = $"Avatars/{layer}/{spriteName}";
                    var sprite = Resources.Load<Sprite>(spritePath);
                    _layerImages[layer].sprite = sprite;
                    _layerImages[layer].enabled = sprite != null;
                }
                else
                {
                    _layerImages[layer].enabled = false;
                }
            }
        }
    }
}
