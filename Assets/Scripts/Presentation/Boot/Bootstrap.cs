using UnityEngine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Infra;
using LifeSim.Presentation.UI;

public class Bootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] GameUIController ui;

    GameEngine _engine;
    ResourcesLocalization _loc;

    void Start()
    {
        var save = new JsonFileStore();
        var loadedState = save.Load();

        _loc = new ResourcesLocalization();
        _loc.SetLanguage(loadedState?.lang ?? "en");
        Database.LoadAll(loadedState?.lang ?? "en");

        var rng  = new DefaultRandom();
        var clk  = new FixedClock(1);

        _engine  = new GameEngine(loadedState ?? new GameState(), save, _loc, rng, clk);
        _engine.Boot(_loc.CurrentLanguage);

        if (ui) ui.Bind(_engine, _loc);

        var auto = FindAnyObjectByType<LifeSim.Presentation.UI.PanelAutoBinder>();
        if (auto == null)
        {
            var go = new GameObject("PanelAutoBinder");
            auto = go.AddComponent<LifeSim.Presentation.UI.PanelAutoBinder>();
        }
        auto.Init(_engine, _loc);

        var avatarPreview = FindObjectOfType<LifeSim.Presentation.UI.AvatarPreview>(true);
        if (avatarPreview != null)
        {
            // You can set the age from game state here, for now just redraw
            avatarPreview.Redraw();
        }

        StartCoroutine(GameTick());
    }

    System.Collections.IEnumerator GameTick()
    {
        var wait = new WaitForSecondsRealtime(0.5f);
        while (true){ _engine.Tick(); yield return wait; }
    }
}