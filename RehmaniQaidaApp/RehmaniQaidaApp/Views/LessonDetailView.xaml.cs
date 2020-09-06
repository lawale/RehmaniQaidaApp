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
    public partial class LessonDetailView : BaseView<LessonDetailViewModel>
    {
        public LessonDetailView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel.Title = (Parent as TabbedPage).Title;
            if(ViewModel.Pages == null)
                ViewModel.GetLessonResourcesCommand.Execute(null);
            base.OnAppearing();
        }

        
        private void Previous(object sender, EventArgs e) => ViewModel.Previous.Execute(null);

        private void Next(object sender, EventArgs e) => ViewModel.Next.Execute(null);

        private void GoToReading(object sender, EventArgs e)
        {
            var parent = Parent as TabbedPage;
            parent.CurrentPage = parent.Children[1];
        }
    }
}