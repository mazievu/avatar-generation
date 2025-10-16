using System;

namespace LifeSim.Core.Domain.Events
{
    [Serializable]
    public class EventChoice
    {
        public string id;
        public string labelKey; // localization key
    }
}
