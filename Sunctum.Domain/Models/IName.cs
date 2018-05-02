
namespace Sunctum.Domain.Models
{
    public interface IName
    {
        string Name { get; set; }

        string UnescapedName { get; set; }
    }
}
