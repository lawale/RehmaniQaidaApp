using RehmaniQaidaApp.Options;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RehmaniQaidaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuCell : ViewCell
    {
        public MenuCell()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty IconProperty =
             BindableProperty.Create(nameof(Icon), typeof(string), typeof(MenuCell), null);

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(MenuCell), string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty ShowBottomDividerProperty =
            BindableProperty.Create(nameof(ShowBottomDivider), typeof(bool), typeof(MenuCell), true);

        public bool ShowBottomDivider
        {
            get => (bool)GetValue(ShowBottomDividerProperty);
            set => SetValue(ShowBottomDividerProperty, value);
        }

        public static readonly BindableProperty ShowContentProperty =
            BindableProperty.Create(nameof(ShowContent), typeof(bool), typeof(MenuCell), true);

        public bool ShowContent
        {
            get => (bool)GetValue(ShowContentProperty);
            set => SetValue(ShowContentProperty, value);
        }

        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(MenuCell), propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (MenuCell)bindable;
                control.Tapped += OnMenuTapped;
            });

        private static void OnMenuTapped(object sender, EventArgs e)
        {
            var menu = sender as MenuCell;
            Execute(menu.TappedCommand, menu.TappedCommandParameter);
        }

        private static void Execute(ICommand command, object commandParameter = null)
        {
            if (command == null) return;
            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        public ICommand TappedCommand
        {
            get => (ICommand)GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }
        
        public static readonly BindableProperty TappedCommandParameterProperty =
            BindableProperty.Create(nameof(TappedCommandParameter), typeof(MenuOption), typeof(MenuCell), MenuOption.Default);

        public MenuOption TappedCommandParameter
        {
            get => (MenuOption)GetValue(TappedCommandParameterProperty);
            set => SetValue(TappedCommandParameterProperty, value);
        }

    }
}