using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class is EXTREMELY IMPORTANT. You must call the Initialize function before making any web requests or
    /// they could fail or be unsafe. This provides the logic to check with the os if a given ssl cert is ok when you
    /// are not running on windows.
    /// </summary>
    public static class OsSslValidator
    {
        /// <summary>
        /// Call this at program startup to secure any ssl connections made with the os.
        /// </summary>
        public static void Initialize()
        {
            //Setup the server certificate validation.
            ServicePointManager.ServerCertificateValidationCallback = checkValidationResult;
        }

        /// <summary>
        /// Verify the validation result either using the default based on sslPolicyErrors or a custom function for a given os. This depends on
        /// the value of PlatformConfig.HasCustomSSLValidation.
        /// </summary>
        /// <returns>True if the cert is valid. False otherwise.</returns>
        private static bool checkValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (PlatformConfig.HasCustomSSLValidation)
            {
                String host = null;
                if (sender is HttpWebRequest)
                {
                    host = ((HttpWebRequest)sender).Host;
                }
                else if (sender is String)
                {
                    host = sender.ToString();
                }

                if (host != null)
                {
                    bool trusted = PlatformConfig.TrustSSLCertificate(certificate, host);
                    if (!trusted)
                    {
                        Logging.Log.Error("Could not trust ssl certificate with subject '{0}' for host '{1}'. Connections to this server will not be possible", certificate.Subject, host);
                    }
                    return trusted;
                }
                else
                {
                    Logging.Log.Error("Host not specified when validating ssl certificate with subject '{0}'. Connections to this server will not be possible", certificate.Subject, host);
                    return false; //If we cannot check with the hosts, we just want to fail.
                }
            }
            else
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            }
        }
    }
}
