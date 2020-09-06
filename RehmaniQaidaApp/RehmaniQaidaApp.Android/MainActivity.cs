using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XF.Material.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using FFImageLoading.Forms.Platform;
using CarouselView.FormsPlugin.Android;
using Plugin.Permissions;
using Android.Content;
using Plugin.LocalNotification;

namespace RehmaniQaidaApp.Droid
{
    [Activity(Label = "Rehmani Qaida App", Icon = "@mipmap/ic_launcher", Theme = "@style/Splashscreen", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
            Forms.SetFlags("CarouselView_Experimental", "IndicatorView_Experimental");
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Material.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            CarouselViewRenderer.Init();
            NotificationCenter.CreateNotificationChannel();
            //NotificationCenter.CreateNotificationChannel(new NotificationChannelRequest
            //{
            //    Id = $"my_channel_01",
            //    Name = "General",
            //    Description = "General",
                
            //});
            LoadApplication(new App());
            NotificationCenter.NotifyNotificationTapped(Intent);
        }




        public override void OnBackPressed()
        {
            Material.HandleBackButton(base.OnBackPressed);

        }

        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}