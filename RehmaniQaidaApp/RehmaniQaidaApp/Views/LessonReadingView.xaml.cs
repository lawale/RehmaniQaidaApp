using FFImageLoading.Forms;
using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RehmaniQaidaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LessonReadingView : BaseView<LessonReadingViewModel>
    {
        public LessonReadingView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel.Title = (Parent as TabbedPage).Title;
            if (ViewModel.Readings == null)
                ViewModel.GetLessonResourcesCommand.Execute(null);
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //    {
            //        if (ViewModel.Position == ViewModel.Count - 1 && ViewModel.OnNavigationRequest != null)
            //        {
            //            list.ScrollTo(ViewModel.Position, position: ScrollToPosition.Center, animate: true);
                        
            //            return true;
            //        }
            //        return false;
            //    });
            //});
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void ScrollToReading(object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e)
        {
            list.ScrollTo(ViewModel.Position, position: ScrollToPosition.Center, animate: true);
        }

        private void PlayAudio(object sender, EventArgs e)
        {
            ViewModel.PlayAudioFiles.Execute(null);
        }

        private void OnItemTapped(object sender, EventArgs e)
        {
            var contentView = (ContentView)sender;
            var image = (CachedImage)contentView.Content;
            ViewModel.ItemSelectedCommand.Execute(image.Source);
        }
    }
}