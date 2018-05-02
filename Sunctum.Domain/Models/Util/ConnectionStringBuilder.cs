
namespace Sunctum.Domain.Models
{
    public class ConnectionStringBuilder
    {
        public static string Build(string filename)
        {
            return $"Data Source={filename}";
        }
    }
}
