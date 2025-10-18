
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    [RequireComponent(typeof(Button))]
    public class BusinessHotspotUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image ownedIndicator;

        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Setup(BusinessSO businessSO, BusinessInstance ownedInstance, GameEngine engine, ILocalization loc)
        {
            nameText.text = loc.T(businessSO.businessNameKey);
            ownedIndicator.enabled = ownedInstance != null;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => {
                var modalManager = FindObjectOfType<ModalManager>();
                if (modalManager == null) return;

                if (ownedInstance == null)
                {
                    modalManager.OpenBusinessPurchaseModal(businessSO, engine, loc);
                }
                else
                {
                    modalManager.OpenBusinessManagementModal(ownedInstance, engine, loc);
                }
            });
        }
    }
}
