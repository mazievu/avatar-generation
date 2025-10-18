// FILE: Assets\Scripts\Presentation\UI\Panels\GameLogPanel.cs

using UnityEngine;
using System.Collections.Generic;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;

namespace LifeSim.Presentation.UI.Panels
{
    public class GameLogPanel : MonoBehaviour
    {
        [Header("Prefab & Container")]
        [SerializeField] private LogEntryUI logEntryPrefab;
        [SerializeField] private Transform container;

        private GameEngine _engine;
        private ILocalization _loc;
        private List<LogEntryUI> _entries = new List<LogEntryUI>();

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e;
            _loc = l;
            _engine.OnStateChanged += Refresh;
            Refresh(_engine.State);
        }

        void OnDestroy()
        {
            if (_engine != null) _engine.OnStateChanged -= Refresh;
        }

        public void Refresh(GameState state)
        {
            if (state == null) return;

            // Simple refresh, could be optimized by only adding new entries
            foreach (var entry in _entries)
            {
                Destroy(entry.gameObject);
            }
            _entries.Clear();

            // === LỖI Ở ĐÂY ===
            // Sửa "state.gameLog" thành "state.GameLog"
            foreach (var logData in state.GameLog) // <- SỬA DÒNG NÀY
            {
                LogEntryUI instance = Instantiate(logEntryPrefab, container);
                instance.Setup(logData, _loc);
                _entries.Add(instance);
            }
        }
    }
}