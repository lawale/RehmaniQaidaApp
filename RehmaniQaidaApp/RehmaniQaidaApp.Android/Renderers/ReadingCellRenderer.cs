using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RehmaniQaidaApp.Controls;
using RehmaniQaidaApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(ReadingCell), typeof(ReadingCellRenderer))]
namespace RehmaniQaidaApp.Droid.Renderers
{
    public class ReadingCellRenderer : ViewCellRenderer
    {
        private Android.Views.View _cellCore;
        private bool _selected = false;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _cellCore = base.GetCellCore(item, convertView, parent, context);
            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);
            if (args.PropertyName == "IsSelected")
            {
                _selected = !_selected;
                
                var cell = sender as ReadingCell;
                if (_selected)
                    _cellCore.SetBackgroundColor(cell.SelectedItemColor.ToAndroid());
                else
                    _cellCore.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}