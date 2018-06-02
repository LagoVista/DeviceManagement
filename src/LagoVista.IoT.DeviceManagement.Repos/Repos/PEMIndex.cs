using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class PEMIndex : TableEntity, IPEMIndex
    {
        public string MessageId { get; set; }
        public string DeviceId { get; set; }
        public string Status { get; set; }
        public string MessageType { get; set; }
        public string ErrorReason { get; set; }
        public string CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string JSON { get; set; }
        public string TextPayload { get; set; }
        public string Values { get; set; }
        public string OutgoingMessages { get; set; }
        public string ResponseMessage { get; set; }
        public string Log { get; set; }
        public string Instructions { get; set; }
        public string Device { get; set; }

        public InvokeResult<string> ToPEM()
        {
            if (!String.IsNullOrEmpty(JSON))
            {
                var pem = JsonConvert.DeserializeObject(JSON) as JObject;
                if (pem == null)
                {
                    return InvokeResult<string>.FromError("Could not deserialize PEM JSON");
                }

                pem.Add("textPayload", TextPayload);

                var envelope = pem["envelope"] as JObject;
                if (envelope == null)
                {
                    return InvokeResult<string>.FromError("Could not deserialize PEM Envelope");
                }

                if (!String.IsNullOrEmpty(Values))
                {
                    var values = JsonConvert.DeserializeObject(Values) as JObject;
                    if (values != null)
                    {
                        envelope.Add("values", values);
                    }
                }

                if (!String.IsNullOrEmpty(Log))
                {
                    var log = JsonConvert.DeserializeObject(Log) as JArray;
                    if (log != null)
                    {
                        if (pem.ContainsKey("log")) pem.Remove("log");
                        pem.Add("log", Log);
                    }
                }

                if (!string.IsNullOrEmpty(Device))
                {
                    var device = JsonConvert.DeserializeObject(Device) as JObject;
                    if (device != null)
                    {
                        if (pem.ContainsKey("device")) pem.Remove("device");
                        pem.Add("device", device);
                    }
                }

                if (!string.IsNullOrEmpty(Instructions))
                {
                    var instructions = JsonConvert.DeserializeObject(Instructions) as JArray;
                    if (instructions != null)
                    {
                        if (pem.ContainsKey("instructions")) pem.Remove("instructions");
                        pem.Add("instructions", instructions);
                    }
                }

                if (!string.IsNullOrEmpty(ResponseMessage))
                {
                    var responseMessage = JsonConvert.DeserializeObject(ResponseMessage) as JObject;
                    if (responseMessage != null)
                    {
                        if (pem.ContainsKey("responseMessage")) pem.Remove("responseMessage");
                        pem.Add("responseMessage", responseMessage);
                    }
                }

                if (!string.IsNullOrEmpty(OutgoingMessages))
                {
                    var outgoingMessages = JsonConvert.DeserializeObject(OutgoingMessages) as JArray;
                    if (outgoingMessages != null)
                    {
                        if (pem.ContainsKey("outgoingMessages")) pem.Remove("outgoingMessages");
                        pem.Add("outgoingMessages", outgoingMessages);
                    }
                }
           
                return InvokeResult<string>.Create(pem.ToString());
            }

            return InvokeResult<string>.FromError("JSON In Table Storage Entity was null or empty");


        }
    }
}
