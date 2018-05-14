
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Bridge
{
    public static class Extentions
    {
        public static Book ToEntity(this BookViewModel obj)
        {
            return new Book()
            {
                AuthorID = obj.AuthorID,
                ByteSize = obj.ByteSize,
                ID = obj.ID,
                PublishDate = obj.PublishDate,
                Title = obj.Title
            };
        }

        public static Page ToEntity(this PageViewModel obj)
        {
            return new Page()
            {
                BookID = obj.BookID,
                ID = obj.ID,
                ImageID = obj.ImageID,
                PageIndex = obj.PageIndex,
                Title = obj.Title
            };
        }

        public static Image ToEntity(this ImageViewModel obj)
        {
            return new Image()
            {
                ByteSize = obj.ByteSize,
                ID = obj.ID,
                RelativeMasterPath = obj.RelativeMasterPath,
                Title = obj.Title
            };
        }

        public static Author ToEntity(this AuthorViewModel obj)
        {
            return new Author()
            {
                ID = obj.ID,
                Name = obj.Name
            };
        }

        public static AuthorCount ToEntity(this AuthorCountViewModel obj)
        {
            return new AuthorCount()
            {
                Author = obj.Author?.ToEntity(),
                Count = obj.Count,
                IsSearchingKey = obj.IsSearchingKey
            };
        }

        public static ImageTag ToEntity(this ImageTagViewModel obj)
        {
            return new ImageTag()
            {
                ImageID = obj.ImageID,
                TagID = obj.TagID
            };
        }

        public static Tag ToEntity(this TagViewModel obj)
        {
            return new Tag()
            {
                ID = obj.ID,
                Name = obj.Name
            };
        }

        public static TagCount ToEntity(this TagCountViewModel obj)
        {
            return new TagCount()
            {
                Count = obj.Count,
                Tag = obj.Tag.ToEntity()
            };
        }

        public static Thumbnail ToEntity(this ThumbnailViewModel obj)
        {
            return new Thumbnail()
            {
                ID = obj.ID,
                ImageID = obj.ImageID,
                RelativeMasterPath = obj.RelativeMasterPath
            };
        }

        public static BookTag ToEntity(this BookTagViewModel obj)
        {
            return new BookTag()
            {
                BookID = obj.BookID,
                TagID = obj.TagID,
            };
        }

        public static BookViewModel ToViewModel(this Book obj)
        {
            return new BookViewModel()
            {
                Configuration = Configuration.ApplicationConfiguration,
                AuthorID = obj.AuthorID,
                ByteSize = obj.ByteSize,
                ID = obj.ID,
                PublishDate = obj.PublishDate,
                Title = obj.Title
            };
        }

        public static PageViewModel ToViewModel(this Page obj)
        {
            return new PageViewModel()
            {
                Configuration = Configuration.ApplicationConfiguration,
                BookID = obj.BookID,
                ID = obj.ID,
                ImageID = obj.ImageID,
                PageIndex = obj.PageIndex,
                Title = obj.Title
            };
        }

        public static ImageViewModel ToViewModel(this Image obj)
        {
            return new ImageViewModel()
            {
                Configuration = Configuration.ApplicationConfiguration,
                ByteSize = obj.ByteSize,
                ID = obj.ID,
                RelativeMasterPath = obj.RelativeMasterPath,
                Title = obj.Title
            };
        }

        public static AuthorViewModel ToViewModel(this Author obj)
        {
            return new AuthorViewModel()
            {
                ID = obj.ID,
                Name = obj.Name
            };
        }

        public static AuthorCountViewModel ToViewModel(this AuthorCount obj)
        {
            return new AuthorCountViewModel()
            {
                Author = obj.Author.ToViewModel(),
                Count = obj.Count,
                IsSearchingKey = obj.IsSearchingKey
            };
        }

        public static ImageTagViewModel ToViewModel(this ImageTag obj)
        {
            return new ImageTagViewModel()
            {
                ImageID = obj.ImageID,
                TagID = obj.TagID
            };
        }

        public static TagViewModel ToViewModel(this Tag obj)
        {
            return new TagViewModel()
            {
                ID = obj.ID,
                Name = obj.Name
            };
        }

        public static TagCountViewModel ToViewModel(this TagCount obj)
        {
            return new TagCountViewModel()
            {
                Count = obj.Count,
                Tag = obj.Tag.ToViewModel()
            };
        }

        public static ThumbnailViewModel ToViewModel(this Thumbnail obj)
        {
            return new ThumbnailViewModel()
            {
                ID = obj.ID,
                ImageID = obj.ImageID,
                RelativeMasterPath = obj.RelativeMasterPath
            };
        }

        public static BookTagViewModel ToViewModel(this BookTag obj)
        {
            return new BookTagViewModel()
            {
                BookID = obj.BookID,
                TagID = obj.TagID
            };
        }

        public static IEnumerable<Book> ToEntity(this IEnumerable<BookViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<Page> ToEntity(this IEnumerable<PageViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<Image> ToEntity(this IEnumerable<ImageViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<Author> ToEntity(this IEnumerable<AuthorViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<AuthorCount> ToEntity(this IEnumerable<AuthorCountViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<ImageTag> ToEntity(this IEnumerable<ImageTagViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<Tag> ToEntity(this IEnumerable<TagViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<TagCount> ToEntity(this IEnumerable<TagCountViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<Thumbnail> ToEntity(this IEnumerable<ThumbnailViewModel> obj)
        {
            return obj.Select(i => i.ToEntity());
        }

        public static IEnumerable<BookViewModel> ToViewModel(this IEnumerable<Book> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<PageViewModel> ToViewModel(this IEnumerable<Page> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<ImageViewModel> ToViewModel(this IEnumerable<Image> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<AuthorViewModel> ToViewModel(this IEnumerable<Author> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<AuthorCountViewModel> ToViewModel(this IEnumerable<AuthorCount> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<ImageTagViewModel> ToViewModel(this IEnumerable<ImageTag> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<TagViewModel> ToViewModel(this IEnumerable<Tag> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<TagCountViewModel> ToViewModel(this IEnumerable<TagCount> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }

        public static IEnumerable<ThumbnailViewModel> ToViewModel(this IEnumerable<Thumbnail> obj)
        {
            return obj.Select(i => i.ToViewModel());
        }
    }
}