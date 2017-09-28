using System;
using System.Collections.Generic;
using System.Globalization;

using System.Security.Cryptography;

using System.Text;
using System.Net;

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace LagoVista.IoT.DeviceManagement.Repos.Utils
{
    public sealed class SharedAccessSignatureBuilder
    {
        string key;

        public SharedAccessSignatureBuilder()
        {
            this.TimeToLive = TimeSpan.FromMinutes(20);
        }

        public string KeyName { get; set; }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                StringValidationHelper.EnsureBase64String(value, "Key");
                this.key = value;
            }
        }

        public string Target { get; set; }

        public TimeSpan TimeToLive { get; set; }

        public string ToSignature()
        {
            return BuildSignature(this.KeyName, this.Key, this.Target, this.TimeToLive);
        }

        static string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive)
        {
            string expiresOn = BuildExpiresOn(timeToLive);
            string audience = WebUtility.UrlEncode(target);
            List<string> fields = new List<string>();
            fields.Add(audience);
            fields.Add(expiresOn);

            // Example string to be signed:
            // dh://myiothub.azure-devices.net/a/b/c?myvalue1=a
            // <Value for ExpiresOn>

            string signature = Sign(string.Join("\n", fields), key);

            // Example returned string:
            // SharedAccessSignature sr=ENCODED(dh://myiothub.azure-devices.net/a/b/c?myvalue1=a)&sig=<Signature>&se=<ExpiresOnValue>[&skn=<KeyName>]

            var buffer = new StringBuilder();
            buffer.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}",
                SharedAccessSignatureConstants.SharedAccessSignature,
                SharedAccessSignatureConstants.AudienceFieldName, audience,
                SharedAccessSignatureConstants.SignatureFieldName, WebUtility.UrlEncode(signature),
                SharedAccessSignatureConstants.ExpiryFieldName, WebUtility.UrlEncode(expiresOn));

            if (!string.IsNullOrEmpty(keyName))
            {
                buffer.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}",
                    SharedAccessSignatureConstants.KeyNameFieldName, WebUtility.UrlEncode(keyName));
            }

            return buffer.ToString();
        }

        static string BuildExpiresOn(TimeSpan timeToLive)
        {
            DateTime expiresOn = DateTime.UtcNow.Add(timeToLive);
            TimeSpan secondsFromBaseTime = expiresOn.Subtract(SharedAccessSignatureConstants.EpochTime);
            long seconds = Convert.ToInt64(secondsFromBaseTime.TotalSeconds, CultureInfo.InvariantCulture);
            return Convert.ToString(seconds, CultureInfo.InvariantCulture);
        }

        static string Sign(string requestString, string key)
        {
#if WINDOWS_UWP
            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            var hash = algorithm.CreateHash(Convert.FromBase64String(key));
            hash.Append(Encoding.UTF8.GetBytes(requestString));
            var mac = hash.GetValueAndReset();
            return Convert.ToBase64String(mac);
#else
            using (var hmac = new HMACSHA256(Convert.FromBase64String(key)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
            }
#endif
        }
    }

    static class SharedAccessSignatureConstants
    {
        public const int MaxKeyNameLength = 256;
        public const int MaxKeyLength = 256;
        public const string SharedAccessSignature = "SharedAccessSignature";
        public const string AudienceFieldName = "sr";
        public const string SignatureFieldName = "sig";
        public const string KeyNameFieldName = "skn";
        public const string ExpiryFieldName = "se";
        public const string SignedResourceFullFieldName = SharedAccessSignature + " " + AudienceFieldName;
        public const string KeyValueSeparator = "=";
        public const string PairSeparator = "&";
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static readonly TimeSpan MaxClockSkew = TimeSpan.FromMinutes(5);
    }

    static class StringValidationHelper
    {
        const char Base64Padding = '=';
        static readonly HashSet<char> base64Table = new HashSet<char>{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
                                                                      'P','Q','R','S','T','U','V','W','X','Y','Z','a','b','c','d',
                                                                      'e','f','g','h','i','j','k','l','m','n','o','p','q','r','s',
                                                                      't','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                                                                      '8','9','+','/' };

        public static void EnsureBase64String(string value, string paramName)
        {
            if (!IsBase64StringValid(value))
            {
                throw new ArgumentException("String is not Base 64", paramName);
            }
        }

        public static bool IsBase64StringValid(string value)
        {
            if (value == null)
            {
                return false;
            }

            return IsBase64String(value);
        }

        public static void EnsureNullOrBase64String(string value, string paramName)
        {
            if (!IsNullOrBase64String(value))
            {
                throw new ArgumentException("String is not base 64", paramName);
            }
        }

        public static bool IsNullOrBase64String(string value)
        {
            if (value == null)
            {
                return true;
            }

            return IsBase64String(value);
        }

        public static bool IsBase64String(string value)
        {
            value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (value.Length == 0 || (value.Length % 4) != 0)
            {
                return false;
            }

            var lengthNoPadding = value.Length;
            value = value.TrimEnd(Base64Padding);
            var lengthPadding = value.Length;

            if ((lengthNoPadding - lengthPadding) > 2)
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!base64Table.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}