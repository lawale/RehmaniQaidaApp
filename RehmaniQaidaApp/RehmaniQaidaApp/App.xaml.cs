using Akavache;
using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.Resources.Typography;
using XF.Material.Forms.UI;
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

            var detailPage = new MaterialNavigationPage(new LessonsView
            {
                ViewModel = new LessonsViewModel()
            });
            MainPage = new MasterDetailPage
            {
                Master = masterPage,
                Detail = detailPage
            };
        }

        void InitMaterialStyle()
        {
            
            var colorConfig = new MaterialColorConfiguration
            {
                Primary = Color.FromHex("#003366"),
            };
            var fontConfig = new MaterialFontConfiguration();
            var config = new MaterialConfiguration
            {
                ColorConfiguration = colorConfig,
                FontConfiguration = fontConfig
            };

            Material.Init(this, config);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
