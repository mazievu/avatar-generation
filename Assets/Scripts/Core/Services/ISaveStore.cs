using System.Threading.Tasks;

namespace LifeSim.Core.Services
{
    public interface ISaveStore
    {
        Task SaveAsync<T>(string key, T data);
        Task<T> LoadAsync<T>(string key, T fallback = default);
    }
}
