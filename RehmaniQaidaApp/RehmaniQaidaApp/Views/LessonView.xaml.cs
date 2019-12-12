using RehmaniQaidaApp.ViewModels;
using RehmaniQaidaApp.Views.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;

namespace RehmaniQaidaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LessonView : Xamarin.Forms.TabbedPage
    {
        public LessonView()
        {
            InitializeComponent();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}