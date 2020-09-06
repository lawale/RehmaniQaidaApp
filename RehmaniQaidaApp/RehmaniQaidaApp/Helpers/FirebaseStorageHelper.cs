using Firebase.Storage;
using RehmaniQaidaApp.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using System.Threading.Tasks;

[assembly: Dependency(typeof(FirebaseStorageHelper))]
namespace RehmaniQaidaApp.Helpers
{
    public class FirebaseStorageHelper : IFirebaseStorageHelper
    {
        readonly FirebaseStorage firebaseStorage = new FirebaseStorage("rehmaniqaidaapp.appspot.com");
        public async Task<string> DownloadLessonPageAsync(string lesson, int itemNumber)
        {
            var file = lesson.ToLower().Replace(" ", string.Empty);
            return await firebaseStorage
                .Child(lesson)
                .Child("Page")
                .Child($"{file}_{itemNumber}.jpg")
                .GetDownloadUrlAsync();
        }

        public async Task<string> DownloadLessonAudioAsync(string lesson, int index, string audioType)
        {
            var link = await firebaseStorage
                .Child(lesson)
                .Child("Audio")
                .Child(audioType)
                .Child($"sound {index}.mp3")
                .GetDownloadUrlAsync();
            return link;
        }

        public async Task<string> DownloadLessonReadingAsync(string lesson, int index)
        {
            string itemNumber;
            string file;
            var lessonNumber = int.Parse(lesson.Replace("Lesson ", string.Empty));
            if(lessonNumber == 1)
            {
                //file = lesson.Replace("Lesson ", "w");
                file = "w";
                return await firebaseStorage
                    .Child(lesson)
                    .Child("Reading")
                    .Child($"{file}{index}.png")
                    .GetDownloadUrlAsync();
            }

            file = lesson.Replace("Lesson ", "c");
            if (index < 10)
                itemNumber = $"0{index}";
            else
                itemNumber = $"{index}";
            return await firebaseStorage
                .Child(lesson)
                .Child("Reading")
                .Child($"{file}_{itemNumber}.png")
                .GetDownloadUrlAsync();
        }
    }
}
