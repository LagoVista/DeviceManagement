using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{

    /// <summary>
    /// This is a public controller, it will pass in a GUID that will be used to do
    /// a lookup on a download of a firmware revision.
    /// </summary>
    public class FirmwarDownloaddController : Controller
    {
        readonly IFirmwareDownloadManager _firmwareDownloadManager;

        public FirmwarDownloaddController(IFirmwareDownloadManager firmwareDownlaodManager)
        {
            _firmwareDownloadManager = firmwareDownlaodManager ?? throw new ArgumentNullException();
        }

        [HttpGet("/api/firmware/download/{requestid}")]
        public async Task<IActionResult> DownloadFirmwareAsync(string requestid, int? start, int? length)
        {
            var firmware = await _firmwareDownloadManager.DownloadFirmwareAsync(requestid, start, length);

            var ms = new MemoryStream(firmware.Result);
            return new FileStreamResult(ms, "application/octet-stream");
        }

        [HttpGet("/api/firmware/download/{requestid}/failed")]
        public async Task<InvokeResult> MarkAsFailedAsync(string requestid, string err)
        {
            return await _firmwareDownloadManager.MarkAsFailedAsync(requestid, err);
        }

        [HttpGet("/api/firmware/download/{requestid}/success")]
        public async Task<InvokeResult> MarkAsSuccessAsync(string requestid)
        {
            return await _firmwareDownloadManager.MarkAsCompleteAsync(requestid);
        }

        [HttpGet("/api/firmware/download/{requestid}/size")]
        public async Task<int> GetFirmwareSize(string requestid)
        {
            var firmware = await _firmwareDownloadManager.GetFirmwareLengthAsync(requestid);
            return firmware.Result;
        }
    }

    [Authorize]
    public class FirmwareController : LagoVistaBaseController
    {
        readonly IFirmwareManager _firmwareManager;

        public FirmwareController(IFirmwareManager firmwareManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _firmwareManager = firmwareManager ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Firmware - Add Firmare
        /// </summary>
        /// <param name="firmware"></param>
        [HttpPost("/api/firmware")]
        public Task<InvokeResult> AddFirmawre([FromBody] Firmware firmware)
        {
            return _firmwareManager.AddFirmwareAsync(firmware, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Firmware - Add Firmare
        /// </summary>
        /// <param name="firmware"></param>
        [HttpPut("/api/firmware")]
        public Task<InvokeResult> UpdateFirmwareAsync([FromBody] Firmware firmware)
        {
            SetUpdatedProperties(firmware);
            return _firmwareManager.UpdateFirmwareAsync(firmware, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/firmwares")]
        public Task<ListResponse<FirmwareSummary>> GetFirmwaresForOrgAsync()
        {
            return _firmwareManager.GetFirmwareForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get the firmware dowwnload history for a device.
        /// </summary>
        /// <param name="repoid"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [HttpGet("/api/firmware/history/{repoid}/{deviceid}")]
        public Task<ListResponse<FirmwareDownloadRequest>> GetFirmwaresForOrgAsync(string repoid, string deviceid)
        {
            return _firmwareManager.GetRequestsForDeviceAsync(repoid, deviceid, UserEntityHeader, OrgEntityHeader, GetListRequestFromHeader());
        }


        /// <summary>
        /// Firmware - Get firmawre.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("/api/firmware/{id}")]
        public async Task<DetailResponse<Firmware>> GetFirmwareAsync(string id)
        {
            return DetailResponse<Firmware>.Create(await _firmwareManager.GetFirmwareAsync(id, OrgEntityHeader, UserEntityHeader));
        }

        /// <summary>
        /// Firmware - Delete firmawre.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("/api/firmware/{id}")]
        public Task<InvokeResult> DeleteFirmwareAsync(string id)
        {
            return _firmwareManager.DeleteFirmwareAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Download a firmware revision.
        /// </summary>
        /// <param name="firmwareid"></param>
        /// <param name="revisionid"></param>
        /// <returns></returns>
        [HttpGet("/api/firmware/download/{firmwareid}/{revisionid}")]
        public async Task<IActionResult> DownloadFirmwareAsync(string firmwareid, string revisionid)
        {
            var firmware = await _firmwareManager.DownloadFirmwareAsync(firmwareid, revisionid, OrgEntityHeader, UserEntityHeader);

            var ms = new MemoryStream(firmware.Result);
            return new FileStreamResult(ms, "application/octet-stream");
        }

        /// <summary>
        /// Firmware upload a new revision.
        /// </summary>
        /// <param name="firmwareid"></param>
        /// <param name="versioncode"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("/api/firmware/{firmwareid}/{versioncode}")]
        public async Task<InvokeResult<FirmwareRevision>> AddFirmwareRevisionAsync(string firmwareid, string versioncode, [FromForm] IFormFile file)
        {
            using (var strm = file.OpenReadStream())
            {
                return await _firmwareManager.UploadRevision(firmwareid, versioncode, strm, OrgEntityHeader, UserEntityHeader);
            }
        }

        /// <summary>
        /// Create new instance of a firmware object that can be used to populate the UI.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/firmware/factory")]
        public  DetailResponse<Firmware> Factory()
        {
            var response = DetailResponse<Firmware>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Create new instance of a firmware object that can be used to populate the UI.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/firmware/revision/factory")]
        public DetailResponse<FirmwareRevision> RevisionFactory()
        {
            var response = DetailResponse<FirmwareRevision>.Create();
            response.Model.Id = Guid.NewGuid().ToString();

            return response;
        }
    }
}
