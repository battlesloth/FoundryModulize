using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Core.Parsers.DdbImporter
{
    public class DbbSpellParser : BaseParser
    {

        private const string magicPack = "sulex-spells";

        public DbbSpellParser() : base (
            "sulex-spells",
            "Sulex's Mystical Tome",
            "A collection of spells for D&D 5E",
            "sulex",
            "0.0.1",
            "0.7.0",
            EntityType.Item){ }

        public override async Task<Manifest> ParsePack(string path)
        {

            var magic = GeneratePack(magicPack, "Sulex's Spells");

            var vals = await File.ReadAllLinesAsync(path);
            
            foreach (var val in vals)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                magic.Objects.Add(obj);
            }

            module.Packs.Add(magic);

            return module;
        }

        public override Task LoadCatalog(string path)
        {
            return  Task.CompletedTask;
        }
    }
}
