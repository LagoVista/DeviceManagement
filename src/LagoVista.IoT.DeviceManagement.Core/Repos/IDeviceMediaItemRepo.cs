using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceMediaItemRepo
    {
        Task<byte[]> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);

        Task<InvokeResult> DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);
    }
}
