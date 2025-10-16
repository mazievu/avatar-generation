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
        _loc = new ResourcesLocalization();
        _loc.SetLanguage("en");         // hoặc "vi" khi bạn có JSON vi
        Database.LoadAll("en");         // nếu data phụ thuộc ngôn ngữ

        var save = new JsonFileStore();
        var rng  = new DefaultRandom();
        var clk  = new FixedClock(1);

        _engine  = new GameEngine(new GameState(), save, _loc, rng, clk);
        _engine.Boot("en");

        if (ui) ui.Bind(_engine, _loc);

        StartCoroutine(GameTick());
    }

    System.Collections.IEnumerator GameTick()
    {
        var wait = new WaitForSecondsRealtime(0.5f);
        while (true){ _engine.Tick(); yield return wait; }
    }
}