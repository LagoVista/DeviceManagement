using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceStatusChangeRepo : IDeviceStatusChangeRepo
    {
        private readonly IOperationalDataStore<DeviceCurrentStatusRecord> _currentStore;
        private readonly IActivityRecordStore<DeviceStatusHistoryActivityRecord> _historyStore;
        public DeviceStatusChangeRepo(IOperationalDataStore<DeviceCurrentStatusRecord> currentStore, IActivityRecordStore<DeviceStatusHistoryActivityRecord> historyStore)
        {
            _currentStore = currentStore ?? throw new ArgumentNullException(nameof(currentStore));
            _historyStore = historyStore ?? throw new ArgumentNullException(nameof(historyStore));
        }
        public Task AddDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status) => _currentStore.UpsertAsync(DeviceCurrentStatusRecord.From(deviceRepo, status));
        public Task AddDeviceStatusHistoryAsync(DeviceRepository deviceRepo, DeviceStatus status) => _historyStore.InsertAsync(DeviceStatusHistoryActivityRecord.From(deviceRepo, status));
        public Task UpdateDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status) => _currentStore.UpsertAsync(DeviceCurrentStatusRecord.From(deviceRepo, status));
        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            var query = new HistoryQuery<DeviceStatusHistoryActivityRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .Where<string>(record => record.DeviceUniqueId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(request?.PageSize > 0 ? request.PageSize : 100));
            var page = await _historyStore.QueryAsync(query);
            return new ListResponse<DeviceStatus> { Model = page.Items.Select(item => item.ToModel()).ToList(), HasMoreRecords = page.HasMoreRecords, PageSize = request?.PageSize ?? 100, PageIndex = request?.PageIndex ?? 0 };
        }
        public async Task<DeviceStatus> GetDeviceStatusAsync(DeviceRepository deviceRepo, string deviceUniqueId)
        {
            var record = await _currentStore.GetAsync(deviceRepo.Id, deviceUniqueId);
            return record?.ToModel();
        }
        public async Task<ListResponse<DeviceStatus>> GetWatchdogDeviceStatusAsync(DeviceRepository deviceRepo, ListRequest request)
        {
            var query = new StorageQuery<DeviceCurrentStatusRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .WithPage(new StoragePageRequest(request?.PageSize > 0 ? request.PageSize : 100));
            var page = await _currentStore.QueryAsync(query);
            return new ListResponse<DeviceStatus> { Model = page.Items.Select(item => item.ToModel()).ToList(), HasMoreRecords = page.HasMoreRecords, PageSize = request?.PageSize ?? 100, PageIndex = request?.PageIndex ?? 0 };
        }
    }
}