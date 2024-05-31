using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IRemotePropertyNamanager
    {
        Task<InvokeResult> SendPropertyAsync(string deviceUniqueId, int propertyIndex);
        Task<InvokeResult> SendAllPropertiesAsync(string deviceUniqueId);
        Task<InvokeResult> QueryRemoteConfigurationAsync(string deviceUniqueId);
        Task<InvokeResult> RestartDeviceAsync(string deviceUniqueId);
        Task<InvokeResult> SetFirmwareVersionAsync(string deviceUniqueId, string requestId);
    }
}
