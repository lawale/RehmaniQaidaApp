using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RehmaniQaidaApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; }

        public Func<BaseViewModel, bool, Task> OnNavigationRequest { get; set; }

        public async Task NavigateTo<TViewmodel>(TViewmodel viewmodel, bool isMasterDetailNavigation = false) where TViewmodel:BaseViewModel,new()
        {
            await OnNavigationRequest?.Invoke(viewmodel, isMasterDetailNavigation);
        }
    }
}
