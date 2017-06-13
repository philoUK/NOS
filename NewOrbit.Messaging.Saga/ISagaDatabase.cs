using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga
{
    public interface ISagaDatabase
    {
        Task<bool> SagaExists(string id);
        Task Save(ISaga saga);
        Task<ISagaData> LoadSagaData(string id);
    }
}