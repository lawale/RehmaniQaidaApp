using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RehmaniQaidaApp.Controls
{
    public class ReadingCell : ViewCell
    {
        public readonly static BindableProperty SelectedItemColorProperty =
            BindableProperty.Create(nameof(SelectedItemColor), typeof(Color), typeof(ReadingCell), Color.Default);

        public Color SelectedItemColor
        {
            get => (Color)GetValue(SelectedItemColorProperty);
            set => SetValue(SelectedItemColorProperty, value);
        }
    }
}
