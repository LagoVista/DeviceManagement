using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceAccountTransactionRepo
    {
        Task<InvokeResult<ConnectionSettings>> CreateDatabaseIfNeccesaryAsync(string dbName);

        void AddSettings(ConnectionSettings settings);

        Task<InvokeResult<decimal>> AddTransactionAsync(DeviceTransaction tx);

        Task<ListResponse<DeviceTransaction>> GetDeviceAccountTransactions(string deviceId, string key, ListRequest listRequest);

    }
}
