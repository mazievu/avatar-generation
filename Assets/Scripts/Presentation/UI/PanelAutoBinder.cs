using UnityEngine;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;

namespace LifeSim.Presentation.UI
{
    public class PanelAutoBinder : MonoBehaviour
    {
        public void Init(GameEngine engine, ILocalization loc)
        {
            var bindables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var b in bindables)
            {
                var method = b.GetType().GetMethod("Bind");
                if (method != null)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 2 && parameters[0].ParameterType == typeof(GameEngine) && parameters[1].ParameterType == typeof(ILocalization))
                    {
                        method.Invoke(b, new object[] { engine, loc });
                    }
                }
            }
        }
    }
}