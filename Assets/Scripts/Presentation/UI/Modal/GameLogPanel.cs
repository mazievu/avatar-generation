using UnityEngine;
using TMPro;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI.Panels
{
    public class GameLogPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text txtLog;
        GameEngine _engine; ILocalization _loc;

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e; _loc = l;
            _engine.OnLog += OnLog;
        }

        void OnDestroy(){ if (_engine!=null) _engine.OnLog -= OnLog; }

        void OnLog(string msg)
        {
            txtLog.text += "\n" + msg;
        }
    }
}