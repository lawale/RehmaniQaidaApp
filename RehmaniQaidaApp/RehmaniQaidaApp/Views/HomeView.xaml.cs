﻿using RehmaniQaidaApp.ViewModels;
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
    public partial class HomeView : BaseView<HomeViewModel>
    {
        public HomeView()
        {
            InitializeComponent();
        }
    }
}