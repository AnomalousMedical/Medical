using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical.Controller
{
    class CredentialsManager : IDisposable
    {
        private String tempStringStorage;
        private IntPtr credentialsManager;

        public CredentialsManager()
        {
            getStringCallback = new GetStringDelegate(stringCallbackFunc);
            credentialsManager = CredentialsManager_create(MedicalConfig.CredentialsFile);
        }

        public void Dispose()
        {
            CredentialsManager_delete(credentialsManager);
        }

        public void secureStoreCredentials(String user, String pass)
        {
            CredentialsManager_secureStoreCredentials(credentialsManager, user, pass);
        }

        public void deleteSecureCredentials()
        {
            CredentialsManager_deleteSecureCredentials(credentialsManager);
        }

        public bool HasStoredCredentials
        {
            get
            {
                return CredentialsManager_getHasStoredCredentials(credentialsManager);
            }
        }

        public String Username
        {
            get
            {
                CredentialsManager_getUsername(credentialsManager, getStringCallback);
                String retVal = tempStringStorage;
                tempStringStorage = null;
                return retVal;
            }
        }

        public String Password
        {
            get
            {
                CredentialsManager_getPassword(credentialsManager, getStringCallback);
                String retVal = tempStringStorage;
                tempStringStorage = null;
                return retVal;
            }
        }

        #region PInvoke

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GetStringDelegate(String data);

        GetStringDelegate getStringCallback;

        private void stringCallbackFunc(String data)
        {
            tempStringStorage = data;
        }

        [DllImport("OSHelper")]
        private static extern IntPtr CredentialsManager_create(String file);

        [DllImport("OSHelper")]
        private static extern void CredentialsManager_delete(IntPtr credentialsManager);

        [DllImport("OSHelper")]
        private static extern void CredentialsManager_secureStoreCredentials(IntPtr credentialsManager, String user, String pass);

        [DllImport("OSHelper")]
        private static extern void CredentialsManager_deleteSecureCredentials(IntPtr credentialsManager);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool CredentialsManager_getHasStoredCredentials(IntPtr credentialsManager);

        [DllImport("OSHelper")]
        private static extern void CredentialsManager_getUsername(IntPtr credentialsManager, GetStringDelegate stringCallback);

        [DllImport("OSHelper")]
        private static extern void CredentialsManager_getPassword(IntPtr credentialsManager, GetStringDelegate stringCallback);

        #endregion
    }
}
