using Akavache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RehmaniQaidaApp.Extensions
{
    public interface IBlobCacheInstanceHelper
    {
        Task Init();

        IBlobCache LocalMachineCache { get; set; }

        void CopyDb();
    }
}
