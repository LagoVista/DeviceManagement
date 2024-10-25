using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceStatusManager
    {
        Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus deviceStatus, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SetSilenceAlarmAsync(DeviceRepository deviceRepo, string id, bool silenced, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceStatus>> GetWatchdogDeviceStatusAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
