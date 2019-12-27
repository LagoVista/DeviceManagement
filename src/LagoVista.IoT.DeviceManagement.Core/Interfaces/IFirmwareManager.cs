using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IFirmwareManager
    {
        Task<InvokeResult> AddFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user);
        Task<Firmware> GetFirmwareAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteFirmwareAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryKeyInUse(string key, EntityHeader org);
        Task<InvokeResult<FirmwareRevision>> UploadRevision(string firmwareId, string versionCode, Stream stream, EntityHeader org, EntityHeader user);
        Task<InvokeResult<FirmwareDownloadRequest>> RequestDownloadLinkAsync(string deviceId, string firmwareId, string revisionId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string firmwareId, string revisionId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string downloadId);
    }
}
