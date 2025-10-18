
using UnityEngine;
using TMPro;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class LogEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text logText;

        public void Setup(GameLogEntry logEntry, ILocalization loc)
        {
            string message = loc.T(logEntry.messageKey, "(Log message not found)");
            // TODO: Implement argument formatting if messageKey supports it

            logText.text = $"[Year {logEntry.year}] {logEntry.characterName}: {message}";
        }
    }
}
