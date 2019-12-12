using System;
using System.Collections.Generic;
using System.Text;

namespace RehmaniQaidaApp.Views.Abstractions
{
    public interface IView<TViewmodel>
    {
        TViewmodel ViewModel { get; set; }
    }
}
