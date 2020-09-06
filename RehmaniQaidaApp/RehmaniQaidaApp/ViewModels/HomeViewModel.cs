using RehmaniQaidaApp.Extensions;
using RehmaniQaidaApp.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RehmaniQaidaApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ICommand NavigateCommand { get; }

        public HomeViewModel()
        {
            NavigateCommand = new Command<string>(async param => await ExecuteNavigateCommand(param));
        }
        private async Task ExecuteNavigateCommand(string param)
        {
            Enum.TryParse(param, out MenuOption option);
            switch (option)
            {
                case MenuOption.About:
                    await NavigateTo(new AboutViewModel(), true);
                    break;
                case MenuOption.Home:
                    await NavigateTo(new HomeViewModel(), true);
                    break;
                case MenuOption.Lesson:
                    await NavigateTo(new LessonsViewModel(), true);
                    break;
                case MenuOption.Quiz:
                    await NavigateTo(new QuizViewModel(), true);
                    break;
                case MenuOption.Color:
                    await NavigateTo(new LessonColorChartViewModel(), true);
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
                    await NavigateTo(new HomeViewModel(), true);
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
