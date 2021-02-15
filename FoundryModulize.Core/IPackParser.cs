using System.Threading.Tasks;
using FoundryModulize.Core.Models;

namespace FoundryModulize.Core
{
    public interface IPackParser
    {
        Task<Manifest> ParsePack(string path);
        Task LoadCatalog(string path);
    }
}
