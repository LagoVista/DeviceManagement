using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceProximityObservationRepo : IDeviceProximityObservationRepo
    {
        private readonly IActivityRecordStore<DeviceProximityObservationActivityRecord> _store;
        public DeviceProximityObservationRepo(IActivityRecordStore<DeviceProximityObservationActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task WriteAsync(IReadOnlyList<DeviceProximityObservation> observations)
        {
            if (observations == null) throw new ArgumentNullException(nameof(observations));
            foreach (var observation in observations)
            {
                if (String.IsNullOrWhiteSpace(observation.OrganizationId) ||
                    String.IsNullOrWhiteSpace(observation.DeviceRepoId) ||
                    String.IsNullOrWhiteSpace(observation.DeviceUniqueId) ||
                    String.IsNullOrWhiteSpace(observation.ObservedDeviceId))
                    throw new ArgumentException("An observation is missing required identity fields.", nameof(observations));
                await _store.InsertAsync(DeviceProximityObservationActivityRecord.From(observation));
            }
        }

        public async Task<ListResponse<DeviceProximityObservation>> QueryAsync(string organizationId, string deviceRepoId, string deviceUniqueId, ListRequest request, DateTime? startUtc = null, DateTime? endUtc = null)
        {
            var pageSize = Math.Min(1000, request?.PageSize > 0 ? request.PageSize : 100);
            var pageIndex = Math.Max(1, request?.PageIndex ?? 1);
            var partition = deviceRepoId + "/" + deviceUniqueId;
            var hasCursor = request?.HasCursor == true &&
                            String.Equals(request.NextPartitionKey, partition, StringComparison.Ordinal);
            var query = new HistoryQuery<DeviceProximityObservationActivityRecord>()
                .Where<string>(item => item.OrganizationId, StorageFilterOperator.Equal, organizationId)
                .Where<string>(item => item.DeviceRepoId, StorageFilterOperator.Equal, deviceRepoId)
                .Where<string>(item => item.DeviceUniqueId, StorageFilterOperator.Equal, deviceUniqueId);
            // Exact UTC bounds take precedence; retain legacy calendar-day headers for other callers.
            if (startUtc.HasValue || endUtc.HasValue)
                query.Between(startUtc, endUtc);
            else if (request?.StartDate.HasValue == true || request?.EndDate.HasValue == true)
                query.Between(request.StartDate?.ToDateTime(), request.EndDate?.ToDateTime());

            var token = hasCursor ? request.NextRowKey : null;
            StoragePageResult<DeviceProximityObservationActivityRecord> page = null;
            var pagesToRead = hasCursor ? 1 : pageIndex;
            for (var i = 0; i < pagesToRead; i++)
            {
                page = await _store.QueryAsync(query.WithPage(new StoragePageRequest(pageSize, token)));
                token = page.ContinuationToken;
                if (String.IsNullOrEmpty(token) && i + 1 < pagesToRead)
                {
                    page = new StoragePageResult<DeviceProximityObservationActivityRecord>(Array.Empty<DeviceProximityObservationActivityRecord>());
                    break;
                }
            }
            return new ListResponse<DeviceProximityObservation>
            {
                Model = page.Items.Select(item => item.ToModel()).ToList(),
                HasMoreRecords = page.HasMoreRecords,
                NextPartitionKey = page.HasMoreRecords ? partition : null,
                NextRowKey = page.ContinuationToken,
                PageSize = pageSize,
                PageIndex = request?.PageIndex ?? 1
            };
        }
    }
}
