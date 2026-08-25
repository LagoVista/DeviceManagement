using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceConnectionEventRepo : IDeviceConnectionEventRepo
    {
        private readonly IActivityRecordStore<DeviceConnectionEvent> _store;

        public DeviceConnectionEventRepo(IActivityRecordStore<DeviceConnectionEvent> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static void ConfigureStorage(FlatStorageDefinition<DeviceConnectionEvent> definition)
        {
            definition
                .PartitionBy(record => record.OrganizationId)
                .PartitionBy(record => record.DeviceId);
        }

        public Task AddConnectionEventAsync(DeviceRepository deviceRepo, DeviceConnectionEvent connectionEvent)
        {
            if (deviceRepo == null) throw new ArgumentNullException(nameof(deviceRepo));
            if (connectionEvent == null) throw new ArgumentNullException(nameof(connectionEvent));
            if (String.IsNullOrWhiteSpace(connectionEvent.DeviceId)) throw new ArgumentException("Device id is required.", nameof(connectionEvent));
            if (deviceRepo.OwnerOrganization == null || String.IsNullOrWhiteSpace(deviceRepo.OwnerOrganization.Id))
            {
                throw new InvalidOperationException("Device repository must have an owner organization before connection history can be stored.");
            }

            connectionEvent.Id = String.IsNullOrWhiteSpace(connectionEvent.Id) ? Guid.NewGuid().ToId() : connectionEvent.Id;
            connectionEvent.OrganizationId = deviceRepo.OwnerOrganization.Id;
            connectionEvent.Organization = deviceRepo.OwnerOrganization.Text;

            if (connectionEvent.CreationDate == default)
            {
                connectionEvent.CreationDate = DateTime.TryParse(connectionEvent.TimeStamp, out var timestamp)
                    ? timestamp.ToUniversalTime()
                    : DateTime.UtcNow;
            }

            return _store.InsertAsync(connectionEvent);
        }

        public async Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest)
        {
            if (deviceRepo == null) throw new ArgumentNullException(nameof(deviceRepo));
            if (String.IsNullOrWhiteSpace(deviceId)) throw new ArgumentNullException(nameof(deviceId));
            if (deviceRepo.OwnerOrganization == null || String.IsNullOrWhiteSpace(deviceRepo.OwnerOrganization.Id))
            {
                throw new InvalidOperationException("Device repository must have an owner organization before connection history can be queried.");
            }

            listRequest ??= ListRequest.Create();
            var pageSize = listRequest.PageSize <= 0 ? 100 : Math.Min(listRequest.PageSize, 1000);
            var query = new HistoryQuery<DeviceConnectionEvent>()
                .Where(record => record.OrganizationId, StorageFilterOperator.Equal, deviceRepo.OwnerOrganization.Id)
                .Where(record => record.DeviceId, StorageFilterOperator.Equal, deviceId)
                .WithPage(new StoragePageRequest(pageSize, listRequest.NextRowKey));

            var result = await _store.QueryAsync(query);
            return new ListResponse<DeviceConnectionEvent>()
            {
                Model = result.Items,
                NextPartitionKey = result.HasMoreRecords ? "activity" : null,
                NextRowKey = result.ContinuationToken,
                PageIndex = listRequest.PageIndex,
                PageSize = pageSize
            };
        }
    }
}
