using Akavache;
using RehmaniQaidaApp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RehmaniQaidaApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        {
            //Cache = BlobCache.LocalMachine;
            Cache = DependencyService.Get<IBlobCacheInstanceHelper>().LocalMachineCache;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IBlobCache Cache = null;

        protected virtual void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null, Action action = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;
            backingField = value;
            OnViewModelPropertyChanged(propertyName);
            action?.Invoke();
        }

        protected void OnViewModelPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; }

        public Func<BaseViewModel, bool, Task> OnNavigationRequest { get; set; }

        public Func<bool,Task> OnPopRequest { get; set; }

        public Func<BaseViewModel, Task> OnReplaceIndexRequest { get; set; }

        public async Task NavigateTo<TViewmodel>(TViewmodel viewmodel, bool isMasterDetailNavigation = false) where TViewmodel:BaseViewModel,new()
        {
            await OnNavigationRequest?.Invoke(viewmodel, isMasterDetailNavigation);
        }

        public virtual async Task OnViewAppeared()
        {
            await Task.Delay(0);
        }

        public virtual async Task OnViewDisappeared()
        {
            await Task.Delay(0);
        }

        public async Task ReplaceIndex(BaseViewModel viewModel)
        {
            await OnReplaceIndexRequest?.Invoke(viewModel);
        }

        public async Task Pop(bool toRoot = false)
        {
            await OnPopRequest?.Invoke(toRoot);
        }
    }
}
