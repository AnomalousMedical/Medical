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
    public class PluginVerifier
    {
        private static readonly String Sha1OID = CryptoConfig.MapNameToOID("SHA1");
        private static SHA1Managed sha1 = new SHA1Managed();

        byte[] authorityBytes;

        internal PluginVerifier(Assembly authorityAssembly)
        {
            AuthenticodeDeformatter authorityAd = new AuthenticodeDeformatter(Assembly.GetExecutingAssembly().Location);
            if (authorityAd.SigningCertificate != null)
            {
                authorityBytes = authorityAd.SigningCertificate.RawData;
            }
        }

        internal PluginVerifier(Stream certStream)
        {
            using (BinaryReader br = new BinaryReader(certStream))
            {
                authorityBytes = br.ReadBytes((int)certStream.Length);
            }
        }

        public static void SignDataFile(String file, String outFile, X509Certificate2 cert)
        {
            byte[] signature;
            RSACryptoServiceProvider privateKey = cert.PrivateKey as RSACryptoServiceProvider;
            byte[] hash;
            byte[] publicKeyBytes = cert.Export(X509ContentType.Cert);
            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                hash = sha1.ComputeHash(stream);
                signature = privateKey.SignHash(hash, Sha1OID);
                stream.Seek(0, SeekOrigin.Begin);
                using (BinaryWriter outStream = new BinaryWriter(File.Open(outFile, FileMode.Create, FileAccess.Write)))
                {
                    stream.CopyTo(outStream.BaseStream);
                    long footerStart = stream.Position;
                    outStream.Write(signature.LongLength);
                    outStream.Write(signature, 0, signature.Length);
                    outStream.Write(publicKeyBytes.LongLength);
                    outStream.Write(publicKeyBytes, 0, publicKeyBytes.Length);
                    long nowTicks = DateTime.UtcNow.Ticks;
                    outStream.Write(nowTicks);
                    outStream.Write(footerStart);
                }
            }
        }

        internal bool isSafeDataFile(String file)
        {
#if ALLOW_OVERRIDE
            if (MedicalConfig.AllowUnsignedPlugins)
            {
                return true;
            }
#endif

            try
            {
                byte[] signature;
                X509Certificate2 cert;
                using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
                {
                    reader.BaseStream.Seek(reader.BaseStream.Length - sizeof(long), SeekOrigin.Begin);
                    long footerStart = reader.ReadInt64();
                    reader.BaseStream.Seek(footerStart, SeekOrigin.Begin);
                    long signatureLength = reader.ReadInt64();
                    signature = reader.ReadBytes((int)signatureLength);
                    long publicKeyLength = reader.ReadInt64();
                    byte[] publicKeyBytes = reader.ReadBytes((int)publicKeyLength);
                    cert = new X509Certificate2(publicKeyBytes);
                    DateTime timestamp = new DateTime(reader.ReadInt64());

                    if (timestamp <= cert.NotAfter && timestamp >= cert.NotBefore)
                    {
                        if (cert.RawData.SequenceEqual(authorityBytes))
                        {
                            RSACryptoServiceProvider publicKey = cert.PublicKey.Key as RSACryptoServiceProvider;
                            reader.BaseStream.Seek(0, SeekOrigin.Begin);
                            byte[] hash;
                            using (Stream stream = new SignedStream(reader.BaseStream, footerStart))
                            {
                                hash = sha1.ComputeHash(stream);
                            }
                            if (publicKey.VerifyHash(hash, Sha1OID, signature))
                            {
                                return true;
                            }
                            else
                            {
                                Log.Error("Plugin '{0}' is improperly signed.", file);
                            }
                        }
                        else
                        {
                            Log.Error("Plugin '{0}' is signed but does not match the authority.", file);
                        }
                    }
                    else
                    {
                        Log.Error("Invalid timestamp on plugin '{0}'.", file);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception reading signature info for plugin '{0}' Message '{1}'.", file, ex.Message);
                return false;
            }
        }

        internal bool isSafeDll(String path)
        {
            bool safe = false;
            if (authorityBytes != null)
            {
                AuthenticodeDeformatter ad = new AuthenticodeDeformatter(path);
                if (ad.IsTrusted() || ad.Reason == 6) //Allow if passes or just if the cert authority cannot be trusted
                {
                    if (ad.SigningCertificate.RawData.SequenceEqual(authorityBytes))
                    {
                        safe = true;
                    }
                    else
                    {
                        Log.Error("Plugin '{0}' is signed, but does not match the authority.", path);
                    }
                }
                else
                {
                    Log.Error("Plugin '{0}' is improperly signed.", path);
                }
            }
            else
            {
                Log.Error("PluginVerifier Authority is improperly signed.", path);
            }

#if ALLOW_OVERRIDE
            if(MedicalConfig.AllowUnsignedPlugins)
            {
                safe = true;
            }
#endif

            return safe;
        }
    }
}
