﻿using LagoVista.Core.Exceptions;
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
using System.Threading.Tasks;
using System.Linq;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class FirmwareDownloadManager : IFirmwareDownloadManager
    {
        private readonly IFirmwareRepo _repo;

        public const string MainFirmware = "main";
        public const string OtaFirmware = "ota";

        public FirmwareDownloadManager(IFirmwareRepo repo)
        {
            _repo = repo ?? throw new ArgumentNullException();
        }

        public async Task<InvokeResult<FirmwareDownload>> DownloadFirmwareAsync(string type, string downloadId, int? start = null, int? length = null)
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

            if ((DateTime.UtcNow - request.ExpiresUTC.ToDateTime()).TotalMinutes > 10)
            {
                request.Expired = true;
                request.Status = "Expired";
                request.Error = "Requested after expired.";
                await _repo.UpdateDownloadRequestAsync(request);
                throw new NotAuthorizedException("Firmware request has expired.");
            }

            await _repo.UpdateDownloadRequestAsync(request);

            var result = await _repo.GetFirmareBinaryAsync(type, request.FirmwareId, request.FirmwareRevisionId);
            if (result.Successful)
            {
                if (start.HasValue && length.HasValue)
                {
                    var buffer = result.Result;

                    var remainingLength = buffer.Length - start.Value;
                    var sendLength = Math.Min(length.Value, remainingLength);

                    var output = new byte[sendLength];
                    Array.Copy(buffer, start.Value, output, 0, sendLength);

                    var requestedSoFar = start.Value + length.Value;
                    request.PercentRequested = (requestedSoFar * 100) / buffer.Length;
                    request.Status = "Downloading";
                    await _repo.UpdateDownloadRequestAsync(request);

                    return InvokeResult<FirmwareDownload>.Create(new FirmwareDownload
                    {
                        Buffer = output,
                        PercentComplete = request.PercentRequested,
                        Size = buffer.Length,
                        Message = $"Percent Complete {request.PercentRequested}%"
                    });
                }
                else
                {
                    request.PercentRequested = 100;
                    request.Status = "Downloading";
                    return InvokeResult<FirmwareDownload>.FromInvokeResult(result.ToInvokeResult());
                }
            }
            else
            {
                return InvokeResult<FirmwareDownload>.FromInvokeResult(result.ToInvokeResult());
            }
        }

        public async Task<InvokeResult<int>> GetFirmwareLengthAsync(string downloadId)
        {
            var request = await _repo.GetDownloadRequestAsync(downloadId);
            if (request == null)
            {
                throw new RecordNotFoundException(nameof(FirmwareDownloadRequest), downloadId);
            }

            var result = await _repo.GetFirmareBinaryAsync(OtaFirmware, request.FirmwareId, request.FirmwareRevisionId);
            if (result.Successful)
            {
                return InvokeResult<int>.Create(result.Result.Length);
            }
            else
            {
                throw new RecordNotFoundException(nameof(FirmwareDownloadRequest), downloadId);
            }
        }

        public async Task<InvokeResult> MarkAsCompleteAsync(string downloadId)
        {
            var request = await _repo.GetDownloadRequestAsync(downloadId);
            if (request == null)
            {
                throw new RecordNotFoundException(nameof(FirmwareDownloadRequest), downloadId);
            }

            request.Expired = true;
            request.Status = "Completed";
            request.PercentRequested = 100;
            request.Error = "noerror";

            await _repo.UpdateDownloadRequestAsync(request);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> MarkAsFailedAsync(string downloadId, string err)
        {
            var request = await _repo.GetDownloadRequestAsync(downloadId);
            if (request == null)
            {
                throw new RecordNotFoundException(nameof(FirmwareDownloadRequest), downloadId);
            }

            request.Expired = true;
            request.Status = "Failed";
            request.Error = err;

            await _repo.UpdateDownloadRequestAsync(request);

            return InvokeResult.Success;
        }
    }

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

        public async Task<ListResponse<FirmwareDownloadRequest>> GetRequestsForDeviceAsync(string deviceRepoId, string deviceId, EntityHeader user, EntityHeader org, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, org.Id, typeof(FirmwareDownloadRequest), Actions.Read);

            return await _repo.GetDownloadRequestsForDeviceAsync(deviceRepoId, deviceId);
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

        public async Task<InvokeResult<EntityHeader>> UploadMainRevision(string firmwareId, string revisionid, Stream stream, EntityHeader org, EntityHeader user)
        {
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);

            await AuthorizeAsync(user.Id, org.Id, "UploadFirmwareBinary", $"Firmware Id: {firmwareId} RevisionId: {revisionid}");

            var fileName = await _repo.AddFirmwareRevisionAsync(FirmwareDownloadManager.MainFirmware,firmwareId, revisionid, bytes);

            return InvokeResult<EntityHeader>.Create(EntityHeader.Create(Guid.NewGuid().ToId(), fileName));
        }

        public async Task<InvokeResult<EntityHeader>> UploadOtaRevision(string firmwareId, string revisionid, Stream stream, EntityHeader org, EntityHeader user)
        {
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);

            await AuthorizeAsync(user.Id, org.Id, "UploadFirmwareBinary", $"Firmware Id: {firmwareId} RevisionId: {revisionid}");

            var fileName = await _repo.AddFirmwareRevisionAsync(FirmwareDownloadManager.OtaFirmware, firmwareId, revisionid, bytes);

            return InvokeResult<EntityHeader>.Create(EntityHeader.Create(Guid.NewGuid().ToId(), fileName));
        }


        public async Task<InvokeResult<FirmwareDownloadRequest>> RequestDownloadLinkAsync(string deviceRepoId, string deviceId, string firmwareId, string revisionId, EntityHeader org, EntityHeader user)
        {
            var firmware = await _repo.GetFirmwareAsync(firmwareId);
            if (firmware.OwnerOrganization.Id != org.Id)
            {
                return InvokeResult<FirmwareDownloadRequest>.FromError("Can not request firmware from a different organization.");
            }

            var revision = firmware.Revisions.SingleOrDefault(rev => rev.Id == revisionId);
            if (revision == null)
            {
                throw new RecordNotFoundException(nameof(FirmwareRevision), revisionId);
            }

            var request = new FirmwareDownloadRequest()
            {
                FirmwareId = firmwareId,
                FirmwareName = $"{firmware.FirmwareSku} {revision.VersionCode}",
                OrgId = org.Id,
                FirmwareRevisionId = revisionId,
                ExpiresUTC = DateTime.UtcNow.AddMinutes(30).ToJSONString(),
                DeviceId = deviceId,
                DeviceRepoId = deviceRepoId,
                Status = "New",
                PercentRequested = 0,
            };

            await AuthorizeAsync(firmware, AuthorizeResult.AuthorizeActions.Update, user, org, "updateDeviceFirmware");

            await _repo.AddDownloadRequestAsync(request);

            return InvokeResult<FirmwareDownloadRequest>.Create(request);
        }

        public async Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string type, string firmwareId, string revisionId, EntityHeader org, EntityHeader user)
        {
            var firmware = await _repo.GetFirmwareAsync(firmwareId);
            await AuthorizeAsync(user.Id, org.Id, "DownloadFirmwareBinary", $"Firmware Id: {firmwareId} RevisionId: {revisionId}");

            return await _repo.GetFirmareBinaryAsync(type,firmwareId, revisionId);
        }
    }
}
