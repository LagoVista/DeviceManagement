// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1454769fc38f659bf4426187454a953889b3b321613c42083ec4c83eb618df0d
// IndexVersion: 2
// --- END CODE INDEX META ---
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
