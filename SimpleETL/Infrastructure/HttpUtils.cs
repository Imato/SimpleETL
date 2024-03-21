using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Imato.SimpleETL
{
    public static class HttpUtils
    {
        public static HttpClientHandler GetDefaultHandler()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                ClientCertificateOptions = ClientCertificateOption.Automatic
            };
            return handler;
        }
    }
}