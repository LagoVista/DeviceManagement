// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b10403d7cffb26838031cd554f0c98a675ad8f3849718edb5c43344addd0de81
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        Task<ListResponse<DeviceAccountTransactionRecord>> GetDeviceAccountTransactions(string deviceId, string locatoinId, string key, ListRequest listRequest);
    }
}
