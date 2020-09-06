using Akavache;
using RehmaniQaidaApp.Helpers;
using RehmaniQaidaApp.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using RehmaniQaidaApp.Extensions;
using Plugin.LocalNotification;
using Plugin.SimpleAudioPlayer;
using System.Threading;
using Plugin.DeviceInfo;
using RehmaniQaidaApp.Views;
using Rg.Plugins.Popup.Services;
using System.Diagnostics;

namespace RehmaniQaidaApp.ViewModels
{
    public class LessonReadingViewModel : BaseViewModel
    {
        #region Properties and backing fields
        private ObservableCollection<ImageSource> readings;

        public ObservableCollection<ImageSource> Readings
        {
            get => readings;
            set
            {
                SetValue(ref readings, value);
                OnViewModelPropertyChanged(nameof(SelectedItem));
                OnViewModelPropertyChanged(nameof(ReversedReadings));
            }
        }

        public ObservableCollection<ImageSource> ReversedReadings => readings == null ? null : new ObservableCollection<ImageSource>(readings.Reverse());

        private int currentDownloadIndex;

        public int CurrentDownloadIndex
        {
            get => currentDownloadIndex;

            set
            {
                SetValue(ref currentDownloadIndex, value);
                OnViewModelPropertyChanged(nameof(Progress));
            }
        }

        private int totalDownloads;
        public int TotalDownloads => Count * 3;

        public float Progress => (float) CurrentDownloadIndex / TotalDownloads;

        private bool isPlaying;

        public bool IsPlaying
        {

            get => isPlaying;

            set => SetValue(ref isPlaying, value);

        }


        private int count;

        public int Count
        {
            get => count;
            set => SetValue(ref count, value);
        }

        private int position;

        public int Position
        {
            get => position;
            set
            {
                SetValue(ref position, value);
                OnViewModelPropertyChanged(nameof(SelectedItem));
            }
        }

        private int reversedPosition;

        public int ReversedPosition
        {
            get => reversedPosition;
            set => SetValue(ref reversedPosition, value);
        }

        public ImageSource SelectedItem => Readings?[Position];

        private ObservableCollection<Stream> audios;

        public ObservableCollection<Stream> Audios
        {
            get => audios;
            set => SetValue(ref audios, value);
        }


        private List<ISimpleAudioPlayer> audioPlayers = null;

        public List<ISimpleAudioPlayer> AudioPlayers
        {
            set => SetValue(ref audioPlayers, value);

            get
            {
                if (audioPlayers == null)
                {
                    audioPlayers = Audios?.Select(stream =>
                    {
                        var player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                        stream.Position = 0;
                        player.Load(stream);
                        return player;
                    }).ToList();
                }
                return audioPlayers;
            }
        }


        #endregion



        #region private fields
        private readonly IFirebaseStorageHelper firebaseStorage;


        private CancellationTokenSource PlayTaskCancelToken;

        private readonly string[] audioModeChoices = new string[]
        {
            "Auto (Default)",
            "By word"
        };

        private readonly string[] audioTypeChoices = new string[]
        {
            "Hijjay",
            "Rawan",
            "Tarteel"
        };

        #endregion



        #region Commands
        public ICommand GetLessonResourcesCommand { get; }

        public ICommand DownloadLessonAudioCommmand { get; }

        public ICommand Next { get; }

        public ICommand Previous { get; }

        public ICommand AudioModeCommand { get; }

        public ICommand AudioTypeCommand { get; }

        public ICommand PlayAudioFiles { get; }

        public ICommand ItemSelectedCommand { get; }

        #endregion



        #region Constructor
        public LessonReadingViewModel()
        {
            firebaseStorage = DependencyService.Get<IFirebaseStorageHelper>();
            GetLessonResourcesCommand = new Command(async () => await ExecuteGetLessonResources());
            AudioModeCommand = new Command(async () => await ExecuteAudioModeCommand());
            AudioTypeCommand = new Command(async () => await ExecuteAudioTypeCommand());
            DownloadLessonAudioCommmand = new Command(async () => await ExecuteDownloadLessonAudioCommand());
            ItemSelectedCommand = new Command<ImageSource>(async item => await ExecuteItemSelected(item));
            PlayAudioFiles = new Command(async manualClick => await PlayAudioFilesAsync());
            Next = new Command(ExecuteNext);
            Previous = new Command(ExecutePrevious);
        }
        #endregion



