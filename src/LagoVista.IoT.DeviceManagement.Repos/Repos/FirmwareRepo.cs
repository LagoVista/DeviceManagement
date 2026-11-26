using LagoVista.CloudStorage.DocumentDB;
using LagoVista.CloudStorage.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class FirmwareRepo : DocumentDBRepoBase<Firmware>, IFirmwareRepo
    {
        private readonly FirmwareDownloadRequestRepo _downloadRequestRepo;
        private readonly ICloudFileStorageClient _cloudFileStorageClient;

        public FirmwareRepo(
            FirmwareDownloadRequestRepo downloadRequestRepo,
            ICloudFileStorageClient cloudFileStorageClient,
            IDocumentCloudCachedServices services) : base(services)
        {
            _downloadRequestRepo = downloadRequestRepo ?? throw new ArgumentNullException(nameof(downloadRequestRepo));
            _cloudFileStorageClient = cloudFileStorageClient ?? throw new ArgumentNullException(nameof(cloudFileStorageClient));
        }

        public Task AddDownloadRequestAsync(FirmwareDownloadRequest request)
        {
            return _downloadRequestRepo.AddRequestAsync(request);
        }

        public Task AddFirmwareAsync(Firmware firmware)
        {
            return CreateDocumentAsync(firmware);
        }

        private string GetFileName(string type, string firmwareId, string revisionId)
        {
            return $"{type}.{firmwareId}.{revisionId}.bin";
        }

        public async Task<string> AddFirmwareRevisionAsync(string type, string firmwareId, string revisionId, byte[] buffer)
        {
            var fileName = GetFileName(type, firmwareId, revisionId);
            var binRepo = new FirmwareBinRepo(_cloudFileStorageClient);
            await binRepo.AddBinAsync(buffer, fileName);
            return fileName;
        }

        public Task DeleteFirmwareAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<FirmwareDownloadRequest> GetDownloadRequestAsync(string id)
        {
            return _downloadRequestRepo.GetRequestAsync(id);
        }

        public Task<InvokeResult<byte[]>> GetFirmareBinaryAsync(string type, string firmwareId, string revisionId)
        {
            var binRepo = new FirmwareBinRepo(_cloudFileStorageClient);
            return binRepo.GetFirmwareBinaryAsync(GetFileName(type, firmwareId, revisionId));
        }

        public Task<Firmware> GetFirmwareAsync(string firmwareId)
        {
            return GetDocumentAsync(firmwareId);
        }

        public Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<FirmwareSummary, Firmware>(attr => (attr.OwnerOrganization.Id == orgId), attr => attr.Name, listRequest);
        }

        public async Task<bool> QueryKeyInUseAsync(string key, string org)
        {
            return (await base.QueryAsync(attr => (attr.OwnerOrganization.Id == org && attr.Key == key))).Any();
        }

        public Task UpdateDownloadRequestAsync(FirmwareDownloadRequest request)
        {
            return _downloadRequestRepo.UpdateRequestAsync(request);
        }

        public Task UpdateFirmwareAsync(Firmware firmware)
        {
            return UpsertDocumentAsync(firmware);
        }

        public async Task<ListResponse<FirmwareDownloadRequest>> GetDownloadRequestsForDeviceAsync(string orgId, string deviceRepoId, string deviceId)
        {
            var requests = await _downloadRequestRepo.GetForDeviceAsync(orgId, deviceRepoId, deviceId).ConfigureAwait(false);
            return ListResponse<FirmwareDownloadRequest>.Create(requests);
        }
    }
}
