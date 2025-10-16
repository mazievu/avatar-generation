using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Events
{
    [Serializable]
    public class EventChoice
    {
        public string id;
        public string labelKey; // localization key
    }

    [Serializable]
    public class GameEvent
    {
        public string id;
        public string characterId;    // the subject of this event
        public string titleKey;
        public string bodyKey;
        public List<EventChoice> choices = new List<EventChoice>();
    }
}