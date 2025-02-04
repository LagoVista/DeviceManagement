using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IRemoteConfigurationManager
    {
        Task<InvokeResult> SetDesiredConfigurationRevisionAsync(string deviceRepoId, string deviceUniqueId, int configurationRevision, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendPropertyAsync(string deviceRepoId, string deviceUniqueId, int propertyIndex, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendAllPropertiesAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> QueryRemoteConfigurationAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartDeviceAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<string>> ApplyFirmwareAsync(string deviceRepoId, string deviceUniqueId, string firmwareId, string firmwareRevisionId, bool triggerOnDevice, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendCommandAsync(string deviceRepoId, string deviceUniqueId, string commandId, List<KeyValuePair<string, string>> parameters, EntityHeader org, EntityHeader user);
       
    }
}
