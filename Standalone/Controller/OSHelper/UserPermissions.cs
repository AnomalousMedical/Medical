using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical.Controller
{
    public enum Features
    {
        ANOMALOUS_MEDICAL_KEY = 0,
        PIPER_JBO_MODULE = 1,
        PIPER_JBO_WIZARD_DOPPLER = 2,
        PIPER_JBO_WIZARD_DENTITION = 3,
        PIPER_JBO_WIZARD_CEPHALOMETRIC = 4,
        PIPER_JBO_WIZARD_MANDIBLE = 5,
        PIPER_JBO_WIZARD_DISC_CLOCK = 6,
        PIPER_JBO_WIZARD_DISC_SPACE = 7,
        PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION = 8,
        PIPER_JBO_WIZARD_CLINICAL_DOPPLER = 9,
        PIPER_JBO_WIZARD_RADIOGRAPHY = 10,
        PIPER_JBO_WIZARD_MRI = 11,
        PIPER_JBO_VERSION_DOPPLER = 12,
        PIPER_JBO_VERSION_DENTITION_PROFILE = 13,
        PIPER_JBO_VERSION_CLINICAL = 14,
        PIPER_JBO_VERSION_RADIOGRAPHY_CT = 15,
        PIPER_JBO_VERSION_MRI = 16,
        PIPER_JBO_VERSION_GRAPHICS = 17,
        PIPER_JBO_FEATURE_CUSTOM_LAYERS = 18,
        PIPER_JBO_FEATURE_FULL_RENDERING = 19,
        PIPER_JBO_FEATURE_CLONE_WINDOW = 20,
        PIPER_JBO_FEATURE_SIMULATION = 21,
    }

    public enum ConnectionResult
    {
	    Ok = 1,
	    NoDongle = 2,
	    TooManyUsers = 3,
    }

#if !ENABLE_HASP_PROTECTION
    public enum SimulatedVersion
    {
	    Doppler,
	    DentitionAndProfile,
	    Clinical,
	    Radiography,
	    MRI,
	    Graphics
    }
#endif

    public class UserPermissions : IDisposable
    {
        public static UserPermissions Instance { get; private set; }

        private IntPtr userPermissions;
        private String featureLevelString;

#if !ENABLE_HASP_PROTECTION
        public UserPermissions(SimulatedVersion simVersion)
        {
            userPermissions = UserPermissions_create(simVersion);
#else
        public UserPermissions()
        {
            userPermissions = UserPermissions_create();
#endif
            if (Instance != null)
            {
                throw new Exception("Can only create UserPermissions one time");
            }
            Instance = this;

            if (allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                featureLevelString = "Graphics Edition";
            }
            else if (allowFeature(Features.PIPER_JBO_VERSION_MRI) || allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                featureLevelString = "Imaging Edition";
            }
            else if (allowFeature(Features.PIPER_JBO_VERSION_CLINICAL) || 
                allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE) ||
                allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            {
                featureLevelString = "Clinical Edition";
            }
        }

        public void Dispose()
        {
            UserPermissions_destroy(userPermissions);
            userPermissions = IntPtr.Zero;
        }

        public bool allowFeature(Features featureId)
        {
            return UserPermissions_allowFeature(userPermissions, featureId);
        }

	    public ConnectionResult checkConnection()
        {
            return UserPermissions_checkConnection(userPermissions);
        }

        public String Id
        {
            get
            {
                return Marshal.PtrToStringAnsi(UserPermissions_getId(userPermissions));
            }
        }

        public String FeatureLevelString
        {
            get
            {
                return featureLevelString;
            }
        }

#region PInvoke

    #if !ENABLE_HASP_PROTECTION
        [DllImport("OSHelper")]
        private static extern IntPtr UserPermissions_create(SimulatedVersion simVersion);
#else 
        [DllImport("OSHelper")]
        private static extern IntPtr UserPermissions_create();
#endif

        [DllImport("OSHelper")]
        private static extern void UserPermissions_destroy(IntPtr permissions);

        [DllImport("OSHelper")]
        private static extern bool UserPermissions_allowFeature(IntPtr permissions, Features featureId);

        [DllImport("OSHelper")]
        private static extern ConnectionResult UserPermissions_checkConnection(IntPtr permissions);

        [DllImport("OSHelper")]
        private static extern IntPtr UserPermissions_getId(IntPtr permissions);

#endregion
    }
}
