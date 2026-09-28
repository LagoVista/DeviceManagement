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
    public class SensorDataArchiveRepo : ISensorDataArchiveRepo
    {
        private readonly IActivityRecordStore<SensorDataArchiveActivityRecord> _store;
        public SensorDataArchiveRepo(IActivityRecordStore<SensorDataArchiveActivityRecord> store) { _store = store ?? throw new ArgumentNullException(nameof(store)); }
        public Task AddSensorDataArchiveAsync(DeviceRepository deviceRepo, SensorDataArchive archiveEntry) => _store.InsertAsync(SensorDataArchiveActivityRecord.From(deviceRepo, archiveEntry));
        public Task ClearSensorDataArchivesAsync(DeviceRepository deviceRepo, string deviceId) => Task.CompletedTask;
        public async Task<ListResponse<SensorDataArchive>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            var query = new HistoryQuery<SensorDataArchiveActivityRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .Where<string>(record => record.DeviceUniqueId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(request?.PageSize > 0 ? request.PageSize : 100));
            var page = await _store.QueryAsync(query);
            return new ListResponse<SensorDataArchive> { Model = page.Items.Select(item => item.ToModel()).ToList(), HasMoreRecords = page.HasMoreRecords, PageSize = request?.PageSize ?? 100, PageIndex = request?.PageIndex ?? 0 };
        }
    }
}