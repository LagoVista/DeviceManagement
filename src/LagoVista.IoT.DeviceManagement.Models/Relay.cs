// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b1be8f294ae3743f85d261ff8a4d0a7294111e0fa3563a5ea96fcb7a77bcd11
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public enum RelayStates
    {
        [EnumLabel(Relay.RelayState_On, DeviceManagementResources.Names.RelayState_On, typeof(DeviceManagementResources))]
        On,
        [EnumLabel(Relay.RelayState_Off, DeviceManagementResources.Names.RelayState_Off, typeof(DeviceManagementResources))]
        Off,
        [EnumLabel(Relay.RelayState_Unknown, DeviceManagementResources.Names.RelayState_Unknown, typeof(DeviceManagementResources))]
        Unknown
    }

    public class Relay
    {
        public const string RelayState_Unknown = "unknown";
        public const string RelayState_On = "on";
        public const string RelayState_Off = "off";

        public int Index { get; set; }
        public string Name { get; set; }
    
        public EntityHeader<RelayStates> CurrentState { get; set; }
        public EntityHeader<RelayStates> DesiredState { get; set; }

        public string LastUpdated { get; set; }
    }
}
