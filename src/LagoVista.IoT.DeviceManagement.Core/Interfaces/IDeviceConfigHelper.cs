using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IDeviceConfigHelper
    {
        /// <summary>
        /// There are a number of meta data properties that are useful for device admin, but should not be stored with the device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task<InvokeResult> PopulateDeviceConfigToDeviceAsync(Device device, EntityHeader instance, EntityHeader org, EntityHeader usr);

        Task<EntityHeader<StateSet>> GetCustomDeviceStatesAsync(string deviceConfigId, EntityHeader org, EntityHeader user);
    }
}