        #region Base Method Override
        public override Task OnViewAppeared()
        {
            var key = Title.Replace(" ", string.Empty);
            Count = (int)typeof(LessonReadingCount).GetField(key).GetValue(null);
            Position = 0;
            ReversedPosition = Count - 1;
            return base.OnViewAppeared();
        }

        public override Task OnViewDisappeared()
        {
            if(audioPlayers != null)
            {
                foreach (var player in AudioPlayers)
                {
                    player.Stop();
                    player.Dispose();
                    Audios = null;
                }
            }
            return base.OnViewDisappeared();
        }
        #endregion



        #region Command Method Action
        private async Task ExecuteItemSelected(ImageSource selectedItem)
        {
            if (selectedItem == null)
                return;

            if (!isPlaying)
            {
                Position = Readings.IndexOf(selectedItem);
                ReversedPosition = count - position - 1;
                return;
            }

            PlayTaskCancelToken.Cancel();
            //CancelTask();

            //audioPlayers?.ForEach(player => player?.Stop());
            IsPlaying = false;
            Position = Readings.IndexOf(selectedItem);
            ReversedPosition = count - position - 1;
            await PlayAudioFilesAsync();
        }

        private async Task ExecuteAudioTypeCommand()
        {
            var canPlay = isPlaying;

            CancelTask();

            try
            {

                var cache = await Cache.GetObject<int>("Audio Type");
                if (cache == -1)
                    cache = 1;
                var choice = await MaterialDialog.Instance.SelectChoiceAsync("Select an Audio Type", audioTypeChoices, cache);
                if (cache == -1)
                    cache = 1;
                await Cache.InsertObject("Audio Type", choice);
            }
            catch (KeyNotFoundException)
            {
                var choice = await MaterialDialog.Instance.SelectChoiceAsync("Select an Audio Type", audioTypeChoices, (int)AudioTypeOption.Hijjay);
                if (choice == -1)
                {
                    await Cache.InsertObject("Audio Type", (int)AudioTypeOption.Hijjay);
                    return;
                }
                await Cache.InsertObject("Audio Type", choice);
            }
            Audios = null;
            if(canPlay)
            {
                //var response = await ExecuteDownloadLessonAudioCommand();
                //if (response)
                    await PlayAudioFilesAsync();
            }
        }

        private async Task<bool> ExecuteDownloadLessonAudioCommand()
        {
            var key = $"{Title} Reading";

            AudioTypeOption _audioType;
            try
            {
                _audioType = await Cache.GetObject<AudioTypeOption>("Audio Type");
                if ((int)_audioType == -1)
                    throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                await Cache.InsertObject("Audio Type", (int)AudioTypeOption.Hijjay);
                _audioType = AudioTypeOption.Hijjay;
            }
            try
            {
                using (await MaterialDialog.Instance.LoadingDialogAsync($"Retreiving {Title} Resources"))
                {
                    var audioValue = Enum.GetName(typeof(AudioTypeOption), _audioType);
                    var audioFilelist = await Cache.GetObject<IEnumerable<byte[]>>($"{key} audioFile {audioValue}");
                    if (audioFilelist == null || audioFilelist.Count() < 1)
                        throw new KeyNotFoundException();
                    Audios = new ObservableCollection<Stream>(audioFilelist.Select(b => new MemoryStream(b)));
                    return true;
                }

            }
            catch (KeyNotFoundException)
            {
                var userResponse = await MaterialDialog.Instance.ConfirmAsync($"{Title} Audio resources have not been downloaded. Do you want to download these files?", $"{Title} Audio", "Download", "No");
                if (userResponse.HasValue && !userResponse.Value)
                    return false;

                var downloadedAudios = await DownloadAllAudioFiles();

                if(downloadedAudios is null)
                {
                    DisplayNotification($"{Title} Audio lesson files download was unsuccessful");
                    return false;
                }

                await Cache.InsertObject($"{key} audioFile {audioTypeChoices[0]}", downloadedAudios[0]);
                await Cache.InsertObject($"{key} audioFile {audioTypeChoices[1]}", downloadedAudios[1]);
                await Cache.InsertObject($"{key} audioFile {audioTypeChoices[2]}", downloadedAudios[2]);

                DisplayNotification($"{Title} Audio lesson files have successfully been downloaded.");
                return false;
            }
            catch (Exception ex)
            {
                DisplayNotification($"{Title} Audio lesson files retrieval was unsuccessful.");
                return false;
            }
        }

