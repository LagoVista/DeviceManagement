using System;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class FirmwareRepo : DocumentDBRepoBase<Firmware>, IFirmwareRepo
    {
        private readonly IFirmwareRepoSettings _repoSettings;
        private readonly IAdminLogger _adminLogger;

        public FirmwareRepo(IFirmwareRepoSettings repoSettings, IAdminLogger logger)
            : base(repoSettings.FirmwareDocDBSettings.Uri, repoSettings.FirmwareDocDBSettings.AccessKey, repoSettings.FirmwareDocDBSettings.ResourceName, logger)
        {
            _repoSettings = repoSettings ?? throw new ArgumentNullException(nameof(repoSettings));
            _adminLogger = logger ?? throw new ArgumentNullException(nameof(repoSettings));
        }

        protected override bool ShouldConsolidateCollections => true;

        public Task AddDownloadRequestAsync(FirmwareDownloadRequest request)
        {
            var downloadRequestRepo = new FirmwareDownloadRequestRepo(_repoSettings.FirmwareRequestSettings.AccountId, _repoSettings.FirmwareRequestSettings.AccessKey, _adminLogger);
            return downloadRequestRepo.AddRequestAsync(request);
        }

        public Task AddFirmwareAsync(Firmware firmware)
        {
            return CreateDocumentAsync(firmware);
        }

        private string GetFileName(string firmwareId, string revisionId)
        {
            return $"{firmwareId}.{revisionId}.bin";
        }

        public Task AddFirmwareRevisionAsync(string firmwareId, string revisionId, byte[] buffer)
        {
            var binRepo = new FirmwareBinRepo(_adminLogger, _repoSettings.FirmwareBinSettings);
            return binRepo.AddBinAsync(buffer, GetFileName(firmwareId, revisionId));
        }

        public Task DeleteFirmwareAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<FirmwareDownloadRequest> GetDownloadRequestAsync(string id)
        {
            var downloadRequestRepo = new FirmwareDownloadRequestRepo(_repoSettings.FirmwareRequestSettings.AccountId, _repoSettings.FirmwareRequestSettings.AccessKey, _adminLogger);
            return downloadRequestRepo.GetRequestAsync(id);
        }

        public Task<InvokeResult<byte[]>> GetFirmareBinaryAsync(string firmwareId, string revisionId)
        {
            var binRepo = new FirmwareBinRepo(_adminLogger, _repoSettings.FirmwareBinSettings);
            return binRepo.GetMediaAsync(GetFileName(firmwareId, revisionId));
        }

        public Task<Firmware> GetFirmwareAsync(string firmwareId)
        {
            return GetDocumentAsync(firmwareId);
        }

        public async Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, ListRequest listRequest)
        {
            var items = (await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId), listRequest)).Model;
            return ListResponse<FirmwareSummary>.Create(items.Select(itm => itm.CreateSummary()));

        }

        public async Task<bool> QueryKeyInUseAsync(string key, string org)
        {
            return (await base.QueryAsync(attr => (attr.OwnerOrganization.Id == org && attr.Key == key))).Any();
        }

        public Task UpdateDownloadRequestAsync(FirmwareDownloadRequest request)
        {
            var downloadRequestRepo = new FirmwareDownloadRequestRepo(_repoSettings.FirmwareRequestSettings.AccountId, _repoSettings.FirmwareRequestSettings.AccessKey, _adminLogger);
            return downloadRequestRepo.UpdateRequestAsync(request);
        }

        public Task UpdateFirmwareAsync(Firmware firmware)
        {
            return UpsertDocumentAsync(firmware);
        }

        public async Task<ListResponse<FirmwareDownloadRequest>> GetDownloadRequestsForDeviceAsync(string deviceRepoId, string deviceId)
        {
            var downloadRequestRepo = new FirmwareDownloadRequestRepo(_repoSettings.FirmwareRequestSettings.AccountId, _repoSettings.FirmwareRequestSettings.AccessKey, _adminLogger);
            var requests = await downloadRequestRepo.GetForDeviceAsync(deviceRepoId, deviceId);
            requests = requests.OrderByDescending(rqst => rqst.Timestamp);
            return ListResponse<FirmwareDownloadRequest>.Create(requests);
        }
    }
}
