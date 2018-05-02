

using Sunctum.Domail.Util;
using System;
using System.ComponentModel;

namespace Sunctum.Domain.Models.Managers
{
    public interface IProgressManager : INotifyPropertyChanged
    {
        void UpdateProgress(int current, int total, TimeKeeper timekeeper);

        void Complete();

        double Progress { get; }

        TimeSpan? EstimateRemainTime { get; }

        void Abort();

        bool IsAbort { get; }
    }
}
