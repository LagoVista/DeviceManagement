using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    /*
    https://docs.microsoft.com/en-us/rest/api/iot-dps/deviceenrollment/createorupdate#definitions_attestationmechanism
    */
    public class AzureIoTDevice
    {

    }

    public class SymmetericKey
    {
        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty("secondaryKey")]
        public string SecondaryKey { get; set; }
    }

    public class AttestationMechanism
    {
        public TpmAttestation tpm { get; set; }

        public string type { get; set; }
        public X509Attestation x509 { get; set; }
    }

    public class DeviceRegistrationStatus
    {
        public string assignedHub { get; set; }
        public string createdDateTimeUtc { get; set; }
        public string deviceId { get; set; }
        public Int32 errorCode { get; set; }
        public string errorMessage { get; set; }
        public string etag { get; set; }
        public string generationId { get; set; }
        public string lastUpdatedDateTimeUtc { get; set; }
        public string registrationId { get; set; }
        public string status { get; set; }
    }

    public class Enrollment
    {
        public AttestationMechanism attestation { get; set; }

        public string createdDateTimeUtc { get; set; }
        public string deviceId { get; set; }
        public string etag { get; set; }
        public string generationId { get; set; }
        public TwinState initialTwinState { get; set; }
        public string iotHubHostName { get; set; }
        public string lastUpdatedDateTimeUtc { get; set; }
        public string provisioningStatus { get; set; }
        public string registrationId { get; set; }

        public DeviceRegistrationStatus registrationStatus { get; set; }
    }

    public class TpmAttestation
    {
        public string endorsementKey { get; set; }
        public string storageRootKey { get; set; }
    }

    public class TwinState
    {
        [JsonProperty("desiredProperties")]
        public Object[] DesiredProperties { get; set; }


        [JsonProperty("tags")]
        public Object[] Tags { get; set; }
    }

    public class X509Attestation
    {
        public X509Certificates clientCertificates { get; set; }
        public X509Certificates signingCertificates { get; set; }
    }

    public class X509Certificates
    {
        public X509CertificateWithInfo primary { get; set; }
        public X509CertificateWithInfo secondary { get; set; }

    }

    public class X509CertificateWithInfo
    {
        public string kcertificate { get; set; }

        public X509CertificateInfo info { get; set; }
    }

    public class X509CertificateInfo
    {
        public string issuerName { get; set; }

        public string notAfterUtc { get; set; }
        public string notBeforeUtc { get; set; }
        public string serialNumber { get; set; }
        public string sha1Thumbprint { get; set; }
        public string sha256Thumbprint { get; set; }
        public string subjectName { get; set; }
        public Int32 version { get; set; }
    }
    
}
