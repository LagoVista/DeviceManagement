using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceExceptionRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceExceptionDTO>, IDeviceExceptionRepo
    {
        public DeviceExceptionRepo( IAdminLogger logger) : base(logger)
        {
        }

        public Task AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException exception)
        {
            SetTableName(deviceRepo.GetDeviceExceptionsStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(new DeviceExceptionDTO(exception, false));
        }

        public Task AddDeviceExceptionClearedAsync(DeviceRepository deviceRepo, DeviceException exception)
        {
            SetTableName(deviceRepo.GetDeviceExceptionsStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return InsertAsync(new DeviceExceptionDTO(exception, true));
        }

        public Task ClearDeviceExceptionsAsync(DeviceRepository deviceRepo, string id)
        {
            return Task.CompletedTask;    
        }

        public async Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetTableName(deviceRepo.GetDeviceExceptionsStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            var result = await base.GetPagedResultsAsync(deviceId, request);
            return ListResponse<DeviceException>.Create(result.Model.Select(dto => dto.ToDeviceException()), result);
        }
    }
}
