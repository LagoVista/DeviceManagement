using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class FirmwareManager : ManagerBase, IFirmwareManager
    {
        IFirmwareRepo _repo;

        public FirmwareManager(IFirmwareRepo repo, IAdminLogger logger,
                IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security)
            : base(logger, appConfig, dependencyManager, security)
        {
            _repo = repo;
        }


        public async Task<InvokeResult> AddFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(firmware, Actions.Create);
            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _repo.AddFirmwareAsync(firmware);

            return InvokeResult.Success;
        }

        public async Task<Firmware> GetFirmwareAsync(string id, EntityHeader org, EntityHeader user)
        {
            var firmware = await _repo.GetFirmwareAsync(id);

            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Read, user, org);

            return firmware;
        }

        public async Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Firmware), Actions.Read);
            return await _repo.GetFirmwareForOrgAsync(orgId, listRequest);
        }

        public async Task<InvokeResult> UpdateFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(firmware, Actions.Update);
            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Update, user, org);

            await _repo.UpdateFirmwareAsync(firmware);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteFirmwareAsync(string id, EntityHeader org, EntityHeader user)
        {
            var firmware = await GetFirmwareAsync(id, org, user);
            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _repo.DeleteFirmwareAsync(id);

            return InvokeResult.Success;
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _repo.QueryKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult<FirmwareRevision>> UploadRevision(string firmwareId, string versionCode, Stream stream, EntityHeader org, EntityHeader user)
        {
            var revision = new FirmwareRevision()
            {
                VersionCode = versionCode
            };

            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);

            await AuthorizeAsync(user.Id, org.Id, "UploadFirmwareBinary", $"Firmware Id: {firmwareId} RevisionId: {revision.Id}");

            await _repo.AddFirmwareRevisionAsync(firmwareId, revision.Id, bytes);

            return InvokeResult<FirmwareRevision>.Create(revision);
        }

        public async Task<InvokeResult<FirmwareDownloadRequest>> RequestDownloadLinkAsync(string deviceId, string firmwareId, string revisionId, EntityHeader org, EntityHeader user)
        {
            var request = new FirmwareDownloadRequest()
            {
                FirmwareId = firmwareId,
                OrgId = org.Id,
                FirmwareRevisionId = revisionId,
                ExpiresUTC = DateTime.UtcNow.AddMinutes(5).ToJSONString(),
                DeviceId = deviceId,
            };

            var firmware = await _repo.GetFirmwareAsync(firmwareId);
            if (firmware.OwnerOrganization.Id != org.Id)
            {
                return InvokeResult<FirmwareDownloadRequest>.FromError("Can not request firmware from a different organization.");
            }

            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Update, user, org, "updateDeviceFirmware");

            await _repo.AddDownloadRequestAsync(request);

            return InvokeResult<FirmwareDownloadRequest>.Create(request);
        }

        public async Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string firmwareId, string revisionId, EntityHeader org, EntityHeader user)
        {
            var firmware = await _repo.GetFirmwareAsync(firmwareId);
            await AuthorizeAsync(user.Id, org.Id, "DownloadFirmwareBinary", $"Firmware Id: {firmwareId} RevisionId: {revisionId}");

            return await _repo.GetFirmareBinaryAsync(firmwareId, revisionId);
        }

        public async Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string downloadId)
        {
            var request = await _repo.GetDownloadRequestAsync(downloadId);
            if (request == null)
            {
                throw new RecordNotFoundException(nameof(FirmwareDownloadRequest), downloadId);
            }

            if (request.Expired)
            {
                throw new NotAuthorizedException("Request has already been handled or has expired.");
            }

            if ((DateTime.UtcNow - request.ExpiresUTC.ToDateTime()).TotalMinutes > 2)
            {
                request.Expired = true;
                await _repo.UpdateDownloadRequestAsync(request);
                throw new NotAuthorizedException("Firmware request has expired.");
            }

            request.Expired = true;
            await _repo.UpdateDownloadRequestAsync(request);

            return await _repo.GetFirmareBinaryAsync(request.FirmwareId, request.FirmwareRevisionId);
        }
    }
}
