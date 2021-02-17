using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public interface IStoreProjections
    {
        Task<T> Get<T>(string id, string partitionKey);
        Task Add<T>(T projection) where T : IProjection;
        Task Replace<T>(T projection) where T : IProjection;
        Task Remove<T>(T projection) where T : IProjection;
    }
}