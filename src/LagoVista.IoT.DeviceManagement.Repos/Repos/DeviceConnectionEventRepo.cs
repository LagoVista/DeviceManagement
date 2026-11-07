// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c3b2ffe1310268e75297bd1656a880115a59f4716d6a826da56922ea230b601b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceConnectionEventRepo : IDeviceConnectionEventRepo
    {
        private readonly IActivityRecordStore<DeviceConnectionActivityRecord> _store;

        public DeviceConnectionEventRepo(IActivityRecordStore<DeviceConnectionActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<InvokeResult> AddDeviceConnectionEventAsync(DeviceRepository deviceRepo, DeviceConnectionEvent connectionEvent)
        {
            await _store.InsertAsync(DeviceConnectionActivityRecord.From(deviceRepo, connectionEvent));
            return InvokeResult.Success;
        }

        public async Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest)
        {
            var query = new HistoryQuery<DeviceConnectionActivityRecord>()
                .Where<string>(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.Id)
                .Where<string>(record => record.DeviceUniqueId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(listRequest?.PageSize > 0 ? listRequest.PageSize : 100));
            var page = await _store.QueryAsync(query);
            return new ListResponse<DeviceConnectionEvent>
            {
                Model = page.Items.Select(item => item.ToModel()).ToList(),
                HasMoreRecords = page.HasMoreRecords,
                PageSize = listRequest?.PageSize ?? 100,
                PageIndex = listRequest?.PageIndex ?? 0
            };
        }
    }
}
