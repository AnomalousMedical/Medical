using System;
using Android.App;
using ExpansionDownloader.Service;

namespace AnomalousMedicalAndroid
{
    [Service]
    public class AnomalousMedicalDownloaderService : DownloaderService
    {
        /// <summary>
        /// This public key comes from your Android Market publisher account, and it
        /// used by the LVL to validate responses from Market on your behalf.
        /// Note: MODIFY FOR YOUR APPLICATION!
        /// </summary>
        protected override string PublicKey
        {
            get
            {
                return "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApOyxKxzFWqff/zYyMIrGFVkkghqLIz3dFM208auj3B4HFNgRzct5peVsmMGxYE1WAtOMFcf7FunfnSGGEUVRWt0pv4AtDNvZ0di0ZOd3nYa8UOjMXBZX6DMYYrRbt55SMW/EmUu5JhBCTZB9TQONLe3F9+a1c6I4KLog2EdPEBQpvD5X6vn57edX8PQ7uqtZNMGF5L3wHc5tktQORn5wadiAG1OiIBkXB9AlB5Vl4azfANEaHdim1ahHvKA/QTZra3SUeoRYeS8BO2YjvvSkczLoxC/6/0OBddfAYpCvHUEBdGDTbbD5DbgBnI4fpRy/Ma2YBawJtDmzc2SPCBA2wwIDAQAB";
            }
        }

        /// <summary>
        /// This is used by the preference obfuscater to make sure that your
        /// obfuscated preferences are different than the ones used by other
        /// applications.
        /// </summary>
        protected override byte[] Salt
        {
            get
            {
                return new byte[] { 1, 43, 12, 1, 54, 98, 100, 12, 43, 2, 8, 4, 9, 5, 106, 108, 33, 45, 1, 84 };
            }
        }

        /// <summary>
        /// Fill this in with the class name for your alarm receiver. We do this
        /// because receivers must be unique across all of Android (it's a good idea
        /// to make sure that your receiver is in your unique package)
        /// </summary>
        protected override string AlarmReceiverClassName
        {
            get
            {
                return "expansiondownloader.sample.SampleAlarmReceiver";
            }
        }
    }
}

