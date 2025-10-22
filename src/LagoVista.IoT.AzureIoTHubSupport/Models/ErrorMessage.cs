// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 42e6db4fcea005fd8fdbf185542618b4b314b8fe3eb5496bacdeb4a07662041e
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.AzureIoTHubSupport.Models
{
    public class ErrorMessage
    {
        //{"Message":"ErrorCode:DeviceAlreadyExists;A device with ID 'device004' is already registered.","ExceptionMessage":"Tracking ID:987573ab94334e77a1cd0beb671a71c4-G:5-TimeStamp:09/28/2017 17:31:56"}

        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
}
}
