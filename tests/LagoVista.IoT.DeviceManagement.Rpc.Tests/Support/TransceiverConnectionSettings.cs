using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Settings;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    public class TransceiverConnectionSettings : ITransceiverConnectionSettings
    {
        public TransceiverConnectionSettings()
        {
            RpcClientTransmitter = new ConnectionSettings();
            RpcClientReceiver = new ConnectionSettings();
            RpcServerTransmitter = new ConnectionSettings();
            RpcServerReceiver = new ConnectionSettings();
        }

        public IConnectionSettings RpcAdmin { get; }        

        public IConnectionSettings RpcClientTransmitter { get; }

        public IConnectionSettings RpcClientReceiver { get; }

        public IConnectionSettings RpcServerTransmitter { get; }

        public IConnectionSettings RpcServerReceiver { get; }
    }
}
