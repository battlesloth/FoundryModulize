using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Core.Parsers.DdbImporter
{
    public class DbbMonsterParser : BaseParser
    {
        private const string allPack = "sulex-all";
        private const string namedPack = "sulex-named";
        private const string npcPack = "sulex-npc";
        private const string beastPack = "sulex-beasts";
        private const string monsterPack = "sulex-monsters";

        private Catalog catalog;

        public DbbMonsterParser() : base(
            "sulex-monsters",
            "Sulex's Marvelous Menagerie",
            "A collection of monsters for D&D 5E",
            "sulex",
            "0.5.0.1",
            "0.7.0",
            EntityType.Actor){
           
            catalog = new Catalog();
        }

        public override async Task LoadCatalog(string path)
        {
            await catalog.LoadCatalog(path);
        }

        public override async Task<Manifest> ParsePack(string path)
        {
            var all = GeneratePack(allPack, "Sulex's All");
            var beasts = GeneratePack(beastPack, "Sulex's Beasts");
            var named = GeneratePack(namedPack, "Sulex's Personas");
            var npcs= GeneratePack(npcPack, "Sulex's NPCs");
            var monsters = GeneratePack(monsterPack, "Sulex's Monsters");

            var vals = await File.ReadAllLinesAsync(path);

            foreach (var val in vals)
            {

                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                all.Objects.Add(obj);

                var name = (string) obj.name;

                if (catalog.IsInACatalog(name.Trim(), out var category))
                {
                    switch (category)
                    {
                        case namedPack:
                            named.Objects.Add(obj);
                            continue;
                        case npcPack:
                            npcs.Objects.Add(obj);
                            continue;
                        case beastPack:
                            beasts.Objects.Add(obj);
                            continue;
                        case monsterPack:
                            monsters.Objects.Add(obj);
                            continue;
                    }
                }

                if (name.Contains("(level"))
                {
                    npcs.Objects.Add(obj);
                }
                else if (obj.data.details.type == "Beast")
                {
                    beasts.Objects.Add(obj);
                }
                else
                {
                   monsters.Objects.Add(obj);
                }
            }

            module.Packs.Add(all);
            module.Packs.Add(beasts);
            module.Packs.Add(named);
            module.Packs.Add(npcs);
            module.Packs.Add(monsters);

            return module;
        }
    }
}
