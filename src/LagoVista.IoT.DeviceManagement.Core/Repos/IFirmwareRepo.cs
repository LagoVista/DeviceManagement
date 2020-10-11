using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement
{
    public interface IFirmwareRepo
    {
        Task AddFirmwareAsync(Firmware firmware);
        Task<Firmware> GetFirmwareAsync(string firmwareId);
        Task UpdateFirmwareAsync(Firmware firmware);
        Task DeleteFirmwareAsync(string id);
        Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, ListRequest listRequest);
        Task<bool> QueryKeyInUseAsync(string key, string org);

        Task<FirmwareDownloadRequest> GetDownloadRequestAsync(string id);

        Task<ListResponse<FirmwareDownloadRequest>> GetDownloadRequestsForDeviceAsync(string deviceRepoId, string deviceId);


        Task AddDownloadRequestAsync(FirmwareDownloadRequest request);
        Task UpdateDownloadRequestAsync(FirmwareDownloadRequest request);

        Task AddFirmwareRevisionAsync(string firmwareId, string revisionId, byte[] buffer);

        Task<InvokeResult<byte[]>> GetFirmareBinaryAsync(string firmwareId, string revisionId);
    }
}
