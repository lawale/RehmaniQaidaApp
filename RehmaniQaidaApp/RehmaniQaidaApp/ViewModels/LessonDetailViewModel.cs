using Akavache;
using RehmaniQaidaApp.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace RehmaniQaidaApp.ViewModels
{
    public class LessonDetailViewModel : BaseViewModel
    {
        private ObservableCollection<ImageSource> pages;
        public ObservableCollection<ImageSource> Pages
        {
            get => pages;
            set => SetValue(ref pages, value);
        }

        private int count;

        public int Count
        {
            get => count;
            set => SetValue(ref count, value);
        }

        public int Counter => count - position;

        private int position;

        public int Position
        {
            get => position;
            set
            {
                SetValue(ref position, value);
                OnViewModelPropertyChanged(nameof(Counter));
            }
        }

        public ICommand Next { get; }

        public ICommand Previous { get; set; }


        public ICommand GetLessonResourcesCommand { get; }

        private readonly IFirebaseStorageHelper firebaseStorage;

        public LessonDetailViewModel()
        {
            
            firebaseStorage = DependencyService.Get<IFirebaseStorageHelper>();
            
            GetLessonResourcesCommand = new Command(async () => await ExecuteGetLessonResourcesCommand());
            Next = new Command(async () => await ExecuteNext());
            Previous = new Command(async () => await ExecutePrevious());
        }

        public override Task OnViewAppeared()
        {
            var key = Title.Replace(" ", string.Empty);
            Count = (int)typeof(LessonPageCount).GetField(key).GetValue(null);
            Position = count - 1;
            return base.OnViewAppeared();
        }

        private async Task ExecutePrevious()
        {
            if(position < (count - 1))
            {
                Position += 1;
                return;
            }
            var response = await MaterialDialog.Instance.ConfirmAsync("Do you want to go to the previous lesson?", confirmingText: "Yes", dismissiveText: "No");
            if (response.HasValue && response.Value)
            {
                var lesson_number = int.Parse(Title.Replace("Lesson ", string.Empty));
                if (lesson_number > 1)
                    --lesson_number;
                else
                    return;

                var lessonViewModel = new LessonViewModel
                {
                    Title = $"Lesson {lesson_number}"
                };
                await ReplaceIndex(lessonViewModel);
            }
        }

        private async Task ExecuteNext()
        {
            if(position > 0)
            {
                Position -= 1;
                return;
            }
            var response = await MaterialDialog.Instance.ConfirmAsync("Do you want to go to the next lesson?", confirmingText: "Yes", dismissiveText: "No");
            if (response.HasValue && response.Value)
            {
                var lesson_number = int.Parse(Title.Replace("Lesson ", string.Empty));
                if (lesson_number < 20)
                    ++lesson_number;
                else
                    return;

                var lessonViewModel = new LessonViewModel
                {
                    Title = $"Lesson {lesson_number}"
                };
                await ReplaceIndex(lessonViewModel);
            }
        }

        private async Task ExecuteGetLessonResourcesCommand()
        {
            var key = $"{Title} Page";
            using (await MaterialDialog.Instance.LoadingDialogAsync($"Retreiving {Title} Resources"))
            {
                try
                {
                    var list = await Cache.GetObject<IEnumerable<byte[]>>(key);
                    if (list.Count() < 1)
                        throw new KeyNotFoundException();
                    Pages = new ObservableCollection<ImageSource>(list.Select(b => ImageSource.FromStream(() => new MemoryStream(b))).Reverse());
                }
                catch (KeyNotFoundException)
                {
                    var list = await DownloadAllPageImageFiles();
                    if (list == null)
                        await Exit();
                    await Cache.InsertObject(key, list);
                    Pages = new ObservableCollection<ImageSource>(list.Select(b => ImageSource.FromStream(() => new MemoryStream(b))).Reverse());
                }
                catch (Exception ex)
                {
                    await Exit();
                }
            }
        }

        private async Task Exit()
        {
            await MaterialDialog.Instance.SnackbarAsync($"An Error has occured while retrieving {Title} reading resources", MaterialSnackbar.DurationShort);
            await Pop();
        }

        private async Task<IEnumerable<byte[]>> DownloadAllPageImageFiles()
        {
            var list = new List<byte[]>();
            try
            {
                var key = Title.Replace(" ", string.Empty);
                for (int index = 1; index <= count; ++index)
                {
                    var file = await DownloadPageImageFile(index);
                    list.Add(file);
                }
            }
            catch
            {
                Exit();
            }
            return list;
        }

        

        private async Task<byte[]> DownloadPageImageFile(int index)
        {
            var link = await firebaseStorage.DownloadLessonPageAsync(Title, index);
            RestClient restClient = new RestClient(link);
            var restRequest = new RestRequest(Method.GET);
            var response = await restClient.ExecuteTaskAsync(restRequest);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Unable to download file");
            var file = response.RawBytes;
            return file;
            
        }

    }
}
