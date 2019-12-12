using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using RehmaniQaidaApp.Droid.Extensions;
using RehmaniQaidaApp.Extensions;
using Xamarin.Forms;

[assembly: Dependency(typeof(RateApp))]
namespace RehmaniQaidaApp.Droid.Extensions
{
    public class RateApp : IRateApp
    {
        void IRateApp.RateApp()
        {
            var activity = Android.App.Application.Context;
            var url = $"market://details?id={(activity as Context)?.PackageName}";

            try
            {
                activity.PackageManager.GetPackageInfo("com.android.vending", PackageInfoFlags.Activities);
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));

                activity.StartActivity(intent);
            }
            catch (PackageManager.NameNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ActivityNotFoundException)
            {
                var playStoreUrl = $"https://play.google.com/store/apps/details?id={activity.PackageName}";
                var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(playStoreUrl));
                browserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);
                activity.StartActivity(browserIntent);
            }
        }
    }
}