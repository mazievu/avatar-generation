using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI.Avatar
{
    public class AvatarBuilder : MonoBehaviour
    {
        [SerializeField] AvatarPreview preview;
        [SerializeField] Button btnNextHair, btnNextEyes, btnNextOutfit;
        [SerializeField] TMP_Text txtLabel;

        GameEngine _engine; ILocalization _loc;
        int hairIdx, eyesIdx, outfitIdx;

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e; _loc = l;
            btnNextHair.onClick.AddListener(()=>NextHair());
            btnNextEyes.onClick.AddListener(()=>NextEyes());
            btnNextOutfit.onClick.AddListener(()=>NextOutfit());
        }

        void NextHair(){ hairIdx=(hairIdx+1)%AvatarLibrary.HairCount; UpdatePreview(); }
        void NextEyes(){ eyesIdx=(eyesIdx+1)%AvatarLibrary.EyeCount; UpdatePreview(); }
        void NextOutfit(){ outfitIdx=(outfitIdx+1)%AvatarLibrary.OutfitCount; UpdatePreview(); }

        void UpdatePreview()
        {
            var st = new AvatarState{
                hairSprite = AvatarLibrary.GetHair(hairIdx),
                eyeSprite  = AvatarLibrary.GetEyes(eyesIdx),
                outfitSprite = AvatarLibrary.GetOutfit(outfitIdx),
                bodySprite = AvatarLibrary.DefaultBody
            };
            preview.Apply(st);
            txtLabel.text = _loc.T("avatar.preview");
        }
    }
}