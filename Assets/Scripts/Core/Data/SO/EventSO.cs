// FILE: Assets/Scripts/Core/Data/SO/EventSO.cs

using UnityEngine;
using System.Collections.Generic;
using LifeSim.Core.Domain.Characters; // Đảm bảo namespace này đúng với project của bạn

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewEvent", menuName = "LifeSim/Event")]
    public class EventSO : ScriptableObject
    {
        [Header("Event Metadata")]
        public EventType eventType;
        public LifePhase lifePhase;
        public List<string> triggerConditions; // Could be complex conditions parsed at runtime

        [Header("Content")]
        public string titleKey; // Localization key
        public string descriptionKey; // Localization key
        public List<EventChoice> choices;

        [System.Serializable]
        public class EventChoice
        {
            public string choiceId;
            public string choiceKey; // Localization key
            public List<Effect> effects;
            public string triggersEvent; // ID of event to trigger next
        }

        [System.Serializable]
        public class Effect
        {
            public EffectType type;
            public string targetStat; // e.g., "happiness", "health", "iq", "eq", "skill", "familyFund"
            public int value;
            public string logKey; // Localization key for the result log
        }

        // Các enum này nên được chuyển ra file riêng nếu dùng ở nhiều nơi
        public enum EventType { Random, Milestone, TriggerOnly, Priority }
        public enum EffectType { StatChange, FundChange, TriggerEvent, SetFlag }
    }
}