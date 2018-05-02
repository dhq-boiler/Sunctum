

using System;
using System.Collections.Generic;

namespace Sunctum.Domail.Util
{
    public class TimeKeeper
    {
        private LinkedList<DateTime> _beginTaskDateTimeList;
        private LinkedList<TimeSpan> _taskElapsedTimeList;

        public LinkedList<TimeSpan> TaskElapsedTimeList
        {
            get { return new LinkedList<TimeSpan>(_taskElapsedTimeList); }
        }

        public DateTime? BeginTaskDatetime { get { return _beginTaskDateTimeList?.First.Value; } }

        public TimeKeeper()
        {
            Reset();
        }

        public void Reset()
        {
            _beginTaskDateTimeList = new LinkedList<DateTime>();
            _taskElapsedTimeList = new LinkedList<TimeSpan>();
        }

        public void RecordTime()
        {
            _beginTaskDateTimeList.AddLast(DateTime.Now);
            if (_beginTaskDateTimeList.Count >= 2)
            {
                _taskElapsedTimeList.AddLast(_beginTaskDateTimeList.Last.Value - _beginTaskDateTimeList.Last.Previous.Value);
            }
        }
    }
}
