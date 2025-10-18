using LifeSim.Core.Domain.Game;

namespace LifeSim.Core.Services
{
    public interface ISaveStore
    {
        void Save(GameState state);
        GameState Load();
    }
}