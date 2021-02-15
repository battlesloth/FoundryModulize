using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Libs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Libs.Parsers.DdbImporter
{
    public class DbbSpellParser : IPackParser 
    {
        public async Task<Manifest> ParsePack(string path)
        {
            var module = new Manifest
            {
                Name = "sulex-spells",
                Title = "Sulex's Mystical Tome",
                Description = "A collection of spells for D&D 5E",
                Author = "sulex",
                Version = "0.0.1",
                MinCoreVersion = "0.7.0"
            };

            var magicPack = new Pack
            {
                Name = "sulex-spells",
                Label = "Sulex's Spells",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-spells",
                Entity = EntityType.Item.ToString()
            };

            var vals = await File.ReadAllLinesAsync(Path.Combine(root, dbName));
            foreach (var val in vals)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                magicPack.Objects.Add(obj);
            }

            module.Packs.Add(magicPack);

            return module;
        }
    }
}
