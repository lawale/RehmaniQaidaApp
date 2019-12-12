using System;
using System.Collections.Generic;
using System.Text;

namespace RehmaniQaidaApp.ViewModels
{
    public class LessonColorChartViewModel : BaseViewModel
    {
        public List<string> Colors { get; }

        public LessonColorChartViewModel()
        {
            Colors = new List<string>();
            AddColors();
        }

        private void AddColors()
        {
            for (var i = 1; i < 9; ++i)
                Colors.Add($"color0{i}");
        }
    }
}
