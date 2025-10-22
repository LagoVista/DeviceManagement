// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bd7cdc751ad850af3c14fc0fc75500e6eb6957d30900ca923abcf2cb918b77a5
// IndexVersion: 0
// --- END CODE INDEX META ---
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
