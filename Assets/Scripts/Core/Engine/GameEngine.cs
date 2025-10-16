using System;
using System.Collections.Generic;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Domain.Events;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;

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

            State.currentDate.day += _clock.DaysPerTick;
            if (State.currentDate.day >= 365)
            {
                State.currentDate.year += State.currentDate.day / 365;
                State.currentDate.day = State.currentDate.day % 365;
            }

            foreach (var kv in State.familyMembers)
            {
                var c = kv.Value;
                if (!c.isAlive) continue;
                c.ageDays += _clock.DaysPerTick;
            }

            MaybeTriggerAmbientEvent();

            if (State.familyFund < -100000)
            {
                State.gameOverReason = "debt";
            }

            Emit();
        }

        private void MaybeTriggerAmbientEvent()
        {
            if (_rng.NextInt(0, 99) == 0 && State.activeEvent == null)
            {
                var id = "evt_" + Guid.NewGuid().ToString("N").Substring(0, 6);
                State.activeEvent = new GameEvent
                {
                    id = id,
                    characterId = "me",
                    titleKey = "event.sample.title",
                    bodyKey = "event.sample.body",
                    choices = new List<EventChoice>
                    {
                        new EventChoice{ id = "ok", labelKey = "common.ok" }
                    }
                };
            }
        }

        public void HandleEventChoice(string choiceId)
        {
            if (State.activeEvent == null) return;

            if (choiceId == "ok")
            {
                State.familyFund += _rng.NextInt(0, 1) == 0 ? 100 : -100;
            }

            State.activeEvent = null;
            Emit();
        }

        public void CloseEventModal()
        {
            State.activeEvent = null;
            Emit();
        }

        public void ClaimFeature(string featureId)
        {
            if (!State.claimedFeatures.Contains(featureId))
                State.claimedFeatures.Add(featureId);
            Emit();
        }

         public void Save()
        {
            _save.SaveAsync("save", State).Forget();
            Emit();
        }


        private void Emit() => OnStateChanged?.Invoke(State);

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}