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
    public class DeviceExceptionRepo : IDeviceExceptionRepo
    {
        private readonly IActivityRecordStore<DeviceExceptionActivityRecord> _store;
        public DeviceExceptionRepo(IActivityRecordStore<DeviceExceptionActivityRecord> store) { _store = store ?? throw new ArgumentNullException(nameof(store)); }
        public Task AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException exception) => _store.InsertAsync(DeviceExceptionActivityRecord.From(deviceRepo, exception, false));
        public Task AddDeviceExceptionClearedAsync(DeviceRepository deviceRepo, DeviceException exception) => _store.InsertAsync(DeviceExceptionActivityRecord.From(deviceRepo, exception, true));
        public Task ClearDeviceExceptionsAsync(DeviceRepository deviceRepo, string id) => Task.CompletedTask;
        public async Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            var query = new HistoryQuery<DeviceExceptionActivityRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .Where<string>(record => record.DeviceUniqueId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(request?.PageSize > 0 ? request.PageSize : 100));
            var page = await _store.QueryAsync(query);
            return new ListResponse<DeviceException> { Model = page.Items.Select(item => item.ToModel()).ToList(), HasMoreRecords = page.HasMoreRecords, PageSize = request?.PageSize ?? 100, PageIndex = request?.PageIndex ?? 0 };
        }
    }
}