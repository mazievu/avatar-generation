using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LifeSim.Core.Data;
using LifeSim.Core.Data.SO;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Domain.Events;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;
using UnityEngine;

namespace LifeSim.Core.Engine
{
    public class GameEngine
    {
        public GameState State { get; private set; }

        private readonly ISaveStore _save;
        private readonly ILocalization _loc;
        private readonly IRandom _rng;
        private readonly IClock _clock;

        public event Action<GameState> OnStateChanged;
        public event Action<string> OnLog;

        public GameEngine(GameState initialState, ISaveStore save, ILocalization loc, IRandom rng, IClock clock)
        {
            State = initialState ?? new GameState();
            _save = save; _loc = loc; _rng = rng; _clock = clock;
        }

        public void Boot(string language)
        {
            _loc.SetLanguage(language);
            State.lang = language;

            if (State.familyMembers.Count == 0)
            {
                var me = new Character { id = "me", name = "You", generation = 1 };
                State.familyMembers[me.id] = me;
                State.totalMembers = 1;
            }

            Emit();
        }

        public void Tick()
        {
            if (Domain.Gameplay.PausePolicy.IsPaused(State) || State.gameOverReason != null)
                return;

            int daysToSimulate = Mathf.Max(1, (int)(_clock.DaysPerTick * State.timeScale));
            for (int i = 0; i < daysToSimulate; i++)
            {
                SimulateOneDay();
            }

            if (State.activeEvent == null && State.eventQueue.Count > 0)
            {
                State.activeEvent = State.eventQueue.Dequeue();
            }
            else
            {
                MaybeTriggerAmbientEvent();
            }

            if (State.familyFund < -100000)
            {
                State.gameOverReason = "debt";
            }

            Emit();
        }

        #region Settings Methods
        public void SetTimeScale(float scale)
        {
            State.timeScale = Mathf.Max(0, scale);
            Emit();
        }

        public void ToggleManualPause()
        {
            State.isManuallyPaused = !State.isManuallyPaused;
            Emit();
        }

        public void SwitchLanguage()
        {
            string newLang = State.lang == "en" ? "vi" : "en";
            _loc.SetLanguage(newLang);
            State.lang = newLang;
            Emit();
        }
        #endregion

        private void SimulateOneDay()
        {
            State.currentDate.day++;

            if (State.eventCooldown > 0) { State.eventCooldown--; }

            if (State.currentDate.day % 30 == 0) { UpdateMonthly(); }

            if (State.currentDate.day >= 365)
            {
                State.currentDate.year++;
                State.currentDate.day = 0;
                UpdateYearly();
            }

            foreach (var c in State.familyMembers.Values)
            {
                if (!c.isAlive) continue;
                c.ageDays++;
                UpdateLifeStage(c);
            }
        }

        private void UpdateMonthly()
        {
            MaybeGenerateJobOffer();

            int totalIncome = 0;
            int totalExpenses = 0;

            foreach (var character in State.familyMembers.Values)
            {
                if (!character.isAlive) continue;

                if (character.status == CharacterStatus.Working && !string.IsNullOrEmpty(character.careerTrackId))
                {
                    if (Database.Careers.TryGetValue(character.careerTrackId, out var careerTrack))
                    {
                        if (character.careerLevel >= 0 && character.careerLevel < careerTrack.levels.Count)
                        {
                            var levelInfo = careerTrack.levels[character.careerLevel];
                            int baseSalary = levelInfo.salaryPerMonth;
                            int finalSalary = baseSalary;

                            if (!string.IsNullOrEmpty(character.companyId) && Database.Companies.TryGetValue(character.companyId, out var company))
                            {
                                finalSalary = (int)(baseSalary * company.salaryMultiplier);
                                int requiredIQ = (int)(careerTrack.requiredIq * company.requiredStatMultiplier);
                                int requiredEQ = (int)(careerTrack.requiredEq * company.requiredStatMultiplier);
                                int statBonus = (character.stats.iq - requiredIQ) + (character.stats.eq - requiredEQ);
                                finalSalary += statBonus * 10;
                            }

                            if (character.isIntern)
                            {
                                finalSalary = (int)(finalSalary * 0.25f);
                                character.stats.skill += 5;

                                int requiredSkill = (int)(levelInfo.requiredSkill * (Database.Companies.TryGetValue(character.companyId, out var c) ? c.requiredStatMultiplier : 1f));
                                if (character.stats.skill >= requiredSkill)
                                {
                                    character.isIntern = false;
                                }
                            }
                            totalIncome += finalSalary;
                        }
                    }
                }
                else if (character.status == CharacterStatus.Retired)
                {
                    totalIncome += Constants.RETIREMENT_PENSION_PER_MONTH;
                }
                totalExpenses += Constants.GetCostOfLiving(character.lifePhase);
            }

            foreach (var businessInstance in State.businesses)
            {
                totalIncome += CalculateBusinessIncome(businessInstance);
            }

            int netChange = totalIncome - totalExpenses;
            State.familyFund += netChange;
            State.familyIncomePerMonth = netChange;
        }

