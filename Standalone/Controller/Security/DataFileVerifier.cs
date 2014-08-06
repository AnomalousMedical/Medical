using Anomalous.Security;
using Logging;
using Mono.Security.Authenticode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class can check signatures on data files and dlls.
    /// </summary>
    public class DataFileVerifier
    {
        CertificateStore certificateStore;

        public DataFileVerifier(CertificateStore certificateStore)
        {
            this.certificateStore = certificateStore;
            AllowUnsignedDataFiles = false;
            AllowUnsignedDlls = false;
        }

        public bool AllowUnsignedDataFiles { get; set; }

        public bool AllowUnsignedDlls { get; set; }

        public bool isSafeDataFile(String file)
        {
            if (AllowUnsignedDataFiles)
            {
                return true;
            }

            bool valid = false;
            SignedDataFile signedFile = new SignedDataFile(certificateStore);
            switch (signedFile.isTrustedFile(file))
            {
                case SignedDataFile.TrustedResult.Valid:
                    valid = true;
                    break;
                case SignedDataFile.TrustedResult.FileNotFound:
                    Log.Error("Plugin '{0}' is not found.", file);
                    break;
                case SignedDataFile.TrustedResult.InvalidSignature:
                    Log.Error("Plugin '{0}' signature is invalid.", file);
                    break;
                case SignedDataFile.TrustedResult.InvalidChain:
                    Log.Error("Plugin '{0}' chain is invalid. Reason: {1}", file, signedFile.Chain.Status);
                    break;
                case SignedDataFile.TrustedResult.InvalidChainCA:
                    Log.Error("Plugin '{0}' does not have a valid CA in its chain.", file);
                    break;
                case SignedDataFile.TrustedResult.SignatureCertRevoked:
                    Log.Error("Plugin '{0}' signature has been revoked.", file);
                    break;
                case SignedDataFile.TrustedResult.NotSigned:
                    Log.Error("Plugin '{0}' is not signed.", file);
                    break;
                case SignedDataFile.TrustedResult.TimestampOutsideRange:
                    Log.Error("Plugin '{0}' timestamp {1} is outside the valid range {2} - {3}.", file, signedFile.Timestamp, signedFile.Certificate.ValidFrom, signedFile.Certificate.ValidUntil);
                    break;
                case SignedDataFile.TrustedResult.UnspecifiedError:
                    Log.Error("Plugin '{0}' an unspecified error occured. {1}", file, signedFile.UnspecifiedErrorMessage);
                    break;
                case SignedDataFile.TrustedResult.InvalidCounterSignature:
                    Log.Error("Plugin '{0}' counter signature hash does not match.", file);
                    break;
                case SignedDataFile.TrustedResult.InvalidCounterSignatureChain:
                    Log.Error("Plugin '{0}' counter signature chain is invalid. Reason: {1}", file, signedFile.CounterSignatureChain.Status);
                    break;
                case SignedDataFile.TrustedResult.InvalidCounterSignatureChainCA:
                    Log.Error("Plugin '{0}' does not have a valid CA in its counter signature chain.", file);
                    break;
                case SignedDataFile.TrustedResult.CounterSignatureCertRevoked:
                    Log.Error("Plugin '{0}' counter signature has been revoked.", file);
                    break;
            }
            return valid;
        }

        public bool isSafeDll(String path)
        {
            if (AllowUnsignedDlls)
            {
                return true;
            }

            bool safe = false;
            AuthenticodeDeformatter ad = new AuthenticodeDeformatter();
            ad.SetupX509Chains += ad_SetupX509Chains;
            ad.FileName = path; //Must do this here because it does the check when the file is set.
            if (ad.IsTrusted())
            {
                if (certificateStore.ValidDllCertificates.Contains(ad.SigningCertificate))
                {
                    safe = true;
                }
                else
                {
                    Log.Error("Plugin '{0}' is signed by a certificate that does not belong to Anomalous Medical or is untrusted.", path);
                }
            }
            else
            {
                Log.Error("Plugin '{0}' is improperly signed.", path);
            }

            return safe;
        }

        void ad_SetupX509Chains(Mono.Security.X509.X509Chain signerChain, Mono.Security.X509.X509Chain timestampChain)
        {
            signerChain.TrustAnchors.AddRange(certificateStore.TrustAnchors);
            timestampChain.TrustAnchors.AddRange(certificateStore.TrustAnchors);
        }
    }
}
