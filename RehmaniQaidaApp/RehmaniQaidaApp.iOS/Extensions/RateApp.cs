using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using RehmaniQaidaApp.Extensions;
using RehmaniQaidaApp.iOS.Extensions;
using StoreKit;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(RateApp))]
namespace RehmaniQaidaApp.iOS.Extensions
{
    public class RateApp : IRateApp
    {
        void IRateApp.RateApp()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
                SKStoreReviewController.RequestReview();
            else
            {
                var storeUrl = "itms-apps://itunes.apple.com/app/APPID";
                var url = storeUrl + "?action=write-review";

                try
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}