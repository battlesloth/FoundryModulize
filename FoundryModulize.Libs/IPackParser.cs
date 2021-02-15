using System.Threading.Tasks;
using FoundryModulize.Libs.Models;

namespace FoundryModulize.Libs
{
    public interface IPackParser
    {
        Task<Manifest> ParsePack(string path);
    }
}
