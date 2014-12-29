﻿using Anomalous.Security;
using Engine.Threads;
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
        /// <summary>
        /// This event is fired when the certificate store is found and loaded.
        /// This will be invoked on the main thread.
        /// </summary>
        public static event Action CertificateStoreLoaded;

        /// <summary>
        /// This event is fired if there is an error loading the certificate store.
        /// This will be invoked on the main thread.
        /// </summary>
        public static event Action CertificateStoreLoadError;

        /// <summary>
        /// This event is fired when a round trip has been made to the server checking for an updated Certificate Store.
        /// This will be invoked on the main thread.
        /// </summary>
        public static event Action ServerCheckedForCertificate;

        private static String hashAlgoName;
        private static String hashAlgoOid;

        private static String certificateStoreFile;
        private static String certificateStoreUrl;
        private static DateTime lastServerCheck;
        private static bool allowInitialize = true;

        static CertificateStoreManager()
        {
            CheckDelay = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Setup the CertificateStoreManager, this will get the settings and will start a thread that will check the server
        /// for any CertificateStore updates.
        /// </summary>
        /// <param name="certificateStoreFile">The location to save the CertificateStore file.</param>
        /// <param name="certificateStoreUrl">The server url to check.</param>
        /// <param name="lastServerCheck">The date of the last server check.</param>
        internal static void Initialize(String certificateStoreFile, String certificateStoreUrl, DateTime lastServerCheck)
        {
            if (allowInitialize)
            {
                CertificateStoreManager.certificateStoreFile = certificateStoreFile;
                CertificateStoreManager.certificateStoreUrl = certificateStoreUrl;
                CertificateStoreManager.lastServerCheck = lastServerCheck;
                allowInitialize = false;

                //Really important to do this last, or else allowInitialize won't be false and you can't refresh.
                ThreadPool.QueueUserWorkItem(arg => GetCertificate());
            }
            else
            {
                throw new Exception("Can only Initialize the CertificateStoreManager one time.");
            }
        }

        /// <summary>
        /// Attempt to refresh the CertificateStore from the server, this will only happen if the program has not
        /// already checked the server once on this run. You must call initialize before calling this or an exception will
        /// be thrown.
        /// </summary>
        public static bool RefreshCertificate()
        {
            if (allowInitialize)
            {
                throw new Exception("Attempted to refresh the certificate store without initializing first.");
            }

            //Only check the server one time per run.
            if (!CheckedServerThisRun)
            {
                lastServerCheck = DateTime.MinValue;
                GetCertificate();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to refresh the CertificateStore from the server, this will only happen if the program has not
        /// already checked the server once on this run. You must call initialize before calling this or an exception will
        /// be thrown. This version will run on a background thread.
        /// </summary>
        public static void RefreshCertificateBg()
        {
            if (allowInitialize)
            {
                throw new Exception("Attempted to refresh the certificate store without initializing first.");
            }

            //Only check the server one time per run.
            if (!CheckedServerThisRun)
            {
                lastServerCheck = DateTime.MinValue;
                ThreadPool.QueueUserWorkItem(arg => GetCertificate());
            }
        }

        /// <summary>
        /// Helper function to actually refresh the certificate. This will only actually check the server if the lastServerCheck time is
        /// more than CheckDelay in the past and if the server has not been checked on this run of the program.
        /// </summary>
        private static void GetCertificate()
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

            bool checkServer = true;
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
                    if (DateTime.Now > lastServerCheck.Add(CheckDelay))
                    {
                        serverConnection.addArgument("clientIssueDate", CertificateStore.IssueDate.Ticks.ToString());
                    }
                    else
                    {
                        checkServer = false;
                    }
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

            if (checkServer)
            {
                try
                {
                    serverConnection.NonOkResultEvent += conn =>
                    {
                        //This will happen if the serverConnection gets a non ok response.
                        onServerContacted();
                    };

                    serverConnection.makeRequestDownloadResponse(responseStream =>
                        {
                            //This only happens if the server returns a certificate store.
                            onServerContacted();
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
        }

        /// <summary>
        /// This will be true if an update to the certificate store was checked for on this run of the program.
        /// </summary>
        public static bool CheckedServerThisRun { get; set; }

        /// <summary>
        /// The current certificate store, note that this could be replaced for a variety of reasons, so it is best not to store
        /// a local copy and instead reference this property when access is needed.
        /// </summary>
        public static CertificateStore CertificateStore { get; private set; }

        /// <summary>
        /// The amount of time between certificate store update checks. Default is 1 day.
        /// </summary>
        public static TimeSpan CheckDelay { get; set; }

        /// <summary>
        /// Check to see if the given data from the server has a valid signature.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="signature">The signature</param>
        /// <returns>True if signature is valid for data.</returns>
        public static bool IsValidServerCommunication(byte[] data, byte[] signature)
        {
            return IsValidServerCommunication(data, signature, true);
        }

        /// <summary>
        /// Helper function to check certificate data, can potentially refresh the certificate store from the server if it is out of date.
        /// </summary>
        private static bool IsValidServerCommunication(byte[] data, byte[] signature, bool refreshCertStoreOnError)
        {
            bool valid = false;
            RSACryptoServiceProvider rsaService = CertificateStore.ServerCommunicationCertificate.RSA as RSACryptoServiceProvider;
            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashAlgoName))
            {
                byte[] hash = hashAlgorithm.ComputeHash(data);
                valid = rsaService.VerifyHash(hash, hashAlgoOid, signature);
            }
            if(!valid && refreshCertStoreOnError)
            {
                if (RefreshCertificate())
                {
                    valid = IsValidServerCommunication(data, signature, false);
                }
            }
            return valid;
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

        /// <summary>
        /// Called when the certificate store server is contacted.
        /// </summary>
        private static void onServerContacted()
        {
            CheckedServerThisRun = true;
            ThreadManager.invoke(() =>
            {
                if (ServerCheckedForCertificate != null)
                {
                    ServerCheckedForCertificate.Invoke();
                }
            });
        }
    }
}
