

using Sunctum.Domain.Extensions;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Models.Managers
{
    public class BookCollectionChanged
    {
        public Change TargetChange { get; set; }

        public BookViewModel Target { get; set; }

        public abstract class Change
        {
            public abstract void ApplyChange(ICollection<BookViewModel> collection, BookViewModel target);
        }

        public class Add : Change
        {
            public override void ApplyChange(ICollection<BookViewModel> collection, BookViewModel target)
            {
                collection.Add(target);
            }
        }

        public class Update : Change
        {
            public override void ApplyChange(ICollection<BookViewModel> collection, BookViewModel target)
            {
                var book = collection.FirstOrDefault(x => x.ID.Equals(target.ID));
                target.CopyTo(book);
            }
        }

        public class Remove : Change
        {
            public override void ApplyChange(ICollection<BookViewModel> collection, BookViewModel target)
            {
                var book = collection.FirstOrDefault(x => x.ID.Equals(target.ID));
                if (book != null)
                {
                    collection.Remove(book);
                }
            }
        }
    }
}
