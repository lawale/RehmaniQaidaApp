using RehmaniQaidaApp.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using RehmaniQaidaApp.Views;
using System.Threading.Tasks;
using RehmaniQaidaApp.Extensions;

namespace RehmaniQaidaApp.ViewModels
{
    public class MasterViewModel : BaseViewModel
    {
        /// <summary>
        /// Command to execute for the Tapped Event of Menu
        /// </summary>
        public ICommand MenuCommand { get; }

        public MasterViewModel()
        {
            MenuCommand = new Command<MenuOption>(async menu => await ExecuteMenuCommand(menu));
        }

        private async Task ExecuteMenuCommand(MenuOption option)
        {
            switch (option)
            {
                case MenuOption.About:
                    await NavigateTo(new AboutViewModel(), true);
                    break;
                case MenuOption.Lesson:
                    await NavigateTo(new LessonsViewModel(), true);
                    break;
                case MenuOption.Quiz:
                    await NavigateTo(new QuizViewModel(), true);
                    break;
                case MenuOption.RateApp:
                    DependencyService.Get<IRateApp>().RateApp();
                    break;
                case MenuOption.ShareApp:
                    ShareMessge();
                    break;
                case MenuOption.UpdateApp:
                    break;
                case MenuOption.Default:
                default:
                    await NavigateTo(new BaseViewModel(), true);
                    break;
            }
        }

        private void ShareMessge()
        {
            var message = "Hey, Check out the Rehmani Qaida ";
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    message += "Android App at: https://play.google.com/store/apps/details?id=com.companyname.RehmaniQaidaApp";
                    break;
                case Device.iOS:
                    message += "iOS App at: itms-apps://itunes.apple.com/app/APP_ID";
                    break;
            }
            Xamarin.Essentials.Share.RequestAsync(message, "Rehmani Qaida");
        }
    }
}
