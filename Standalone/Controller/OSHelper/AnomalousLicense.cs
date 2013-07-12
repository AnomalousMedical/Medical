using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Medical;
using Mono.Security;

namespace Engine
{
    public class LicenseInvalidException : Exception
    {
        public LicenseInvalidException(String message)
            :base(message)
        {

        }
    }

    /// <summary>
    /// This class represents a license file. It does not do any validation of
    /// its own, but will contain values that can be validated.
    /// </summary>
    class AnomalousLicense
    {
        private List<long> features = new List<long>();

        public AnomalousLicense(byte[] licenseData)
        {
            bool match = false;
            try
            {
                //Extract the file contents
                for (int i = 0; i < licenseData.Length; ++i)
                {
                    licenseData[i] ^= 125;
                }
                byte[] signature;
                byte[] realData;
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(licenseData)))
                {
                    signature = new byte[binaryReader.ReadInt32()];
                    binaryReader.Read(signature, 0, signature.Length);
                    realData = new byte[binaryReader.ReadInt32()];
                    binaryReader.Read(realData, 0, realData.Length);
                }

                match = CertificateStoreManager.IsValidServerCommunication(realData, signature);
                if (match)
                {
                    ASN1 license = new ASN1(realData);
                    LicenseeName = Encoding.BigEndianUnicode.GetString(license.Element(0, 0x1E).Value);
                    User = Encoding.BigEndianUnicode.GetString(license.Element(1, 0x1E).Value);
                    Pass = Encoding.BigEndianUnicode.GetString(license.Element(2, 0x1E).Value);
                    ASN1 featuresAsn1 = license.Element(3, 0x30);
                    for(int i = 0; i < featuresAsn1.Count; ++i)
                    {
                        features.Add(BitConverter.ToInt64(featuresAsn1[i].Value, 0));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new LicenseInvalidException(String.Format("{0} Exception occured. Message: {1}", ex.GetType().ToString(), ex.Message));
            }
            if (!match)
            {
                throw new LicenseInvalidException("Invalid signature on license file.");
            }
        }

        public void addFeature(long featureID)
        {
            features.Add(featureID);
        }

        public void removeFeature(long featureID)
        {
            features.Remove(featureID);
        }

        public bool supportsFeature(long featureID)
        {
            return features.Contains(featureID);
        }

        public String LicenseeName { get; private set; }

        public String User { get; private set; }

        public String Pass { get; private set; }
    }
}
