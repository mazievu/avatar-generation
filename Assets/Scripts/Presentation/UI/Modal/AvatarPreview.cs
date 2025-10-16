using UnityEngine;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Presentation.UI.Avatar
{
    public class AvatarPreview : MonoBehaviour
    {
        [SerializeField] SpriteRenderer hair, eyes, body, outfit;
        public void Apply(AvatarState state)
        {
            if (state == null) return;
            if (hair)   hair.sprite   = state.hairSprite;
            if (eyes)   eyes.sprite   = state.eyeSprite;
            if (body)   body.sprite   = state.bodySprite;
            if (outfit) outfit.sprite = state.outfitSprite;
        }
    }
}