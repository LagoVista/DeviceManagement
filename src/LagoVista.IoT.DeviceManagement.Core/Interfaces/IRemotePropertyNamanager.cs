// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d1f0761531e1d3d9dd22833092c0e1ccca01228b0bbfdf5774207290dd645e90
// IndexVersion: 2
// --- END CODE INDEX META ---
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
        Task<InvokeResult> SetDesiredConfigurationRevisionAsync(string deviceUniqueId, int revisionLevel);
    }
}
