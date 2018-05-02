

using System;

namespace Sunctum.Domain.Models.Managers
{
    public class SearchedEventArgs : EventArgs
    {
        private readonly string _searchingText;
        private readonly string _previousSearchingText;

        public SearchedEventArgs(string searchingText, string _previousSearchingText)
        {
            _searchingText = searchingText;
            this._previousSearchingText = _previousSearchingText;
        }

        public string SearchingText { get { return _searchingText; } }

        public string PreviousSearchingText { get { return _previousSearchingText; } }
    }
}