using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Engine
{
    class LicenseInvalidException : Exception
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
                byte[] hashedData;
                byte[] realData;
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(licenseData)))
                {
                    hashedData = new byte[binaryReader.ReadInt32()];
                    binaryReader.Read(hashedData, 0, hashedData.Length);
                    realData = new byte[binaryReader.ReadInt32()];
                    binaryReader.Read(realData, 0, realData.Length);
                }

                RSACryptoServiceProvider decrypt = new RSACryptoServiceProvider();
                decrypt.FromXmlString(Medical.MedicalConfig.ServerPublicKey);
                match = decrypt.VerifyData(realData, new SHA1CryptoServiceProvider(), hashedData);
                if (match)
                {
                    using (StreamReader textReader = new StreamReader(new MemoryStream(realData)))
                    {
                        MachineID = textReader.ReadLine();
                        LicenseeName = textReader.ReadLine();
                        User = textReader.ReadLine();
                        Pass = textReader.ReadLine();
                        String feature;
                        while ((feature = textReader.ReadLine()) != null)
                        {
                            features.Add(NumberParser.ParseLong(feature));
                        }
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

        public String MachineID { get; private set; }

        public String LicenseeName { get; private set; }

        public String User { get; private set; }

        public String Pass { get; private set; }
    }
}
