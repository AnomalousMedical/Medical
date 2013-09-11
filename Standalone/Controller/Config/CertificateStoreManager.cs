using Anomalous.Security;
using Logging;
using Medical.Controller;
using Mono.Security.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Medical
{
    public static class CertificateStoreManager
    {
        public static event Action CertificateStoreLoaded;
        public static event Action CertificateStoreLoadError;

        private static String hashAlgoName;
        private static String hashAlgoOid;

        internal static void Initialize(String certificateStoreFile, String certificateStoreUrl)
        {
            Thread t = new Thread((arg) =>
            {
                X509Certificate trustRoot = LoadEmbeddedCertificate("Medical.Resources.AnomalousMedicalRoot.cer");
                X509Certificate trustedSignature = LoadEmbeddedCertificate("Medical.Resources.AnomalousMedicalCertificateStore.cer");

                #if ALLOW_OVERRIDE
                if (MedicalConfig.OverrideCertificateStore)
                {
                    trustRoot = LoadOverrideCertificate(MedicalConfig.CertificateStoreTrustedRoot);
                    trustedSignature = LoadOverrideCertificate(MedicalConfig.CertificateStoreTrustedSignature);
                }
                #endif

                ServerConnection serverConnection = new ServerConnection(certificateStoreUrl);
                if (File.Exists(certificateStoreFile))
                {
                    try
                    {
                        using (FileStream fs = new FileStream(certificateStoreFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            byte[] bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, bytes.Length);
                            CertificateStore = CertificateStore.fromSignedBytes(bytes, trustRoot, trustedSignature);
                        }
                        serverConnection.addArgument("clientIssueDate", CertificateStore.IssueDate.Ticks.ToString());
                    }
                    catch (SigningException se)
                    {
                        Log.Error("Signing Exception occured loading '{0}'. Message: {1}. Will attempt to reload from server.", certificateStoreFile, se.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("A {0} occured when trying to read the certificate store from the server. Message: {1}. Will attempt to reload from server.", certificateStoreFile, ex.Message);
                    }
                }

                try
                {
                    serverConnection.makeRequestDownloadResponse(responseStream =>
                        {
                            try
                            {
                                byte[] bytes = new byte[responseStream.Length];
                                responseStream.Read(bytes, 0, bytes.Length);
                                CertificateStore = CertificateStore.fromSignedBytes(bytes, trustRoot, trustedSignature);
                                using (FileStream fs = File.Open(certificateStoreFile, FileMode.Create, FileAccess.Write))
                                {
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                            }
                            catch (SigningException se)
                            {
                                Log.Error("Signing Exception occured loading response from '{0}'. Message: {1}. Will attempt to use existing store.", certificateStoreUrl, se.Message);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("A {0} occured when trying to read the certificate store from the server. Message: {1}.", ex.GetType().Name, ex.Message);
                            }
                        });
                }
                catch (Exception ex)
                {
                    Log.Error("A {0} occured when trying to read the certificate store from the server. Message: {1}.", ex.GetType().Name, ex.Message);
                }

                if (CertificateStore != null)
                {
                    hashAlgoName = CertificateStore.ServerCommunicationHashAlgo;
                    hashAlgoOid = CryptoConfig.MapNameToOID(hashAlgoName);
                    ThreadManager.invoke(() =>
                    {
                        Log.ImportantInfo("Certificate Store Issue Date is {0}", CertificateStore.IssueDate.ToString("MM/dd/yyyy"));
                        if (CertificateStoreLoaded != null)
                        {
                            CertificateStoreLoaded.Invoke();
                        }
                    });
                }
                else
                {
                    ThreadManager.invoke(() =>
                    {
                        Log.Error("Cannot find certificate store file '{0}'. Unable to run program without a certificate store.", certificateStoreFile);
                        if (CertificateStoreLoadError != null)
                        {
                            CertificateStoreLoadError.Invoke();
                        }
                    });
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        public static CertificateStore CertificateStore { get; private set; }

        public static bool IsValidServerCommunication(byte[] data, byte[] signature)
        {
            RSACryptoServiceProvider rsaService = CertificateStore.ServerCommunicationCertificate.RSA as RSACryptoServiceProvider;
            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashAlgoName))
            {
                byte[] hash = hashAlgorithm.ComputeHash(data);
                return rsaService.VerifyHash(hash, hashAlgoOid, signature);
            }
        }

        private static X509Certificate LoadEmbeddedCertificate(String name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                return ReadCertificateFromStream(stream);
            }
        }

        private static X509Certificate LoadOverrideCertificate(String fileName)
        {
            using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ReadCertificateFromStream(stream);
            }
        }

        private static X509Certificate ReadCertificateFromStream(Stream stream)
        {
            byte[] rawCert = new byte[stream.Length];
            stream.Read(rawCert, 0, rawCert.Length);
            return new X509Certificate(rawCert);
        }
    }
}
