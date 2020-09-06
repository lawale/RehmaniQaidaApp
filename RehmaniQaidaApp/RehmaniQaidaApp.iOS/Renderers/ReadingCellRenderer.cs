using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using RehmaniQaidaApp.Controls;
using RehmaniQaidaApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ReadingCell), typeof(ReadingCellRenderer))]
namespace RehmaniQaidaApp.iOS.Renderers
{
    public class ReadingCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            var readingCell = item as ReadingCell;
            cell.SelectedBackgroundView = new UIView { BackgroundColor = readingCell.SelectedItemColor.ToUIColor() };

            return cell;
        }
    }
}