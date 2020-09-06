using Akavache;
using Akavache.Sqlite3;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using RehmaniQaidaApp.Extensions;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BlobCacheInstanceHelper))]
namespace RehmaniQaidaApp.Extensions
{
    public class BlobCacheInstanceHelper : IBlobCacheInstanceHelper
    {
        private IFilesystemProvider _filesystemProvider;

        public BlobCacheInstanceHelper()
        {
            
        }

        public async Task Init()
        {
            _filesystemProvider = Locator.Current.GetService<IFilesystemProvider>();
            await GetLocalMachineCache();
        }

        public IBlobCache LocalMachineCache { get; set; }

        private SQLitePersistentBlobCache _Cache;

        private async Task GetLocalMachineCache()
        {
            
            var localCache = new Lazy<Task<IBlobCache>>(async () =>
            {
                var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //var root = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "FilesData");
                var filePath = Path.Combine(root, "RehmaniQaida.db");
                
                if (!File.Exists(filePath))
                {
                    try
                    {
                        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                        if(status != PermissionStatus.Granted)
                        {
                            if(await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                            {
                                Console.WriteLine("Show Permssion Rationale");
                            }
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                            if(results.ContainsKey(Permission.Storage))
                            {
                                status = results[Permission.Storage];
                            }
                        }
                        if(status == PermissionStatus.Granted)
                        {
                            using (var file = File.Create(filePath))
                            using (var dbStream = await Xamarin.Essentials.FileSystem.OpenAppPackageFileAsync("RehmaniQaida.db"))
                            {
                                //dbStream.Seek(0, SeekOrigin.Begin);
                                await dbStream.CopyToAsync(file);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                _filesystemProvider.CreateRecursive(root).SubscribeOn(BlobCache.TaskpoolScheduler).Wait();
                return new SQLitePersistentBlobCache(filePath, BlobCache.TaskpoolScheduler);
            });

            LocalMachineCache = await localCache.Value;
                
            //_filesystemProvider.CreateRecursive(Xamarin.Essentials.FileSystem.AppDataDirectory).SubscribeOn(BlobCache.TaskpoolScheduler).Wait();
            //var cache = new SQLitePersistentBlobCache(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "RehmaniQaida.db"), BlobCache.TaskpoolScheduler);
            //LocalMachineCache = cache;
            //_Cache = cache;
        }

        public void CopyDb()
        {
            var root = Xamarin.Essentials.FileSystem.CacheDirectory + "/FilesData";
            var info = Directory.CreateDirectory(root);
            
            //var root = Android.App.Application.Context.GetExternalFilesDir("DirectoryPictures").ToString();
            //File.Copy(_Cache.Connection.DatabasePath, Path.Combine(Android.OS.Environment.DirectoryDownloads, "RehmaniQaida.db"));
            //File.Copy(_Cache.Connection.DatabasePath, Path.Combine(root, "RehmaniQaida.db"));
        }
    }
}