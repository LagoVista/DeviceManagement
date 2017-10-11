using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IInputCommandService
    {
        Task<InvokeResult<List<InputCommandEndPoint>>> GetInputCommandEndPointsForDeviceConfig(string deviceConfigId, EntityHeader org, EntityHeader user);
    }
}