        private async Task ExecuteAudioModeCommand()
        {
            var canPlay = isPlaying;

            CancelTask();
            
            try
            {
                var cache = await Cache.GetObject<int>("Audio Mode");
                if (cache == -1)
                    cache = 1;
                var choice = await MaterialDialog.Instance.SelectChoiceAsync("Select an Audio Mode", audioModeChoices, cache);
                if (cache == -1)
                    cache = 1;
                await Cache.InsertObject("Audio Mode", choice);
            }
            catch (KeyNotFoundException)
            {
                var choice = await MaterialDialog.Instance.SelectChoiceAsync("Select an Audio Mode", audioModeChoices, (int)AudioModeOption.Auto);
                if (choice == -1)
                {
                    await Cache.InsertObject("Audio Mode", (int)AudioModeOption.Auto);
                    return;
                }
                await Cache.InsertObject("Audio Mode", choice);
            }

            if (canPlay)
                await PlayAudioFilesAsync();
        }

        private async Task PlayAudioFilesAsync()
        {
            if (Audios == null || Audios.Count < 1)
            {
                var isCachedResult = await ExecuteDownloadLessonAudioCommand();
                /**
                 * Exit Method if not cached result beacuse audio files have
                 * just been downloaded and use might have left this lesson page
                 **/

                if (!isCachedResult)
                    return;
            }
            AudioModeOption audioMode;
            try
            {
                audioMode = await Cache.GetObject<AudioModeOption>("Audio Mode");
                if ((int)audioMode == -1)
                    throw new KeyNotFoundException();
            }
            catch(KeyNotFoundException)
            {
                await Cache.InsertObject("Audio Mode", (int)AudioModeOption.Auto);
                audioMode = AudioModeOption.Auto;
            }
            
            if (IsPlaying)
            {
                PlayTaskCancelToken?.Cancel();
                //CancelTask();
                //AudioPlayers.Reset(Audios);
                //AudioPlayers?.ForEach(player => player?.Stop());
                IsPlaying = false;
                return;
            }
            
            TimeSpan time;

            CancelTask();
            
            using(PlayTaskCancelToken = new CancellationTokenSource())
            {
                try
                {
                    switch (audioMode)
                    {
                        case AudioModeOption.Auto:
                            //AudioPlayers.Reset(Audios);
                            while (Position <= Count - 1 && !PlayTaskCancelToken.IsCancellationRequested)
                            {
                                AudioPlayers[Position].Play();
                                IsPlaying = true;
                                time = TimeSpan.FromSeconds(AudioPlayers[Position].Duration);
                                await Task.Delay(time, PlayTaskCancelToken.Token);
                                AudioPlayers[Position].Reset(Audios[Position]);
                                if (ReversedPosition == 0)
                                    break;
                                ++Position;
                                --ReversedPosition;
                            }
                            break;
                        case AudioModeOption.ByWord:
                            //AudioPlayers.Reset(Audios);
                            time = TimeSpan.FromSeconds(AudioPlayers[Position].Duration);
                            AudioPlayers[Position].Play();
                            IsPlaying = true;
                            await Task.Delay(time, PlayTaskCancelToken.Token);
                            
                            AudioPlayers[Position].Reset(Audios[Position]);
                            break;
                    }
                    AudioPlayers = null;
                }
                catch(TaskCanceledException)
                {
                    CancelTask();
                }
            }
            //AudioPlayers.Reset(Audios);
            IsPlaying = false;
        }

        private void ExecuteNext()
        {
            if (reversedPosition > 0)
            {
                ++Position;
                --ReversedPosition;
                return;
            }
        }

        private void ExecutePrevious()
        {
            if (reversedPosition < (count - 1))
            {
                --Position;
                ++ReversedPosition;
                return;
            }
        }

        private async Task ExecuteGetLessonResources()
        {
            var key = $"{Title} Reading";
            using (await MaterialDialog.Instance.LoadingDialogAsync($"Retreiving {Title} Resources"))
            {
                try
                {
                    var audioImagelist = await Cache.GetObject<IEnumerable<byte[]>>($"{key} audioImage");

                    Readings = new ObservableCollection<ImageSource>(audioImagelist.Select(b => ImageSource.FromStream(() => new MemoryStream(b))));
                    
                }
                catch (KeyNotFoundException)
                {
                    var audioImagelist = await DownloadAllAudioImageFiles();

                    if (audioImagelist == null)
                        await Exit();

                    await Cache.InsertObject($"{key} audioImage", audioImagelist);

                    Readings = new ObservableCollection<ImageSource>(audioImagelist.Select(b => ImageSource.FromStream(() => new MemoryStream(b))));
                }
                catch (Exception ex)
                {
                    await Exit();
                }
            }
        }
        #endregion

