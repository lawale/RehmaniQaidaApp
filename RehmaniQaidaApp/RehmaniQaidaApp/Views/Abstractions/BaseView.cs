using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace RehmaniQaidaApp.Views.Abstractions
{
    public class BaseView<TViewModel> : ContentPage, IView<TViewModel> where TViewModel : BaseViewModel, new()
    {

        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(BindingContextProperty);
            set => SetValue(BindingContextProperty, value);
        }

        public BaseView()
        {
            //Title = ViewModel?.Title;
        }

        

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.OnNavigationRequest = HandleNavigationRequest;
            ViewModel.OnPopRequest = HandlePopRequest;
            ViewModel.OnReplaceIndexRequest = HandleIndexRequest;
            await ViewModel.OnViewAppeared();
        }



        private async Task HandleIndexRequest(BaseViewModel viewModel)
        {
            var view = ViewResolver.GetViewFor(viewModel);
            view.BindingContext = viewModel;
            var rootView = (Application.Current.MainPage as MasterDetailPage).Detail;
            var lastView = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.NavigationStack.Last();
            rootView.Navigation.InsertPageBefore(view, lastView);
            await rootView.Navigation.PopAsync();
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.OnNavigationRequest =  null;
            ViewModel.OnPopRequest = null;
            ViewModel.OnReplaceIndexRequest = null;
            await ViewModel.OnViewDisappeared();
        }

        private async Task HandlePopRequest(bool toRoot)
        {
            if (toRoot)
            {
                await Navigation.PopToRootAsync();
                return;
            }
            await Navigation.PopAsync();
        }

        private async Task HandleNavigationRequest(BaseViewModel viewModel, bool isMasterDetailNavigation)
        {
            var view = ViewResolver.GetViewFor(viewModel);
            view.BindingContext = viewModel;
            if (isMasterDetailNavigation)
            {
                var rootView = (Application.Current.MainPage as MasterDetailPage).Detail;
                var firstView = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.NavigationStack.First();
                if (firstView.GetType() == view.GetType())
                    return;
                rootView.Navigation.InsertPageBefore(view, firstView);
                await rootView.Navigation.PopToRootAsync();
                (Application.Current.MainPage as MasterDetailPage).IsPresented = false;
                return;
            }
            
            await Navigation.PushAsync(view);
        }
    }
}
