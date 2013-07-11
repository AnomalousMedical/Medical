using Anomalous.Security;
using Mono.Security.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    public class CertificateStoreManager
    {
        public static void Initialize(String certificateStoreFile)
        {
            if (File.Exists(certificateStoreFile))
            {
                using (FileStream fs = new FileStream(certificateStoreFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    CertificateStore = CertificateStore.fromSignedBytes(bytes, LoadEmbeddedCertificate("Medical.Resources.AnomalousMedicalRoot.cer"), LoadEmbeddedCertificate("Medical.Resources.AnomalousMedicalCertificateStore.cer"));
                }
            }
        }

        public static CertificateStore CertificateStore { get; set; }

        private static X509Certificate LoadEmbeddedCertificate(String name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                byte[] rawCert = new byte[stream.Length];
                stream.Read(rawCert, 0, rawCert.Length);
                return new X509Certificate(rawCert);
            }
        }
    }
}
