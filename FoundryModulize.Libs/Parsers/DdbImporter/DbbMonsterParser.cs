using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Libs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Libs.Parsers.DdbImporter
{
    public class DbbMonsterParser : IPackParser{
        
        public async Task<Manifest> ParsePack(string path)
        {
           
                var module = new Manifest
                {
                    Name = "sulex-monsters",
                    Title = "Sulex's Marvelous Menagerie",
                    Description = "A collection of monsters for D&D 5E",
                    Author = "sulex",
                    Version = "0.0.1",
                    MinCoreVersion = "0.7.0"
                };

                var allPack = new Pack
                {
                    Name = "sulex-all",
                    Label = "Sulex's All",
                    Module = module.Name,
                    System = "dnd5e",
                    Path = "sulex-all",
                    Entity = EntityType.Actor.ToString()
                };

                var beastsPack = new Pack
                {
                    Name = "sulex-beasts",
                    Label = "Sulex's Beasts",
                    Module = module.Name,
                    System = "dnd5e",
                    Path = "sulex-beasts",
                    Entity = EntityType.Actor.ToString()
                };

                var namedPack = new Pack
                {
                    Name = "sulex-named",
                    Label = "Sulex's Personas",
                    Module = module.Name,
                    System = "dnd5e",
                    Path = "sulex-named",
                    Entity = EntityType.Actor.ToString()
                };

                var npcPack = new Pack
                {
                    Name = "sulex-npc",
                    Label = "Sulex's NPCs",
                    Module = module.Name,
                    System = "dnd5e",
                    Path = "sulex-npc",
                    Entity = EntityType.Actor.ToString()
                };

                var nonnpcPack = new Pack
                {
                    Name = "sulex-monsters",
                    Label = "Sulex's Monsters",
                    Module = module.Name,
                    System = "dnd5e",
                    Path = "sulex-monsters",
                    Entity = EntityType.Actor.ToString()
                };



                var npcsList = await File.ReadAllLinesAsync(Path.Combine(root, "npc_generic.txt"));

                var npcs = npcsList.Select(x =>
                {
                    var npc = JsonConvert.DeserializeObject<Npc>(x);
                    return npc.name.Trim();
                }).ToHashSet();

                var namedList = await File.ReadAllLinesAsync(Path.Combine(root, "npc_named.txt"));

                var namedNpcs = namedList.Select(x =>
                {
                    var npc = JsonConvert.DeserializeObject<Npc>(x);
                    return npc.name.Trim();
                }).ToHashSet();



                var vals = await File.ReadAllLinesAsync(Path.Combine(root, dbName));


                foreach (var val in vals)
                {
                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                        allPack.Objects.Add(obj);

                        var name = (string)obj.name;

                        if (name.Contains("(level") || npcs.Contains(name.Trim()))
                        {
                            npcPack.Objects.Add(obj);
                        }
                        else if (namedNpcs.Contains(name.Trim()))
                        {
                            namedPack.Objects.Add(obj);
                        }
                        else if (obj.data.details.type == "Beast")
                        {
                            beastsPack.Objects.Add(obj);
                        }
                        else
                        {
                            nonnpcPack.Objects.Add(obj);
                        }
                    }
                    catch (Exception ex)
                    {
      
                    }
                }

                module.Packs.Add(allPack);
                module.Packs.Add(beastsPack);
                module.Packs.Add(namedPack);
                module.Packs.Add(npcPack);
                module.Packs.Add(nonnpcPack);

                return module;
            }
        }
    }
}