        //private void ProgressNotification(int totalDownloaded, string message)
        //{
        //    var notifications = new NotificationRequest
        //    {
        //        NotificationId = 100,
        //        BadgeNumber = 10,
        //        Title = "sample title",
        //        Description = "sample description",
        //        ReturningData = "sample data",
        //        NotifyTime = DateTime.Now.AddSeconds(1),
        //        Android = new AndroidOptions
        //        {

        //        IconName = "download.jpg",
        //        ChannelId = CrossDeviceInfo.Current.Version.ToString(),
        //        Priority = NotificationPriority.Max,
        //        AutoCancel = false,
        //        TimeoutAfter = TimeSpan.FromSeconds(5),
        //        Ongoing = true,
        //        Color = 255,
        //        LedColor = 255
        //        }
        //    };
        //    NotificationCenter.Current.Show(notifications);
        //}

        #region private helper methods
        private void DisplayNotification(string message)
        {
            var notification = new NotificationRequest()
            {
                NotificationId = int.Parse(Title.Replace("Lesson ", string.Empty)),
                Title = "Rehmaini Qaida",
                Description = message,
                NotifyTime = DateTime.Now.AddSeconds(1),
                
            };
            NotificationCenter.Current.Show(notification);
        }


        

        private void CancelTask()
        {
            try
            {
                PlayTaskCancelToken?.Cancel();
                audioPlayers?.ForEach(player => player?.Stop());
                
                AudioPlayers = null;
            }
            catch (ObjectDisposedException) { }
        }

        private async Task Exit()
        {
            await MaterialDialog.Instance.SnackbarAsync($"An Error has occured while retrieving {Title} reading resources", MaterialSnackbar.DurationShort);
            await Pop();
        }
        #endregion



        #region Files Download Methods
        private async Task<byte[]> DownloadAudioImageFile(int index)
        {
            var key = Title.Replace("Lesson ", "c");
            var link = await firebaseStorage.DownloadLessonReadingAsync(Title, index);
            RestClient restClient = new RestClient(link);
            var restRequest = new RestRequest(Method.GET);
            var response = await restClient.ExecuteTaskAsync(restRequest);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Unable to download file");
            var file = response.RawBytes;
            return file;
        }

        private async Task<IEnumerable<byte[]>> DownloadAllAudioImageFiles()
        {
            try
            {
                var list = new List<byte[]>();
                var key = Title.Replace(" ", string.Empty);
                var count = (int)typeof(LessonReadingCount).GetField(key).GetValue(null);
                
                for (int index = 1; index <= count; ++index)
                {
                    
                    list.Add(await DownloadAudioImageFile(index));
                }
                
                return list;
            }
            catch
            {
                return null;
            }
            
        }

        private async Task<IEnumerable<byte[]>[]> DownloadAllAudioFiles()
        {
            try
            {
                
                //await MaterialDialog.Instance.SnackbarAsync($"Downloading {Title} Audio Files", "Ok");
                
                var key = Title.Replace(" ", string.Empty);
                var count = (int)typeof(LessonReadingCount).GetField(key).GetValue(null);
                var download = new DownloadProgress
                {
                    BindingContext = this
                };
                await PopupNavigation.Instance.PushAsync(download);
                CurrentDownloadIndex = 0;
                IEnumerable<byte[]>[] downloadedAudios = new IEnumerable<byte[]>[3];
                for (var i = 0; i < 3; ++i)
                {
                    var audioType = audioTypeChoices[i];
                    var list = new List<byte[]>();
                    for (var index = 1; index <= count; ++index)
                    {
                        //ProgressNotification(index - 1, "Downloading Audio Files");
                        list.Add(await DownloadAudioFile(index, audioType));
                        ++CurrentDownloadIndex;
                    }
                    downloadedAudios[i] = list;
                }
                await PopupNavigation.Instance.PopAsync();
                return downloadedAudios;
            }
            catch(Exception e)
            {
#if DEBUG
                Debug.WriteLine(e);
#endif
                await PopupNavigation.Instance.PopAsync();
                return null;
            }

        }

        private async Task<byte[]> DownloadAudioFile(int index, string audioType)
        {
            var link = await firebaseStorage.DownloadLessonAudioAsync(Title, index, audioType);
            RestClient restClient = new RestClient(link);
            var restRequest = new RestRequest(Method.GET);
            var response = await restClient.ExecuteTaskAsync(restRequest);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Unable to download file");
            var file = response.RawBytes;
            return file;
        }
        #endregion

    }
}
