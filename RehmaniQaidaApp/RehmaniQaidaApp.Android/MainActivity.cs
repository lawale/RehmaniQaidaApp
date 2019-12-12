using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XF.Material.Droid;
using Plugin.CurrentActivity;
using MediaManager;

namespace RehmaniQaidaApp.Droid
{
    [Activity(Label = "Rehmani Qaida App", Icon = "@mipmap/ic_launcher", Theme = "@style/Splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        bool isRunning = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            if (!isRunning)
            {
                base.Window.RequestFeature(WindowFeatures.ActionBar);
                base.SetTheme(Resource.Style.MainTheme);
                isRunning = true;
            }
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Material.Init(this, savedInstanceState);
            CrossMediaManager.Current.Init(this);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}