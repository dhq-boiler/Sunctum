
using Sunctum.Domain.Models.Managers;

namespace Sunctum.Core.Notifications
{
    public class ActiveTabChanged
    {
        public IBookStorage BookStorage { get; set; }

        public ActiveTabChanged(IBookStorage bookStorage)
        {
            BookStorage = bookStorage;
        }
    }
}
