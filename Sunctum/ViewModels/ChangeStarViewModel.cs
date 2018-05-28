

using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System;
using System.Reactive.Linq;

namespace Sunctum.ViewModels
{
    internal class ChangeStarViewModel : IInteractionRequestAware
    {
        public ReactiveCommand Star1Command { get; }
        public ReactiveCommand Star2Command { get; }
        public ReactiveCommand Star3Command { get; }
        public ReactiveCommand Star4Command { get; }
        public ReactiveCommand Star5Command { get; }
        public ReactiveCommand NotEvaluatedCommand { get; }

        public ReactiveProperty<BookViewModel> EditTarget = new ReactiveProperty<BookViewModel>();
        private INotification _Notification;

        public ChangeStarViewModel()
        {
            Star1Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star1Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 1;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });

            Star2Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star2Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 2;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });

            Star3Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star3Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 3;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });

            Star4Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star4Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 4;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });

            Star5Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star5Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 5;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });

            NotEvaluatedCommand = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            NotEvaluatedCommand.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = null;
                StarFacade.InsertOrReplace(EditTarget.Value);
                FinishInteraction();
            });
        }

        public INotification Notification
        {
            get { return _Notification; }
            set
            {
                _Notification = value;
                EditTarget.Value = value.Content as BookViewModel;
            }
        }
        public Action FinishInteraction { get; set; }
    }
}
