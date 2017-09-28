using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.AzureIoTHubSupport.Exceptions
{
    public class AzureIoTHubException : Exception
    {
        public AzureIoTHubException(Models.ErrorMessage err) : base(err.Message)
        {
            Error = err;
        }

        public Models.ErrorMessage Error { get; private set; }
    }
}
