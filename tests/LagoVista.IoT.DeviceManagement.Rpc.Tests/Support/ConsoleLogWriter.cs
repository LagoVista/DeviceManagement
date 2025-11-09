// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4e5e3314e85ab838a39cf37b5cd2c2a8284579fc1a922006b41c1a127f9e2054
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests.Support
{
    //todo: persist the log entries
    public class ConsoleLogWriter : ILogWriter
    {
        public Task WriteError(LogRecord record)
        {
            Console.WriteLine(record.Message);
            return Task.FromResult<object>(null);
        }

        public Task WriteEvent(LogRecord record)
        {
            Console.WriteLine(record.Message);
            return Task.FromResult<object>(null);
        }
    }
}
