using System.Collections.Generic;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Domain.Events
{
    public static class EventSystem
    {
        public static void ApplyChoice(GameState s, GameEvent ev, string choiceId)
        {
            if (ev == null) return;
            if (!s.familyMembers.TryGetValue(ev.characterId, out Character c)) return;

            switch (choiceId)
            {
                case "ok":
                    break;
                case "boost_happiness":
                    c.stats.happiness += 5;
                    c.stats.Clamp();
                    break;
                case "fine_money":
                    s.familyFund -= 200;
                    break;
            }
        }

        public static GameEvent PickRandomAmbientEvent(string characterId)
        {
            return new GameEvent
            {
                id = "evt_sample",
                characterId = characterId,
                titleKey = "event.sample.title",
                bodyKey = "event.sample.body",
                choices = new List<EventChoice>
                {
                    new EventChoice{ id="ok", labelKey="common.ok" }
                }
            };
        }
    }
}