// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c3b2ffe1310268e75297bd1656a880115a59f4716d6a826da56922ea230b601b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceConnectionEventRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceConnectionEventDTO>, IDeviceConnectionEventRepo
    {
        public DeviceConnectionEventRepo(IAdminLogger logger) : base(logger)
        {
        }

        public async Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest)
        {
            SetTableName(deviceRepo.GetDeviceConnectionEventStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            var result = await base.GetPagedResultsAsync(deviceId, listRequest);
            return result.Create(result.Model.Select(dto => dto.ToDeviceConnectionEvent()));
        }
    }
}
