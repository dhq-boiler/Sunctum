

using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sunctum.UI.ViewModel
{
    public class EntityManagementDialogViewModel<E> : NotifyPropertyChangedImpl where E : class, IId, IName, ICloneable
    {
        private EntityManagementDialog<E> _parent;
        private ILibraryManager _libVM;
        private string _title;
        private List<E> _Entities;
        private E _Selected;
        private string _NewName;
        private E _IntegrateInto;
        private Func<string, E> _add;
        private Func<IEnumerable<E>> _readAll;
        private Func<Guid, E> _readBy;
        private Action<E> _update;
        private Action<Guid> _remove;
        private Action<E, E> _integrate;

        public EntityManagementDialogViewModel(EntityManagementDialog<E> dialog, ILibraryManager libVM, string title,
            Func<string, E> add,
            Func<IEnumerable<E>> readAll,
            Func<Guid, E> readBy,
            Action<E> update,
            Action<Guid> remove,
            Action<E, E> integrate)
        {
            _parent = dialog;
            _libVM = libVM;
            Title = title;
            _add = add;
            _readAll = readAll;
            _readBy = readBy;
            _update = update;
            _remove = remove;
            _integrate = integrate;
        }

        public void Initialize()
        {
            _parent.DataContext = this;
            LoadEntities();
        }

        private void LoadEntities()
        {
            Entities = new List<E>(_readAll.Invoke());
        }

        internal void Revert()
        {
            if (UpdateRevertButtonIsEnable)
            {
                var reference = _readBy.Invoke(Selected.ID);
                Selected = reference.Clone() as E;
            }
        }

        internal void Remove()
        {
            if (RemoveButtonIsEnable)
            {
                var target = Selected;
                _remove.Invoke(target.ID);

                int index = Entities.FindIndex(a => a.ID == target.ID);
                Entities.RemoveAt(index);
                LoadEntities();

                Selected = default(E);
            }
        }

        internal void Integrate()
        {
            var into = IntegrateInto;
            var willDiscard = Selected;

            _integrate.Invoke(willDiscard, into);

            int index = Entities.FindIndex(a => a.ID == willDiscard.ID);
            Entities.RemoveAt(index);
            LoadEntities();
        }

        internal void Update()
        {
            if (UpdateRevertButtonIsEnable)
            {
                var target = Selected;
                _update.Invoke(target);

                int index = Entities.FindIndex(a => a.ID == target.ID);
                Entities[index].UnescapedName = target.Name;

                ButtonStateUpdate_UpdateRevertRemove();
            }
        }

        internal void Add()
        {
            if (AddButtonIsEnable)
            {
                var newEntity = _add.Invoke(NewName);
                Entities.Add(newEntity);
                LoadEntities();
                NewName = "";
            }
        }

        public string Title
        {
            [DebuggerStepThrough]
            get
            { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public List<E> Entities
        {
            [DebuggerStepThrough]
            get
            { return _Entities; }
            set { SetProperty(ref _Entities, value); }
        }

        public E Selected
        {
            [DebuggerStepThrough]
            get
            { return _Selected; }
            set { SetProperty(ref _Selected, value); }
        }

        public string NewName
        {
            [DebuggerStepThrough]
            get
            { return _NewName; }
            set
            {
                SetProperty(ref _NewName, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => AddButtonIsEnable));
            }
        }

        public E IntegrateInto
        {
            [DebuggerStepThrough]
            get
            { return _IntegrateInto; }
            set
            {
                SetProperty(ref _IntegrateInto, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => IntegrateButtonIsEnable));
            }
        }

        public bool UpdateRevertButtonIsEnable
        {
            get
            {
                if (Selected == null) return false;
                var entity = Entities.Where(a => a.ID.Equals(Selected.ID)).Single();
                return !entity.Name.Equals(NewName) && UpdateSupported && ReadBySupported;
            }
        }

        public bool RemoveButtonIsEnable
        {
            [DebuggerStepThrough]
            get
            { return Selected != null && RemoveSupported; }
        }

        public bool AddButtonIsEnable
        {
            get
            {
                return NewName != null
                    && AddSupported
                    && NewName.Count() > 0
                    && _Entities.Where(a => a.Name.Equals(NewName)).Count() == 0;
            }
        }

        public bool IntegrateButtonIsEnable
        {
            get
            {
                return Selected != null
                    && IntegrateInto != null
                    && IntegrateSupported
                    && Selected.ID != IntegrateInto.ID;
            }
        }

        public void ButtonsStateUpdate_Add()
        {
            OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => AddButtonIsEnable));
        }

        public void ButtonStateUpdate_UpdateRevertRemove()
        {
            OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => UpdateRevertButtonIsEnable),
                              PropertyNameUtility.GetPropertyName(() => RemoveButtonIsEnable));
        }

        public bool AddSupported
        {
            get { return _add != null; }
        }

        public bool ReadAllSupported
        {
            get { return _readAll != null; }
        }

        public bool ReadBySupported
        {
            get { return _readBy != null; }
        }

        public bool UpdateSupported
        {
            get { return _update != null; }
        }

        public bool RemoveSupported
        {
            get { return _remove != null; }
        }

        public bool IntegrateSupported
        {
            get { return _integrate != null; }
        }
    }
}