        private void UpdateYearly()
        {
            State.characterEventCount.Clear();
            foreach (var character in State.familyMembers.Values)
            {
                if (!character.isAlive) continue;
                int age = character.ageDays / Constants.DaysPerYear;
                switch (age)
                {
                    case 6: State.pendingSchoolChoice.Add(new PendingSchoolChoice { characterId = character.id, newPhase = "ElementarySchool" }); break;
                    case 12:
                        State.pendingSchoolChoice.Add(new PendingSchoolChoice { characterId = character.id, newPhase = "MiddleSchool" });
                        State.pendingClubChoice = new PendingClubChoice { characterId = character.id };
                        break;
                    case 16: State.pendingSchoolChoice.Add(new PendingSchoolChoice { characterId = character.id, newPhase = "HighSchool" }); break;
                    case 19: State.pendingUniversityChoice.Add(new PendingUniversityChoice { characterId = character.id }); break;
                    case 60:
                        if (character.status != CharacterStatus.Retired) { character.status = CharacterStatus.Retired; }
                        break;
                }
            }
        }

        private void UpdateLifeStage(Character character)
        {
            int age = character.ageDays / Constants.DaysPerYear;
            LifePhase newPhase;
            if (age >= 60) { newPhase = LifePhase.Retired; }
            else if (age >= 23) { newPhase = LifePhase.WorkingLife; }
            else if (age >= 19) { newPhase = LifePhase.University; }
            else if (age >= 16) { newPhase = LifePhase.HighSchool; }
            else if (age >= 12) { newPhase = LifePhase.MiddleSchool; }
            else if (age >= 6) { newPhase = LifePhase.ElementarySchool; }
            else { newPhase = LifePhase.Newborn; }
            if (newPhase != character.lifePhase) { character.lifePhase = newPhase; }
        }

        private void MaybeTriggerAmbientEvent()
        {
            if (State.eventCooldown > 0) return;
            if (State.activeEvent != null) return;
            if (State.HasPendingChoices()) return;

            var potentialSubjects = State.familyMembers.Values.Where(c => c.isAlive).ToList();
            if (potentialSubjects.Count == 0) return;

            var subject = potentialSubjects[_rng.NextInt(0, potentialSubjects.Count - 1)];
            int eventsThisYear = State.characterEventCount.TryGetValue(subject.id, out var count) ? count : 0;
            int subjectAge = subject.ageDays / Constants.DaysPerYear;
            if (eventsThisYear >= 2 && subjectAge > 5) return;

            var possibleEvents = Database.Events.Values.Where(e =>
                e.eventType == EventSO.EventType.Random &&
                e.lifePhase == subject.lifePhase &&
                (string.IsNullOrEmpty(e.requiredClubId) || e.requiredClubId == subject.clubId) &&
                !State.triggeredOneTimeEvents.Contains(e.name)
            ).ToList();

            if (possibleEvents.Count > 0)
            {
                var eventSO = possibleEvents[_rng.NextInt(0, possibleEvents.Count - 1)];
                var gameEvent = new GameEvent
                {
                    id = eventSO.name,
                    characterId = subject.id,
                    titleKey = eventSO.titleKey,
                    bodyKey = eventSO.descriptionKey,
                    choices = eventSO.choices.Select(c => new Domain.Events.EventChoice { id = c.choiceId, labelKey = c.choiceKey }).ToList()
                };
                State.activeEvent = gameEvent;
                State.eventCooldown = _rng.NextInt(15, 45);
                State.characterEventCount[subject.id] = eventsThisYear + 1;
            }
        }

