using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IFirmwareRepoSettings
    {
        IConnectionSettings FirmwareDocDBSettings { get; }
        IConnectionSettings FirmwareRequestSettings { get; }
        IConnectionSettings FirmwareBinSettings { get; }
    }
}
