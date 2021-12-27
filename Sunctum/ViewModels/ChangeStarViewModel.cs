using Prism.Services.Dialogs;
using Reactive.Bindings;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System;
using System.Reactive.Linq;

namespace Sunctum.ViewModels
{
    internal class ChangeStarViewModel : IDialogAware
    {
        public ReactiveCommand Star1Command { get; }
        public ReactiveCommand Star2Command { get; }
        public ReactiveCommand Star3Command { get; }
        public ReactiveCommand Star4Command { get; }
        public ReactiveCommand Star5Command { get; }
        public ReactiveCommand NotEvaluatedCommand { get; }

        public string Title => "Change star";

        public ReactiveProperty<BookViewModel> EditTarget = new ReactiveProperty<BookViewModel>();

        public event Action<IDialogResult> RequestClose;

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
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            Star2Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star2Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 2;
                StarFacade.InsertOrReplace(EditTarget.Value);
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            Star3Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star3Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 3;
                StarFacade.InsertOrReplace(EditTarget.Value);
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            Star4Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star4Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 4;
                StarFacade.InsertOrReplace(EditTarget.Value);
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            Star5Command = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            Star5Command.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = 5;
                StarFacade.InsertOrReplace(EditTarget.Value);
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            NotEvaluatedCommand = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            NotEvaluatedCommand.Subscribe(_ =>
            {
                EditTarget.Value.StarLevel = null;
                StarFacade.InsertOrReplace(EditTarget.Value);
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            EditTarget.Value = parameters.GetValue<BookViewModel>("Book");
        }
    }
}
