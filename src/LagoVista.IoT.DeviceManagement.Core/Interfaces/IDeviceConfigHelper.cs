// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1c42a7e5d44014fa313df001c8a26dd8dfbcd2ff8ff62cd2824e64351b09dd06
// IndexVersion: 0
// --- END CODE INDEX META ---
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

        Task<HomePages> GetHomePagesAsync(string deviceConfigId, EntityHeader org, EntityHeader user);
    }

    public class HomePages
    {
        public string CustomPage { get; set; }
        public string CustomMobilePage { get; set; }
        public string CustomPageQuickLink { get; set; }

    }
}
