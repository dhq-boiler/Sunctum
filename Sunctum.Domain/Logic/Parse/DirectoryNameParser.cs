
namespace Sunctum.Domain.Logic.Parse
{
    public class DirectoryNameParser : DirectoryNameParserBase
    {
        private int _Priority;
        private string _Pattern;

        public int Priority
        {
            get { return _Priority; }
            set { SetProperty(ref _Priority, value); }
        }

        public override string Pattern
        {
            get { return _Pattern; }
            set { SetProperty(ref _Pattern, value); }
        }

        public DirectoryNameParser()
        { }

        public DirectoryNameParser(string pattern)
        {
            Pattern = pattern;
        }
    }
}
