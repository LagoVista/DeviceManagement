using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IRemotePropertyNamanager
    {
        Task<InvokeResult> SendPropertyAsync(string deviceUniqueId, int propertyIndex);
        Task<InvokeResult> SendCommandAsync(string deviceUniqueId, string command, List<KeyValuePair<string, string>> parameters);
        Task<InvokeResult> SendAllPropertiesAsync(string deviceUniqueId);
        Task<InvokeResult> QueryRemoteConfigurationAsync(string deviceUniqueId);
        Task<InvokeResult> RestartDeviceAsync(string deviceUniqueId);
        Task<InvokeResult> SetFirmwareVersionAsync(string deviceUniqueId, string requestId);
    }
}
