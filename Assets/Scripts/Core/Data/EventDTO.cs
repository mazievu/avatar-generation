using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class EventChoiceDTO { public string id; public string labelKey; }
    [Serializable] public class GameEventDTO
    {
        public string id;
        public string characterStage;
        public string titleKey;
        public string bodyKey;
        public List<EventChoiceDTO> choices;
        public Dictionary<string,string> conditions;
        public Dictionary<string,int> rewards;
    }
    [Serializable] public class EventsFile { public string version="1"; public List<GameEventDTO> items = new(); }
}