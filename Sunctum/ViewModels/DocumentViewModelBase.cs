
using Prism.Interactivity.InteractionRequest;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Sunctum.ViewModels
{
    public abstract class DocumentViewModelBase : DockElementViewModelBase
    {
        private IArrangedBookStorage _Cabinet;
        private Dictionary<Guid, Point> _scrollOffset;

        public InteractionRequest<Notification> ResetScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public IArrangedBookStorage BookCabinet
        {
            get { return _Cabinet; }
            set { SetProperty(ref _Cabinet, value); }
        }

        public void ResetScrollOffsetPool()
        {
            _scrollOffset = new Dictionary<Guid, Point>();
        }

        public void StoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StoreBookScrollOffsetRequest.Raise(new Notification()
                    {
                        Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                    });
                });
            }
            else
            {
                StoreContentScrollOffsetRequest.Raise(new Notification()
                {
                    Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                });
            }
        }

        public void RestoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RestoreBookScrollOffsetRequest.Raise(new Notification()
                    {
                        Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                    });
                });
            }
            else
            {
                RestoreContentScrollOffsetRequest.Raise(new Notification()
                {
                    Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                });
            }
        }

        public void ResetScrollOffset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ResetScrollOffsetRequest.Raise(new Notification());
            });
        }
    }
}
