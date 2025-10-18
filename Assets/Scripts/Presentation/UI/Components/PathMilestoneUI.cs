
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class PathMilestoneUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text requirementText;
        [SerializeField] private Button claimButton;
        [SerializeField] private TMP_Text claimButtonText;

        public void Setup(PathMilestoneSO milestone, bool canClaim, bool isClaimed, GameEngine engine, ILocalization loc)
        {
            descriptionText.text = loc.T(milestone.descriptionKey, "(Description)");
            requirementText.text = loc.T("ui.path.requires", "Requires: {0} children", milestone.childrenRequired);

            claimButton.onClick.RemoveAllListeners();

            if (isClaimed)
            {
                claimButton.interactable = false;
                claimButtonText.text = loc.T("ui.path.claimed", "Claimed");
            }
            else
            {
                if (canClaim)
                {
                    claimButton.interactable = true;
                    claimButtonText.text = loc.T("ui.path.claim", "Claim");
                    claimButton.onClick.AddListener(() => engine.ClaimFeature(milestone.featureId));
                }
                else
                {
                    claimButton.interactable = false;
                    claimButtonText.text = loc.T("ui.path.locked", "Locked");
                }
            }
        }
    }
}
