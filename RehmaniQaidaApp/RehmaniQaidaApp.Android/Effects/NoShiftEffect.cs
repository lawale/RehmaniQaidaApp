
using Android.Support.Design.BottomNavigation;
using Android.Support.Design.Widget;
using Android.Views;
using RehmaniQaidaApp.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("RehmaniQaidaApp")]
[assembly: ExportEffect(typeof(NoShiftEffect), nameof(NoShiftEffect))]
namespace RehmaniQaidaApp.Droid.Effects
{
    public class NoShiftEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (!(Container.GetChildAt(0) is ViewGroup layout))
                return;
            if (!(layout.GetChildAt(1) is BottomNavigationView bottomNavigationView))
                return;

            bottomNavigationView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;
        }

        protected override void OnDetached()
        {
        }
    }
}