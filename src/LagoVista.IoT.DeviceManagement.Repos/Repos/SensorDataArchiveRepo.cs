using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class SensorDataArchiveRepo : TableStorageBase<SensorDataArchive>, ISensorDataArchiveRepo
    {

        public SensorDataArchiveRepo(IAdminLogger adminLogger) : base(adminLogger)
        {
        }

        public Task AddSensorDataArchiveAsync(DeviceRepository deviceRepo, SensorDataArchive archiveEntry)
        {
            SetTableName(deviceRepo.GetSensorDataStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            return InsertAsync(archiveEntry);
        }

   
        public Task ClearSensorDataArchivesAsync(DeviceRepository deviceRepo, string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ListResponse<SensorDataArchive>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest)
        {
            SetTableName(deviceRepo.GetSensorDataStorageName());
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);

            return await base.GetPagedResultsAsync(deviceId, listRequest);
        }
    }
}
