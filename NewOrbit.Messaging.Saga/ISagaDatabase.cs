using System.Threading.Tasks;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Saga
{
    public interface ISagaDatabase
    {
        Task<bool> SagaExists(IMessage msg);
        Task Save(ISaga saga);
        Task<ISagaData> LoadSagaData(ISaga saga);
    }
}