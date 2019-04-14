using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Anomalous.OSPlatform.Android;

namespace AnomalousMedicalAndroid
{
    [Activity(Label = "Anomalous Medical", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AnomalousMedicalTheme",
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout,
    WindowSoftInputMode = SoftInput.StateAlwaysHidden, LaunchMode = LaunchMode.SingleTop)]
    public class PermissionActivity : Activity
    {
        public PermissionActivity()
        {
        }

        protected PermissionActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int)Build.VERSION.SdkInt < 23)
            {
                StartMainActivity();
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    || PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                    RequestPermissions(permissions, 1);
                }
                else
                {
                    StartMainActivity();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            StartMainActivity();
        }

        private void StartMainActivity()
        {
            StartActivity(typeof(MainActivity));
        }
    }
}