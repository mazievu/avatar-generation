using UnityEngine;
using System.Collections;
using LifeSim.Core.Engine;

namespace LifeSim.Presentation.Services
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] float autosaveInterval = 60f;
        GameEngine _engine;

        public void Init(GameEngine engine)
        {
            _engine = engine;
            StartCoroutine(AutoSave());
        }

        IEnumerator AutoSave()
        {
            var wait = new WaitForSecondsRealtime(autosaveInterval);
            while (true)
            {
                yield return wait;
                _engine?.Save();
                Debug.Log("[AutoSave] Game saved");
            }
        }
    }
}