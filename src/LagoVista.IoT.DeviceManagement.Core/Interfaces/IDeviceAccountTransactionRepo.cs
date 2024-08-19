using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceAccountTransactionRepo
    {
        Task<InvokeResult<ConnectionSettings>> CreateDatabaseIfNeccesaryAsync(string dbName);

        void AddSettings(ConnectionSettings settings);

        Task<InvokeResult> AddTransactionAsync(DeviceTransaction tx);

        Task<ListResponse<DeviceTransaction>> GetDeviceAccountTransactions(string dbName, string deviceId, string key, ListRequest listRequest);

    }
}
