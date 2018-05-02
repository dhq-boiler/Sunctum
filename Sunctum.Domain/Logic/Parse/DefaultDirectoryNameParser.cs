
namespace Sunctum.Domain.Logic.Parse
{
    public class DefaultDirectoryNameParser : DirectoryNameParserBase
    {
        public override string Pattern { get; set; } = "^\\s*(\\s*\\((?<tag>.*?)\\)\\s*)?(\\s*\\[\\s*(?<author>.*?)\\s*\\]\\s*)?\\s*(?<title>.+?)$";
    }
}