        private void MaybeGenerateJobOffer()
        {
            if (State.HasPendingChoices() || State.activeEvent != null) return;

            var character = State.familyMembers.Values.FirstOrDefault(c => c.isAlive && !string.IsNullOrEmpty(c.careerTrackId) && c.status == CharacterStatus.Idle);
            if (character == null) return;

            if (!Database.Careers.TryGetValue(character.careerTrackId, out var careerSO)) return;

            var eligibleTiers = careerSO.companyTiers;
            if (character.seekingLowerTier) { eligibleTiers = eligibleTiers.Where(t => t <= 2).ToList(); }
            if (eligibleTiers.Count == 0) return;

            var possibleCompanies = Database.Companies.Values.Where(c => eligibleTiers.Contains(c.prestigeTier)).ToList();
            if (possibleCompanies.Count == 0) return;

            var company = possibleCompanies[_rng.NextInt(0, possibleCompanies.Count - 1)];

            State.pendingJobOffer = new JobOffer
            {
                characterId = character.id,
                careerId = careerSO.name,
                companyId = company.name,
                successChance = CalculateSuccessChance(character, careerSO, company)
            };
        }

        private int CalculateSuccessChance(Character character, CareerSO career, CompanySO company)
        {
            int requiredIQ = (int)(career.requiredIq * company.requiredStatMultiplier);
            int requiredEQ = (int)(career.requiredEq * company.requiredStatMultiplier);
            int requiredSkill = (int)(career.levels[0].requiredSkill * company.requiredStatMultiplier);
            
            int requiredEducationTier = 0;
            if (!string.IsNullOrEmpty(company.requiredEducationId) && Database.EducationOptions.TryGetValue(company.requiredEducationId, out var eduSO)) { requiredEducationTier = eduSO.educationTier; }

            int characterEducationTier = 0;
            if (!string.IsNullOrEmpty(character.educationMajorId) && Database.EducationOptions.TryGetValue(character.educationMajorId, out var charEduSO)) { characterEducationTier = charEduSO.educationTier; }

            if (character.stats.iq >= requiredIQ && character.stats.eq >= requiredEQ && character.stats.skill >= requiredSkill && characterEducationTier >= requiredEducationTier)
            {
                return 100;
            }

            int baseChance = 50;
            int statBonus = (character.stats.iq - requiredIQ) + (character.stats.eq - requiredEQ) + (character.stats.skill - requiredSkill);
            int educationBonus = (characterEducationTier - requiredEducationTier) * 10;

            return Mathf.Clamp(baseChance + statBonus + educationBonus, 5, 95);
        }

        #region Player Choices
        public void HandleEventChoice(string choiceId)
        {
            if (State.activeEvent == null) return;
            if (!Database.Events.TryGetValue(State.activeEvent.id, out var eventSO)) { State.activeEvent = null; return; }
            var choice = eventSO.choices.FirstOrDefault(c => c.choiceId == choiceId);
            if (choice == null) { State.activeEvent = null; return; }

            if (State.familyMembers.TryGetValue(State.activeEvent.characterId, out var character))
            {
                foreach (var effect in choice.effects)
                {
                    switch (effect.type)
                    {
                        case EventSO.EffectType.StatChange: ApplyStatChange(character, effect.targetStat, effect.value); break;
                        case EventSO.EffectType.FundChange: State.familyFund += effect.value; break;
                    }
                    if (!string.IsNullOrEmpty(effect.logKey))
                    {
                        State.GameLog.Insert(0, new GameLogEntry { year = State.currentDate.year, characterName = character.name, messageKey = effect.logKey });
                    }
                }
            }
            if (eventSO.eventType != EventSO.EventType.Random) { State.triggeredOneTimeEvents.Add(eventSO.name); }
            State.activeEvent = null;
            Emit();
        }

        public void AcceptPromotion(string characterId)
        {
            if (State.familyMembers.TryGetValue(characterId, out var character))
            {
                character.careerLevel++;
            }
            State.pendingPromotion = null;
            Emit();
        }

        public void DeclinePromotion(string characterId)
        {
            State.pendingPromotion = null;
            Emit();
        }

        public void ChooseCareer(string characterId, string careerId)
        {
            if (State.familyMembers.TryGetValue(characterId, out var character) && Database.Careers.TryGetValue(careerId, out var career))
            {
                if (character.stats.iq < career.requiredIq || character.stats.eq < career.requiredEq)
                {
                    State.pendingUnderqualifiedChoice = new PendingUnderqualifiedChoice { characterId = characterId, careerTrackKey = careerId };
                }
                else
                {
                    character.careerTrackId = careerId;
                    character.careerLevel = 0;
                    character.status = CharacterStatus.Working;
                }
            }
            State.pendingCareerChoice = null;
            Emit();
        }

