

using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public interface IUnencryptionStarting : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        string Password { get; set; }
    }
}
