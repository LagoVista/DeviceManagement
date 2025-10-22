// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 92cd0c6dfaf535c7a70c1d1f41e8313f9f0db68f75cb22c1b2ea02b2e4071ea0
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.Utils
{
    public class LogWriter : ILogWriter
    {
        public Task WriteError(LogRecord record)
        {
            Console.WriteLine(record.Message);
            return Task.FromResult(default(object));
        }

        public Task WriteEvent(LogRecord record)
        {
            Console.WriteLine(record.Message);
            return Task.FromResult(default(object));
        }
    }
}
