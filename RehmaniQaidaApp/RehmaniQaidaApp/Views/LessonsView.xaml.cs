using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;

namespace RehmaniQaidaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LessonsView : BaseView<LessonsViewModel>
    {
        public LessonsView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindableLayout.SetItemsSource(flex, ViewModel.Lessons);
        }

        private void LessonClicked(object sender, EventArgs e)
        {
            var btn = sender as MaterialButton;
            var param = btn.CommandParameter;
            ViewModel.OpenLessonCommand.Execute(param);
        }
    }
}