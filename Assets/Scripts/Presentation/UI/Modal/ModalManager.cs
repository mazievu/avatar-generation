using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI
{
    public class ModalManager : MonoBehaviour
    {
        [Header("Reusable Modals")]
        [SerializeField] private ChoiceModal choiceModalPrefab;
        [SerializeField] private SettingsModal settingsModalPrefab;

        [Header("Specialized Modals")]
        [SerializeField] private AssetDetailModal assetDetailModalPrefab;
        [SerializeField] private BusinessPurchaseModal businessPurchaseModalPrefab;
        [SerializeField] private BusinessManagementModal businessManagementModalPrefab;

        // Active instances
        private ChoiceModal _activeChoiceModal;
        private SettingsModal _activeSettingsModal;
        private AssetDetailModal _activeAssetModal;
        private BusinessPurchaseModal _activePurchaseModal;
        private BusinessManagementModal _activeManagementModal;

        public void SyncWithState(GameEngine engine, GameState state, ILocalization loc)
        {
            bool isModalActive = _activeChoiceModal != null;
            bool shouldModalBeActive = state.activeEvent != null || state.HasPendingChoices();

            if (shouldModalBeActive && !isModalActive)
            {
                // A new event or choice has appeared, so we need to show a modal.
                _activeChoiceModal = Instantiate(choiceModalPrefab, transform);
                
                // Determine which modal to show based on priority
                if (state.activeEvent != null)
                {
                    ShowEventModal(engine, state, loc);
                }
                else if (state.pendingPromotion != null)
                {
                    ShowPromotionModal(engine, state, loc);
                }
                else if (state.pendingJobOffer != null)
                {
                    ShowJobOfferModal(engine, state, loc);
                }
                else if (state.pendingRejection != null)
                {
                    ShowRejectionModal(engine, state, loc);
                }
                else if (state.pendingCareerChoice != null)
                {
                    ShowCareerChoiceModal(engine, state, loc);
                }
                else if (state.pendingClubChoice != null)
                {
                    ShowClubChoiceModal(engine, state, loc);
                }
                else if (state.pendingMajorChoice != null)
                {
                    ShowMajorChoiceModal(engine, state, loc);
                }
                else if (state.pendingLoanChoice != null)
                {
                    ShowLoanChoiceModal(engine, state, loc);
                }
                // ... add other pending choices here in order of priority ...
                else
                {
                    // Fallback, should not happen if logic is correct
                    _activeChoiceModal.Close();
                }
            }
            else if (!shouldModalBeActive && isModalActive)
            {
                // All events and choices have been cleared, close the modal.
                _activeChoiceModal.Close();
                Destroy(_activeChoiceModal.gameObject);
                _activeChoiceModal = null;
            }
        }

        private void ShowEventModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var gameEvent = state.activeEvent;
            var character = state.familyMembers[gameEvent.characterId];
            var title = loc.T(gameEvent.titleKey, "Event");
            var description = loc.T(gameEvent.bodyKey, "...");

            var options = gameEvent.choices.Select(choice => 
                new ChoiceOptionData(loc.T(choice.labelKey), () => {
                    engine.HandleEventChoice(choice.id);
                })
            ).ToList();

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowPromotionModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var pending = state.pendingPromotion;
            var character = state.familyMembers[pending.characterId];
            var track = Database.Careers[character.careerTrackId];
            var newLevelInfo = track.levels[character.careerLevel + 1];

            var title = loc.T("promotion.title", "Promotion!");
            var description = loc.T(pending.newTitleKey, "You have been promoted to {0}!", loc.T(newLevelInfo.titleKey));

            var options = new List<ChoiceOptionData>
            {
                new ChoiceOptionData(loc.T("ui.accept", "Accept"), () => {
                    engine.AcceptPromotion(pending.characterId);
                }),
                new ChoiceOptionData(loc.T("ui.decline", "Decline"), () => {
                    engine.DeclinePromotion(pending.characterId);
                })
            };

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowCareerChoiceModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var pending = state.pendingCareerChoice;
            var character = state.familyMembers[pending.characterId];

            var title = loc.T("career_choice.title", "Choose a Career");
            var description = loc.T("career_choice.desc", "It's time to decide your future path.");

            // Find the preferred career based on the character's major
            string preferredCareerId = null;
            if (!string.IsNullOrEmpty(character.educationMajorId) && Database.EducationOptions.TryGetValue(character.educationMajorId, out var majorSO))
            {
                preferredCareerId = majorSO.preferredCareerId;
            }

            var options = Database.Careers.Values.Select(career => {
                string careerName = loc.T(career.trackNameKey, career.name);
                // Add a star if this career is preferred for the character's major
                if (career.name == preferredCareerId)
                {
                    careerName = "★ " + careerName;
                }

                return new ChoiceOptionData(
                    careerName,
                    () => { engine.ChooseCareer(character.id, career.name); },
                    character.stats.iq >= career.requiredIq && character.stats.eq >= career.requiredEq
                );
            }).ToList();

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowClubChoiceModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var pending = state.pendingClubChoice;
            var character = state.familyMembers[pending.characterId];

            var title = loc.T("modal_club_choice_title", "Choose a Club");
            var description = loc.T("modal_club_choice_desc", "It's time for {0} to join an after-school club!", character.name);

            var options = Database.Clubs.Values.Select(club => 
                new ChoiceOptionData(
                    loc.T(club.clubNameKey, club.name),
                    () => { engine.JoinClub(character.id, club.clubId); }
                )
            ).ToList();

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowMajorChoiceModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var pending = state.pendingMajorChoice;
            var character = state.familyMembers[pending.characterId];

            var title = loc.T("major_choice.title", "Choose a Major");
            var description = loc.T("major_choice.desc", "Your club activities have prepared you for certain fields.");

            // Get the recommended major from the character's club
            string recommendedMajorId = null;
            if (!string.IsNullOrEmpty(character.clubId) && Database.Clubs.TryGetValue(character.clubId, out var clubSO))
            {
                recommendedMajorId = clubSO.recommendedMajorId;
            }

            // Get all available majors and sort them, putting the recommended one first
            var sortedMajors = Database.EducationOptions.Values
                .Where(edu => edu.type == EducationSO.EducationType.UniversityMajor)
                .OrderBy(major => major.name == recommendedMajorId ? 0 : 1) // Sorts the recommended major to the top
                .ToList();

            var options = sortedMajors.Select(major => 
                new ChoiceOptionData(
                    loc.T(major.NameKey, major.name),
                    () => { engine.ChooseMajor(character.id, major.name); },
                    character.stats.iq >= major.minIq && character.stats.eq >= major.minEq
                )
            ).ToList();

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowLoanChoiceModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var title = loc.T("loan.title", "Take a Loan");
            var description = loc.T("loan.desc", "Your family fund is low. A loan could help, but it comes with risks.");

            var options = new List<ChoiceOptionData>
            {
                new ChoiceOptionData("$10,000", () => engine.TakeLoan(10000)),
                new ChoiceOptionData("$50,000", () => engine.TakeLoan(50000)),
                new ChoiceOptionData("$200,000", () => engine.TakeLoan(200000)),
                new ChoiceOptionData(loc.T("ui.cancel", "Cancel"), () => engine.DeclineLoan())
            };

            // No character is associated with a loan, so pass null.
            _activeChoiceModal.Show(null, title, description, options);
        }

        private void ShowJobOfferModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var offer = state.pendingJobOffer;
            var character = state.familyMembers[offer.characterId];
            var company = Database.Companies[offer.companyId];
            var career = Database.Careers[offer.careerId];

            var title = loc.T("job_offer.title", "Job Offer!");
            var description = string.Format(
                "{0}\n\n<b>{1}</b>\n{2}\n\n<b>{3}</b>\nIQ: {4}, EQ: {5}, Skill: {6}\n\n<b><color=yellow>{7}: {8}%</color></b>",
                loc.T(company.companyNameKey, company.name),
                loc.T("ui.career", "Career"),
                loc.T(career.trackNameKey, career.name),
                loc.T("ui.requirements", "Requirements"),
                (int)(career.requiredIq * company.requiredStatMultiplier),
                (int)(career.requiredEq * company.requiredStatMultiplier),
                (int)(career.levels[0].requiredSkill * company.requiredStatMultiplier),
                loc.T("ui.success_chance", "Success Chance"),
                offer.successChance
            );

            var options = new List<ChoiceOptionData>
            {
                new ChoiceOptionData(loc.T("ui.apply", "Apply"), () => engine.ApplyForJob()),
                new ChoiceOptionData(loc.T("ui.ignore", "Ignore"), () => engine.IgnoreJobOffer())
            };

            _activeChoiceModal.Show(character, title, description, options);
        }

        private void ShowRejectionModal(GameEngine engine, GameState state, ILocalization loc)
        {
            var rejection = state.pendingRejection;
            var character = state.familyMembers[rejection.characterId];

            var title = loc.T("rejection.title", "Application Rejected");
            var description = loc.T("rejection.desc", "Unfortunately, your application was not successful. What would you like to do?");

            var options = new List<ChoiceOptionData>
            {
                new ChoiceOptionData(loc.T("rejection.internship", "Ask for Internship"), () => engine.HandleRejection_Internship()),
                new ChoiceOptionData(loc.T("rejection.seek_lower", "Seek Easier Jobs"), () => engine.HandleRejection_SeekLower()),
                new ChoiceOptionData(loc.T("rejection.give_up", "Give Up"), () => engine.HandleRejection_GiveUp()),
            };

            _activeChoiceModal.Show(character, title, description, options);
        }

        // ... Implement other Show...Modal methods for School, University, Major etc. following the same pattern ...

        public void OpenAssetDetailModal(AssetSO asset, GameEngine engine, ILocalization loc)
        {
            if (_activeAssetModal != null) return;
            _activeAssetModal = Instantiate(assetDetailModalPrefab, transform);
            _activeAssetModal.Show(asset, engine, loc);
        }

        public void OpenBusinessPurchaseModal(BusinessSO business, GameEngine engine, ILocalization loc)
        {
            if (_activePurchaseModal != null) return;
            _activePurchaseModal = Instantiate(businessPurchaseModalPrefab, transform);
            _activePurchaseModal.Show(business, engine, loc);
        }

        public void OpenBusinessManagementModal(BusinessInstance instance, GameEngine engine, ILocalization loc)
        {
            if (_activeManagementModal != null) return;
            _activeManagementModal = Instantiate(businessManagementModalPrefab, transform);
            _activeManagementModal.Show(instance, engine, loc);
        }

        public void OpenSettings(GameEngine engine, ILocalization loc)
        {
            if (_activeSettingsModal == null)
            {
                _activeSettingsModal = Instantiate(settingsModalPrefab, transform);
                _activeSettingsModal.Bind(engine, loc);
            }
            _activeSettingsModal.Open();
        }
    }
}
