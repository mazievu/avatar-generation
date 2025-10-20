
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Presentation.UI.Avatar;

namespace LifeSim.Presentation.UI
{
    public class CharacterNode : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private TMP_Text incomeText;
        [SerializeField] private AgeAwareAvatarPreview avatarPreview;
        [SerializeField] private Image genderIcon;
        [SerializeField] private Image highlightBorder;

        [Header("Asset References")]
        public Sprite maleIcon;
        public Sprite femaleIcon;

        [Header("Animation References")]
        [SerializeField] private Animator nodeAnimator;
        [SerializeField] private TMP_Text floatingIncomeText;

        public Character CharacterData { get; private set; }

        public void Setup(Character character, ILocalization loc)
        {
            CharacterData = character;

            // Populate UI
            nameText.text = character.name;
            int age = character.ageDays / Constants.DaysPerYear;
            ageText.text = string.Format(loc.T("ui.age"), age);
            incomeText.text = string.Format(loc.T("ui.income"), character.monthlyIncome);

            if (genderIcon != null)
            {
                genderIcon.sprite = (character.gender == "male") ? maleIcon : femaleIcon;
            }

            // Render avatar
            if (avatarPreview != null)
            {
                avatarPreview.Render(character.GetAvatarState());
            }

            // Set highlight for player character
            if (highlightBorder != null)
            {
                highlightBorder.enabled = (character.relation == "self");
            }
        }

        public void PlayIncomeAnimation(int amount)
        {
            if (nodeAnimator != null && floatingIncomeText != null)
            {
                floatingIncomeText.text = string.Format("+{0}", amount);
                nodeAnimator.SetTrigger("ShowIncome");
            }
        }
    }
}
