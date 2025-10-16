using UnityEngine;
using LifeSim.Core.Services;
using LifeSim.Core.Data;
using LifeSim.Core.Engine;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Infra;

public class Bootstrap : MonoBehaviour
{
    private GameEngine _engine;
    private ResourcesLocalization _loc;

    private void Start()
    {
        _loc = new ResourcesLocalization();
        _loc.SetLanguage("vi");

        Database.LoadAll("vi");

        var save = new JsonFileStore();
        var rng  = new DefaultRandom();
        var clk  = new FixedClock(1);

        _engine  = new GameEngine(new GameState(), save, _loc, rng, clk);
        _engine.Boot("vi");
        _engine.OnStateChanged += s =>
        {
            Debug.Log($"[STATE] Year {s.currentDate.year}, Fund {s.familyFund}");
        };

        // TODO: chạy Tick() bằng coroutine/time loop ở Presentation layer
    }
}