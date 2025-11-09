// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 10367aad7a9ab2a8e324026c0a412ff12c7701ae7e69169637b2c3139d7306e5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.MediaIntegrationTests.Utils
{
    public class LogWriter : ILogWriter
    {
        public Task WriteError(LogRecord record)
        {
            Console.WriteLine(record.Message);
            Console.WriteLine($"\t\t{record.Details}");


            return Task.FromResult(default(object));
        }

        public Task WriteEvent(LogRecord record)
        {
            Console.WriteLine(record.Message);
            return Task.FromResult(default(object));
        }
    }
}
