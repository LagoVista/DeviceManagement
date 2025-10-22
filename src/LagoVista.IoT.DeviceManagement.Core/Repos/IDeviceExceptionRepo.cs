// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e95e4463124425f0474348200647561b1b4368dcf07ebb90b47b4c519b2f808e
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceExceptionRepo
    {
        Task AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException exception);
        Task AddDeviceExceptionClearedAsync(DeviceRepository deviceRepo, DeviceException exception);
        Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
        Task ClearDeviceExceptionsAsync(DeviceRepository deviceRepo, string id);
    }
}
