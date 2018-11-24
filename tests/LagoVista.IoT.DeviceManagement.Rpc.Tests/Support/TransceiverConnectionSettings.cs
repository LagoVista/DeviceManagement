using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Settings;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public class TransceiverConnectionSettings : ITransceiverConnectionSettings
    {
        public TransceiverConnectionSettings()
        {
            RpcAdmin = new ConnectionSettings();
            RpcTransmitter = new ConnectionSettings();
            RpcReceiver = new ConnectionSettings();
        }

        public IConnectionSettings RpcAdmin { get; }
        public IConnectionSettings RpcTransmitter { get; }
        public IConnectionSettings RpcReceiver { get; }
    }
}
