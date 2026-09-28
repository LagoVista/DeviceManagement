using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceArchiveRepo : IDeviceArchiveRepo
    {
        private readonly IDeviceArchiveReportUtils _deviceArchiveReportUtils;
        private readonly IActivityRecordStore<DeviceArchiveActivityRecord> _store;
        public DeviceArchiveRepo(IDeviceArchiveReportUtils deviceArchiveReportUtils, IActivityRecordStore<DeviceArchiveActivityRecord> store)
        {
            _deviceArchiveReportUtils = deviceArchiveReportUtils ?? throw new ArgumentNullException(nameof(deviceArchiveReportUtils));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }
        public Task AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive archiveEntry) => _store.InsertAsync(DeviceArchiveActivityRecord.From(deviceRepo, archiveEntry));
        public Task ClearDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId) => Task.CompletedTask;
        public async Task<ListResponse<System.Collections.Generic.List<object>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            var query = new HistoryQuery<DeviceArchiveActivityRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .Where<string>(record => record.DeviceUniqueId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(request?.PageSize > 0 ? request.PageSize : 100));
            var page = await _store.QueryAsync(query);
            var response = _deviceArchiveReportUtils.CreateNormalizedDeviceArchiveResult(page.Items.Select(item => item.ToReportRow()).ToList());
            response.HasMoreRecords = page.HasMoreRecords;
            return response;
        }
    }
}