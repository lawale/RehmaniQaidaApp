using Akavache;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RehmaniQaidaApp.ViewModels
{
    public class LessonsViewModel : BaseViewModel
    {
        public ICommand OpenLessonCommand { get; }

        public ObservableCollection<string> Lessons { get; }

        public LessonsViewModel()
        {
            Lessons = new ObservableCollection<string>();
            AddLessons();
            OpenLessonCommand = new Command<string>(async param => await ExecuteOpenLessonCommand(param));
        }        

        private void AddLessons()
        {
            for (var i = 1; i < 21; ++i)
            {
                Lessons.Add($"Lesson {i}");
            }
        }

        private async Task ExecuteOpenLessonCommand(string param)
        {
            
            var lessonViewModel = new LessonViewModel
            {
                Title = param
            };
            await NavigateTo(lessonViewModel);
        }
    }
}
