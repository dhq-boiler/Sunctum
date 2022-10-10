

using Homura.Core;
using Microsoft.AspNetCore.Components;
using NLog;
using Prism.Mvvm;
using Sunctum.Domail.Util;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Sunctum.Managers
{
    public class ProgressManager : BindableBase, IProgressManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private int _TotalCount;
        private int _Current;
        private DateTime? _previousUpdateDateTime;
        private bool _IsAbort;

        public double Progress
        {
            get { return GetProgress(); }
        }

        public int TotalCount
        {
            get { return _TotalCount; }
            private set
            {
                _Current = 0;
                SetProperty(ref _TotalCount, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => Progress));
            }
        }

        public int Current
        {
            get { return _Current; }
            private set
            {
                SetProperty(ref _Current, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => Progress));
            }
        }

        public bool IsAbort
        {
            get { return _IsAbort; }
            private set { SetProperty(ref _IsAbort, value); }
        }

        public TimeSpan? EstimateRemainTime { get; private set; }

        [Unity.Dependency]
        public Lazy<IMainWindowViewModel> mainWindowViewModel { get; set; }

        public ProgressManager()
        { }

        public ProgressManager(int totalCount)
        {
            _TotalCount = totalCount;
        }

        public double GetProgress()
        {
            if (_TotalCount == 0.0) return 0.0;
            return (double)_Current / _TotalCount;
        }

        private ConcurrentQueue<DateTime> queue = new ConcurrentQueue<DateTime>();

        private const int _PoolQueueCount = 1000;

        public void UpdateProgress(int current, int total, TimeKeeper timekeeper)
        {
            _TotalCount = total;
            _Current = current;
            if (App.Current is not null)
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!(mainWindowViewModel is null))
                    {
                        mainWindowViewModel.Value.TaskbarItemInfo.Value.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        mainWindowViewModel.Value.TaskbarItemInfo.Value.ProgressValue = (double)current / (double)total;
                    }
                });
            }
            timekeeper.RecordTime();
            if (current > 100 && current < total)
            {
                while (queue.Count > _PoolQueueCount)
                {
                    queue.TryDequeue(out var result);
                }

                var now = DateTime.Now;

                if (_previousUpdateDateTime.HasValue)
                {
                    queue.TryPeek(out var result);
                    var diff = (DateTime.Now - result);
                    var avgMilliseconds = diff.TotalMilliseconds / queue.Count;
                    var remain = total - current;
                    var remainTimesAvgMilliseconds = remain * avgMilliseconds;
                    var duration = now - _previousUpdateDateTime.Value;
                    if (duration.Seconds >= 1)
                    {
                        try
                        {
                            EstimateRemainTime = TimeSpan.FromMilliseconds(remainTimesAvgMilliseconds);
                        }
                        catch (OverflowException e)
                        {
                            s_logger.Error(e);
                        }
                        finally
                        {
                            _previousUpdateDateTime = now;
                            queue.Enqueue(now);
                        }
                    }
                }
                else
                {
                    _previousUpdateDateTime = now;
                    queue.Enqueue(now);
                }
            }
            else if (current == total)
            {
                EstimateRemainTime = null;
                _previousUpdateDateTime = null;
            }
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => Current));
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => Progress));
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => EstimateRemainTime));
        }

        public void Complete()
        {
            Current = TotalCount = 1;
            if (App.Current is not null)
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!(mainWindowViewModel is null))
                    {
                        mainWindowViewModel.Value.TaskbarItemInfo.Value.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                    }
                });
            }

            queue.Clear();
        }

        public void Abort()
        {
            IsAbort = true;
        }
    }
}
