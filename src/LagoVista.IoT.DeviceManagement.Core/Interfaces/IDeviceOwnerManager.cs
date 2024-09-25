using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    // TODO: Not implemented, need to really think through how to associate users with devices.
    public interface IDeviceOwnerManager
    {
        Task<InvokeResult> AddDeviceOwnerAsync(DeviceOwner owner);
        Task<InvokeResult> UpdateDeviceOwnerAsync(DeviceOwner owner);
        Task<InvokeResult> GetDeviceOwnerAsync(string emailAddress);
    }
}