        public void ChooseSchool(string characterId, string schoolId)
        {
            State.pendingSchoolChoice.RemoveAll(c => c.characterId == characterId);
            Emit();
        }

        public void ChooseUniversity(string characterId, string universityId)
        {
            State.pendingUniversityChoice.RemoveAll(c => c.characterId == characterId);
            Emit();
        }

        public void ChooseMajor(string characterId, string majorId)
        {
            if (State.familyMembers.TryGetValue(characterId, out var character))
            {
                character.educationMajorId = majorId;
            }
            State.pendingMajorChoice = null;
            Emit();
        }

        public void TakeLoan(int amount)
        {
            State.familyFund += amount;
            State.pendingLoanChoice = null;
            Emit();
        }

        public void DeclineLoan()
        {
            State.pendingLoanChoice = null;
            Emit();
        }

        public void JoinClub(string characterId, string clubId)
        {
            if (State.familyMembers.TryGetValue(characterId, out var character))
            {
                character.clubId = clubId;
            }
            State.pendingClubChoice = null;
            Emit();
        }

        public void DeclineClubs(string characterId)
        {
            State.pendingClubChoice = null;
            Emit();
        }
        #endregion

        #region Job Application
        public void ApplyForJob()
        {
            var offer = State.pendingJobOffer;
            if (offer == null) return;

            bool success = _rng.NextInt(0, 100) < offer.successChance;

            if (success)
            {
                if (State.familyMembers.TryGetValue(offer.characterId, out var character))
                {
                    character.careerTrackId = offer.careerId;
                    character.companyId = offer.companyId;
                    character.careerLevel = 0;
                    character.status = CharacterStatus.Working;
                    character.isIntern = false;
                    character.seekingLowerTier = false;
                }
            }
            else
            {
                State.pendingRejection = new LifeSim.Core.Domain.Game.RejectionData { characterId = offer.characterId, careerId = offer.careerId, companyId = offer.companyId };
            }

            State.pendingJobOffer = null;
            Emit();
        }

        public void IgnoreJobOffer()
        {
            State.pendingJobOffer = null;
            Emit();
        }

        public void HandleRejection_GiveUp()
        {
            State.pendingRejection = null;
            Emit();
        }

        public void HandleRejection_Internship()
        {
            var rejection = State.pendingRejection;
            if (rejection == null) return;

            if (State.familyMembers.TryGetValue(rejection.characterId, out var character))
            {
                character.careerTrackId = rejection.careerId;
                character.companyId = rejection.companyId;
                character.careerLevel = 0;
                character.status = CharacterStatus.Working;
                character.isIntern = true;
                character.seekingLowerTier = false;
            }
            State.pendingRejection = null;
            Emit();
        }

        public void HandleRejection_SeekLower()
        {
            var rejection = State.pendingRejection;
            if (rejection == null) return;

            if (State.familyMembers.TryGetValue(rejection.characterId, out var character))
            {
                character.seekingLowerTier = true;
            }
            State.pendingRejection = null;
            Emit();
        }
        #endregion

        private void ApplyStatChange(Character character, string stat, int value)
        {
            if (stat == "happiness") character.stats.happiness = Mathf.Clamp(character.stats.happiness + value, Constants.MinStat, Constants.MaxStat);
            else if (stat == "health") character.stats.health = Mathf.Clamp(character.stats.health + value, Constants.MinStat, Constants.MaxStat);
            else if (stat == "iq") character.stats.iq = Mathf.Clamp(character.stats.iq + value, Constants.MinStat, Constants.MaxIQ);
            else if (stat == "eq") character.stats.eq = Mathf.Clamp(character.stats.eq + value, Constants.MinStat, Constants.MaxStat);
            else if (stat == "skill") character.stats.skill = Mathf.Clamp(character.stats.skill + value, Constants.MinStat, Constants.MaxStat);
        }

        private int CalculateBusinessIncome(BusinessInstance instance)
        {
            if (!Database.Businesses.TryGetValue(instance.businessId, out var businessSO)) return 0;
            var tier = businessSO.tiers.FirstOrDefault(t => t.tierLevel == instance.tier);
            if (tier == null) return 0;

            float totalSkill = 0;
            int employeeCount = 0;
            int totalSalaryCost = 0;

            foreach (var employeeId in instance.employeeIds)
            {
                if (string.IsNullOrEmpty(employeeId)) continue;
                employeeCount++;
                if (employeeId == "robot")
                {
                    totalSkill += BusinessSO.BusinessTier.ROBOT_SKILL;
                    totalSalaryCost += tier.robotMonthlyCost;
                }
                else if (State.familyMembers.TryGetValue(employeeId, out var employee))
                {
                    totalSkill += employee.stats.skill;
                    totalSalaryCost += employee.stats.skill * tier.employeeSalaryPerSkillPoint;
                }
            }

            float avgSkill = employeeCount > 0 ? totalSkill / employeeCount : 0;
            float revenue = tier.baseRevenue * (1 + avgSkill / 100f);
            float cogs = revenue * tier.costOfGoodsSoldPercent;
            float totalCosts = cogs + tier.fixedCosts + totalSalaryCost;

            return Mathf.RoundToInt(revenue - totalCosts);
        }

