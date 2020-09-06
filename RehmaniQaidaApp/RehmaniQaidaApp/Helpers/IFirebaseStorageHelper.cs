using System.Threading.Tasks;

namespace RehmaniQaidaApp.Helpers
{
    public interface IFirebaseStorageHelper
    {
        Task<string> DownloadLessonAudioAsync(string lesson, int index, string audioType);

        Task<string> DownloadLessonReadingAsync(string lesson, int itemNumber);

        Task<string> DownloadLessonPageAsync(string lesson, int itemNumber);
    }
}