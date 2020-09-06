using Akavache;
using Firebase.Storage;
using RehmaniQaidaApp.Extensions;
using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.Resources.Typography;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace RehmaniQaidaApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitMaterialStyle();
            BlobCache.ApplicationName = "RehmaniQaidaApp";



            var masterPage = new MasterView
            {
                Title = "Menu",
                ViewModel = new MasterViewModel
                {
                    Title = "menu"
                }
            };

            //var detailPage = new MaterialNavigationPage(new LessonsView
            //{
            //    ViewModel = new LessonsViewModel()
            //});

            var detailPage = new MaterialNavigationPage(new HomeView
            {
                ViewModel = new HomeViewModel()
            })
            {
                BarBackgroundColor = Color.FromHex("#003366"),
                BarTextColor = Color.White
            };
            MainPage = new MasterDetailPage
            {
                Master = masterPage,
                Detail = detailPage
            };
        }

        async Task RegisterCacheHelper()
        {
            await DependencyService.Get<IBlobCacheInstanceHelper>().Init();
            
            //cacheInstanceHelper.CopyDb();
        }

        void InitMaterialStyle()
        {
            Material.Init(this);
            var loadingDialogConfig = new MaterialLoadingDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#003366"),
                MessageTextColor = Color.White,
                TintColor = Color.White,
            };

            var snackbarConfig = new MaterialSnackbarConfiguration
            {
                BackgroundColor = Color.White,
                TintColor = Color.FromHex("#003366"),
                MessageTextColor = Color.FromHex("#003366")
            };

            var confirmationDialogConfig = new MaterialConfirmationDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#003366"),
                TintColor = Color.White,
                ControlSelectedColor = Color.White,
                TextColor = Color.White,
                TitleTextColor = Color.White
            };

            var dialogConfig = new MaterialAlertDialogConfiguration
            {
                BackgroundColor = Color.FromHex("#003366"),
                TintColor = Color.White,
                MessageTextColor = Color.White,
                TitleTextColor = Color.White
            };

            MaterialDialog.Instance.SetGlobalStyles(
                loadingDialogConfiguration: loadingDialogConfig, 
                snackbarConfiguration: snackbarConfig, 
                confirmationDialogConfiguration: confirmationDialogConfig,
                dialogConfiguration: dialogConfig);
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            await RegisterCacheHelper();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
