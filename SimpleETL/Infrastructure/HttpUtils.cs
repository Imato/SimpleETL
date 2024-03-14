using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace SimpleETL
{
    public static class HttpUtils
    {
        public static HttpClientHandler GetDefaultHandler()
        {
            // 1
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                ClientCertificateOptions = ClientCertificateOption.Automatic
            };            

            // 2
            /*
            var cert = GetCertificate();
            if(cert != null)
                handler.ClientCertificates.Add(GetCertificate());
            */
            // 3
            /*
            var certificate = new X509Certificate2("dashprod.p12", "**********");
            handler.ClientCertificates.Add(certificate);
            */
            return handler;
        }

        private static X509Certificate2 GetCertificate()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = store.Certificates;
                X509Certificate2Collection findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "dashprod", true);
                X509Certificate2 clientCertificate = null;
                if (findResult.Count == 1)
                {
                    clientCertificate = findResult[0];
                }
                else
                {
                    throw new ApplicationException("Unable to locate the correct client certificate!");
                }
                return clientCertificate;
            }
            catch
            {
                throw;
            }
            finally
            {
                store.Close();
            }
        }
    }
}
