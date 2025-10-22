// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c96fccd4ca88fb2c1f9fba460ff41ce2e6e22c8fbfdfe69412c7a73f72c922ec
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceMediaItemRepo : TableStorageBase<DeviceMediaTableStorageEntity>, IDeviceMediaItemRepo
    {
        public DeviceMediaItemRepo(IAdminLogger logger) : base(logger)
        {
        }

        public async Task DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId)
        {
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            SetTableName(repo.GetDeviceMediaStorageName());
            var item = await GetAsync(deviceId, itemId);
            await base.RemoveAsync(item);
        }

        public async Task<DeviceMedia> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId)
        {
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            SetTableName(repo.GetDeviceMediaStorageName());

            return (await GetAsync(deviceId, itemId)).ToDeviceMedia();
        }

        public async Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, ListRequest request)
        {
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            SetTableName(repo.GetDeviceMediaStorageName());
            
            var items = await GetPagedResultsAsync(deviceId, request);
           
            var response = new ListResponse<DeviceMedia>();
            response.Model = items.Model.Select(itm => itm.ToDeviceMedia());
            response.NextPartitionKey = items.NextPartitionKey;
            response.NextRowKey = items.NextRowKey;
            response.PageCount = items.PageCount;
            response.PageIndex = items.PageIndex;
            response.HasMoreRecords = items.HasMoreRecords;
            response.PageSize = items.PageSize;
            return response;
        }

        public async Task<InvokeResult> StoreMediaItemAsync(DeviceRepository repo, DeviceMedia media)
        {
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            SetTableName(repo.GetDeviceMediaStorageName());

            await base.InsertAsync(DeviceMediaTableStorageEntity.FromDeviceMedia(media));
            return InvokeResult.Success;
        }
    }
}
