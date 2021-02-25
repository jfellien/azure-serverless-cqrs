using System.Collections.Generic;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public interface IStoreProjections
    {
        Task<T> Get<T>(string id)  where T : IProjection, new();
        Task<IEnumerable<T>> GetAll<T>()  where T : IProjection, new();
        Task Add<T>(T projection) where T : IProjection;
        Task Replace<T>(T projection) where T : IProjection;
        Task Remove<T>(T projection) where T : IProjection;
    }
}