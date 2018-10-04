using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Settings;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public class TransceiverConnectionSettings : ITransceiverConnectionSettings
    {
        public TransceiverConnectionSettings()
        {
            RpcTopicConstructor = new ConnectionSettings();
            RpcTransmitter = new ConnectionSettings();
            RpcReceiver = new ConnectionSettings();
        }

        public IConnectionSettings RpcTopicConstructor { get; }
        public IConnectionSettings RpcTransmitter { get; }
        public IConnectionSettings RpcReceiver { get; }
    }
}