        public int GetBusinessIncome(string instanceId)
        {
            var instance = State.businesses.FirstOrDefault(b => b.instanceId == instanceId);
            if (instance == null) return 0;
            return CalculateBusinessIncome(instance);
        }

        public void PurchaseAsset(string assetId)
        {
            if (State.assets.Contains(assetId)) return;
            if (Database.Assets.TryGetValue(assetId, out var assetSO))
            {
                if (State.familyFund >= assetSO.price)
                {
                    State.familyFund -= assetSO.price;
                    State.assets.Add(assetId);
                }
            }
        }

        public void ClaimFeature(string featureId)
        {
            if (!State.unlockedFeatures.Contains(featureId)) State.unlockedFeatures.Add(featureId);
            Emit();
        }

        public void PurchaseBusiness(string businessId)
        {
            if (!Database.Businesses.TryGetValue(businessId, out var businessSO)) return;
            var tier1 = businessSO.tiers.FirstOrDefault(t => t.tierLevel == 1);
            if (tier1 == null) return;

            if (State.familyFund >= tier1.purchasePrice)
            {
                State.familyFund -= tier1.purchasePrice;
                var newInstance = new BusinessInstance
                {
                    instanceId = Guid.NewGuid().ToString(),
                    businessId = businessId,
                    tier = 1,
                    employeeIds = new List<string>(new string[tier1.employeeSlots])
                };
                State.businesses.Add(newInstance);
            }
        }

        public void UpgradeBusiness(string instanceId)
        {
            var instance = State.businesses.FirstOrDefault(b => b.instanceId == instanceId);
            if (instance == null) return;
            if (!Database.Businesses.TryGetValue(instance.businessId, out var businessSO)) return;
            var nextTier = businessSO.tiers.FirstOrDefault(t => t.tierLevel == instance.tier + 1);
            if (nextTier == null) return;

            if (State.familyFund >= nextTier.upgradePrice)
            {
                State.familyFund -= nextTier.upgradePrice;
                instance.tier = nextTier.tierLevel;
                instance.employeeIds.AddRange(new string[nextTier.employeeSlots - instance.employeeIds.Count]);
            }
        }

        public void AssignEmployeeToBusiness(string instanceId, int slotIndex, string characterId)
        {
            var instance = State.businesses.FirstOrDefault(b => b.instanceId == instanceId);
            if (instance == null || slotIndex < 0 || slotIndex >= instance.employeeIds.Count) return;

            string oldEmployeeId = instance.employeeIds[slotIndex];
            if (!string.IsNullOrEmpty(oldEmployeeId) && State.familyMembers.TryGetValue(oldEmployeeId, out var oldEmployee))
            {
                oldEmployee.status = CharacterStatus.Idle;
            }

            instance.employeeIds[slotIndex] = characterId;
            if (!string.IsNullOrEmpty(characterId) && State.familyMembers.TryGetValue(characterId, out var newEmployee))
            {
                newEmployee.careerTrackId = null;
                newEmployee.careerLevel = 0;
                newEmployee.status = CharacterStatus.Working;
            }
        }

        public void CustomizeAvatar(string characterId, List<string> keys, List<string> values)
        {
            if (State.familyFund < 10000) return;
            if (!State.familyMembers.TryGetValue(characterId, out var character)) return;

            State.familyFund -= 10000;
            character.avatarState_keys = new List<string>(keys);
            character.avatarState_values = new List<string>(values);
        }

        private void Emit() => OnStateChanged?.Invoke(State);
        public void Log(string message) { OnLog?.Invoke(message); }
        public void Save()
        {
            if (State == null) return;
            string json = JsonUtility.ToJson(State);
            string path = Path.Combine(Application.persistentDataPath, "savegame.json");
            File.WriteAllText(path, json);
            Debug.Log("Game saved to " + path);
        }
    }
}