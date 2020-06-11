using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.Logging.Loggers;
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
            SetTableName(deviceRepo.GetDeviceArchiveStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(new DeviceExceptionDTO(exception));
        }

        public async Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            SetTableName(deviceRepo.GetDeviceArchiveStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            var result = await base.GetPagedResultsAsync(deviceId, request);

            return new ListResponse<DeviceException>()
            {
                Model = result.Model.Select(dto=> dto.ToDeviceException()),
                NextPartitionKey = result.NextPartitionKey,
                NextRowKey = result.NextRowKey,
                PageIndex = result.PageIndex,
                PageCount = result.PageCount,
                PageSize = result.PageSize
            };
        }
    }
}
