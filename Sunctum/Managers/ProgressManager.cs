

using Prism.Mvvm;
using Sunctum.Domail.Util;
using Sunctum.Domain.Models.Managers;
using Sunctum.Infrastructure.Core;
using System;

namespace Sunctum.Managers
{
    public class ProgressManager : BindableBase, IProgressManager
    {
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

        public ProgressManager()
        { }

        public ProgressManager(int totalCount)
        {
            _TotalCount = totalCount;
        }

        public double GetProgress()
        {
            if (_TotalCount == 0.0) throw new InvalidOperationException("TotalCount is not set");
            return (double)_Current / _TotalCount;
        }

        public void UpdateProgress(int current, int total, TimeKeeper timekeeper)
        {
            _TotalCount = total;
            _Current = current;
            timekeeper.RecordTime();
            if (current > 100 && current < total)
            {
                var sum = (DateTime.Now - timekeeper.BeginTaskDatetime.Value);
                var avgMilliseconds = sum.TotalMilliseconds / current;
                var remain = total - current;
                var remainTimesAvgMilliseconds = remain * avgMilliseconds;

                var now = DateTime.Now;

                if (_previousUpdateDateTime.HasValue)
                {
                    var duration = now - _previousUpdateDateTime.Value;
                    if (duration.Seconds >= 1)
                    {
                        EstimateRemainTime = TimeSpan.FromMilliseconds(remainTimesAvgMilliseconds);
                        _previousUpdateDateTime = now;
                    }
                }
                else
                {
                    _previousUpdateDateTime = now;
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
            Current = TotalCount;
        }

        public void Abort()
        {
            IsAbort = true;
        }
    }
}
