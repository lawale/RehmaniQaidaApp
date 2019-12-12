using RehmaniQaidaApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace RehmaniQaidaApp.Views.Resolver
{
    internal static class ViewResolver
    {
        public static Page GetViewFor<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel, new()
        {
            var viewName = viewModel.GetType().Name.Replace("ViewModel", "View");
            var definedTypes = viewModel.GetType().GetTypeInfo().Assembly.DefinedTypes;
            var type = definedTypes.FirstOrDefault(t => t.Name == viewName);
            return Activator.CreateInstance(type.AsType()) as Page;
        }
    }
}
