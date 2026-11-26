using LagoVista;
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    /// <summary>
    /// Mongo Application Data record for a mutable firmware download request.
    /// The public DeviceManagement request model remains storage-provider neutral.
    /// </summary>
    public sealed class FirmwareDownloadRequestRecord : IApplicationDataRecord
    {
        public NormalizedId32 Id { get; set; }
        public EntityHeader Organization { get; set; }
        public UtcTimestamp CreationDate { get; set; }
        public UtcTimestamp LastUpdatedDate { get; set; }

        public bool Expired { get; set; }
        public string Status { get; set; }
        public int PercentRequested { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceId { get; set; }
        public string FirmwareName { get; set; }
        public string Timestamp { get; set; }
        public string ExpiresUTC { get; set; }
        public string FirmwareId { get; set; }
        public string FirmwareRevisionId { get; set; }
        public string Error { get; set; }

        public static FirmwareDownloadRequestRecord FromRequest(FirmwareDownloadRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(request.DownloadId)) throw new ArgumentException("Firmware download ID is required.", nameof(request));
            if (String.IsNullOrWhiteSpace(request.OrgId)) throw new ArgumentException("Firmware download organization ID is required.", nameof(request));

            return new FirmwareDownloadRequestRecord
            {
                Id = NormalizedId32.Parse(request.DownloadId),
                Organization = EntityHeader.Create(request.OrgId, request.OrgId),
                CreationDate = UtcTimestamp.Parse(request.Timestamp),
                LastUpdatedDate = UtcTimestamp.Now,
                Expired = request.Expired,
                Status = request.Status,
                PercentRequested = request.PercentRequested,
                DeviceRepoId = request.DeviceRepoId,
                DeviceId = request.DeviceId,
                FirmwareName = request.FirmwareName,
                Timestamp = request.Timestamp,
                ExpiresUTC = request.ExpiresUTC,
                FirmwareId = request.FirmwareId,
                FirmwareRevisionId = request.FirmwareRevisionId,
                Error = request.Error
            };
        }

        public FirmwareDownloadRequest ToRequest()
        {
            return new FirmwareDownloadRequest
            {
                DownloadId = Id.Value,
                OrgId = Organization?.Id,
                Expired = Expired,
                Status = Status,
                PercentRequested = PercentRequested,
                DeviceRepoId = DeviceRepoId,
                DeviceId = DeviceId,
                FirmwareName = FirmwareName,
                Timestamp = Timestamp,
                ExpiresUTC = ExpiresUTC,
                FirmwareId = FirmwareId,
                FirmwareRevisionId = FirmwareRevisionId,
                Error = Error
            };
        }
    }

    /// <summary>
    /// Persists mutable firmware download request state in the shared Mongo
    /// Application Data store. Download IDs remain globally addressable; history
    /// queries are always scoped to the caller's organization.
    /// </summary>
    public sealed class FirmwareDownloadRequestRepo
    {
        private const int QueryPageSize = 1000;
        private readonly IApplicationDataStore _store;

        public FirmwareDownloadRequestRepo(IApplicationDataStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task AddRequestAsync(FirmwareDownloadRequest request)
        {
            return _store.InsertAsync(FirmwareDownloadRequestRecord.FromRequest(request));
        }

        public async Task<IEnumerable<FirmwareDownloadRequest>> GetForDeviceAsync(string orgId, string deviceRepoId, string deviceId)
        {
            if (String.IsNullOrWhiteSpace(orgId)) throw new ArgumentNullException(nameof(orgId));
            if (String.IsNullOrWhiteSpace(deviceRepoId)) throw new ArgumentNullException(nameof(deviceRepoId));
            if (String.IsNullOrWhiteSpace(deviceId)) throw new ArgumentNullException(nameof(deviceId));

            var requests = new List<FirmwareDownloadRequest>();
            string continuationToken = null;

            do
            {
                var query = new StorageQuery<FirmwareDownloadRequestRecord>()
                    .Where(record => record.Organization.Id, StorageFilterOperator.Equal, orgId)
                    .Where(record => record.DeviceRepoId, StorageFilterOperator.Equal, deviceRepoId)
                    .Where(record => record.DeviceId, StorageFilterOperator.Equal, deviceId)
                    .OrderBy(record => record.Timestamp, StorageSortDirection.Descending)
                    .WithPage(new StoragePageRequest(QueryPageSize, continuationToken));

                var page = await _store.QueryAsync(query).ConfigureAwait(false);
                requests.AddRange(page.Items.Select(record => record.ToRequest()));
                continuationToken = page.ContinuationToken;
            }
            while (!String.IsNullOrWhiteSpace(continuationToken));

            return requests;
        }

        public async Task<FirmwareDownloadRequest> GetRequestAsync(string requestId)
        {
            if (String.IsNullOrWhiteSpace(requestId)) throw new ArgumentNullException(nameof(requestId));

            var record = await _store.GetAsync<FirmwareDownloadRequestRecord>(new StorageKey(requestId)).ConfigureAwait(false);
            return record?.ToRequest();
        }

        public Task UpdateRequestAsync(FirmwareDownloadRequest request)
        {
            return _store.UpdateAsync(FirmwareDownloadRequestRecord.FromRequest(request));
        }
    }
}
