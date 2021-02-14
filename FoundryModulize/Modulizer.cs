using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using FoundryModulize.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize
{
    public class Modulizer
    {

        private string root;
        private string output;
        private string datapath;
        
       // public Manifest Manifest { get; set; }


        public Modulizer(string root, string dataPath, string output)
        {
            this.root = root;
            this.output = output;
            this.datapath = dataPath;
        }

        
        private async Task CreateModule(Manifest manifest)
        {
            await CreateModuleStructure(manifest);
            
            foreach (var pack in manifest.Packs)
            {
                await WriteObjectsToPack(pack, manifest);
            }
        }

        public async Task CreateModuleStructure(Manifest manifest)
        {
            var modulePath = Path.Combine(output, manifest.Name);

            Directory.CreateDirectory(modulePath);

            Directory.CreateDirectory(Path.Combine(modulePath, "packs"));

            Directory.CreateDirectory(Path.Combine(modulePath, "images"));

            if (manifest.Packs.Any(x => x.Entity == EntityType.Actor.ToString()))
            {
                Directory.CreateDirectory(Path.Combine(modulePath, "images", "portraits"));
                Directory.CreateDirectory(Path.Combine(modulePath, "images", "tokens"));
                Directory.CreateDirectory(Path.Combine(modulePath, "images", "items"));
            }

            var json = JsonConvert.SerializeObject(manifest);

            await File.WriteAllTextAsync(Path.Combine(modulePath, "module.json"), json);

        }
        

        public async Task WriteObjectsToPack(Pack pack, Manifest manifest)
        {
            var modulePath = Path.Combine(output, manifest.Name, "packs");
            
            var lines = new List<string>();

            foreach (dynamic obj in pack.Objects)
            {
                CopyImagesToPack(obj, manifest.Name, pack.Entity == EntityType.Actor.ToString());

                lines.Add(JsonConvert.SerializeObject(obj));   
            }

            await File.WriteAllLinesAsync(Path.Combine(modulePath, $"{pack.Name}.db"), lines);
        }


        public void CopyImagesToPack(dynamic obj, string moduleName, bool isActor)
        {
            var tokenImg = string.Empty;
            var portraitImg = string.Empty;
            var items = 0;

            try
            {
                portraitImg = (string) obj.img;
            }
            catch
            {
            }

            if (!string.IsNullOrEmpty(portraitImg))
            {
                obj.img = CopyImage(portraitImg, moduleName, "images", isActor ? "portraits" : "");
            }

            if (isActor)
            {
                try
                {
                    tokenImg = (string) obj.token.img;
                }
                catch
                {
                }

                try
                {
                    items = obj.items.Count;
                }
                catch (Exception e)
                {
                }


                if (!string.IsNullOrEmpty(tokenImg))
                {
                    obj.token.img = CopyImage(tokenImg, moduleName, "images", "tokens");
                }


                if (items > 0)
                {
                    foreach (var item in obj.items)
                    {
                        var img = string.Empty;

                        try
                        {
                            img = (string)item.img;
                        }
                        catch { }

                        if (!string.IsNullOrEmpty(img))
                        {
                            item.img = CopyImage(img, moduleName, "images", "items");
                        }
                    }
                }
            }
        }


        private string CopyImage(string image, string moduleName, string dir, string subDir)
        {
            image = image.Replace('/', '\\');

            var imgPath = Path.Combine(datapath, image.TrimStart('\\'));
            var fileName = Path.GetFileName(imgPath);

            var returnPath = $"modules/{moduleName}/{dir}/{(string.IsNullOrEmpty(subDir) ? "" : $"{subDir}/")}{fileName}";

            try
            {
                var path = string.IsNullOrEmpty(subDir) ?
                    Path.Combine(output, moduleName, dir, fileName) :
                    Path.Combine(output, moduleName, dir, subDir, fileName);

                // file is already in out location.
                if (File.Exists(path))
                {
                    return returnPath;
                }
                if (File.Exists(imgPath))
                {
                    
                    File.Copy(imgPath,path);
                    return returnPath;
                }

                Console.WriteLine($"{image} couldn't be found!");

                return image;
            }
            catch (Exception e)
            {
                if (e.Message.EndsWith("already exists."))
                {
                    return returnPath;
                }
                Console.WriteLine($"Exception copying image for {image}. EX: {e.Message}");
                return "";
            }
        }

        public async Task PackageMonsters(string dbName)
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

                    var name = (string) obj.name;

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
                    Console.WriteLine(ex.Message);
                }
            }

            module.Packs.Add(allPack);
            module.Packs.Add(beastsPack);
            module.Packs.Add(namedPack);
            module.Packs.Add(npcPack);
            module.Packs.Add(nonnpcPack);

            await CreateModule(module);
        }

        public async Task PackageSpells(string dbName)
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

            await CreateModule(module);
        }

        public async Task SplitOutItems(string dbName)
        {
            
            var module = new Manifest
            {
                Name = "sulex-emporium",
                Title = "Sulex's Wondrous Emporium",
                Description = "A collection of items from the mundane to the magical for D&D 5E",
                Author = "sulex",
                Version = "0.0.1",
                MinCoreVersion = "0.7.0"
            };

            var magicPack = new Pack
            {
                Name = "sulex-magic",
                Label = "Sulex's Magic Items",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-magic",
                Entity = EntityType.Item.ToString()
            };

            var magicArmorPack = new Pack
            {
                Name = "sulex-magic-armor",
                Label = "Sulex's Magic Armor",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-magic-armor",
                Entity = EntityType.Item.ToString()
            };

            var magicWeaponsPack = new Pack
            {
                Name = "sulex-magic-weapons",
                Label = "Sulex's Magic Weapons",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-magic-weapons",
                Entity = EntityType.Item.ToString()
            };

            var magicConsumablePack = new Pack
            {
                Name = "sulex-magic-consumable",
                Label = "Sulex's Magic Consumable",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-magic-consumable",
                Entity = EntityType.Item.ToString()
            };

            var lootPack = new Pack
            {
                Name = "sulex-loot",
                Label = "Sulex's Loot",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-loot",
                Entity = EntityType.Item.ToString()
            };

            var consumablePack = new Pack
            {
                Name = "sulex-consumable",
                Label = "Sulex's Consumables",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-consumable",
                Entity = EntityType.Item.ToString()
            };

            var ammoPack = new Pack
            {
                Name = "sulex-ammo",
                Label = "Sulex's Ammunition",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-ammo",
                Entity = EntityType.Item.ToString()
            };


            var transportPack = new Pack
            {
                Name = "sulex-transport",
                Label = "Sulex's Transportation",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-transport",
                Entity = EntityType.Item.ToString()
            };

            var mundanePack = new Pack
            {
                Name = "sulex-mundane",
                Label = "Sulex's Mundane Items",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-mundane",
                Entity = EntityType.Item.ToString()
            };


            var armoryPack = new Pack
            {
                Name = "sulex-armory",
                Label = "Sulex's Armory",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-armory",
                Entity = EntityType.Item.ToString()
            };

            var toSortPack = new Pack
            {
                Name = "sulex-to-sort",
                Label = "Sulex's To Sort",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-to-sort",
                Entity = EntityType.Item.ToString()
            };


            var trashPack = new Pack
            {
                Name = "sulex-trash",
                Label = "Sulex's Trash",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-trash",
                Entity = EntityType.Item.ToString()
            };

            var vals = await File.ReadAllLinesAsync(Path.Combine(root, "items.db"));

            foreach (var val in vals)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                if (obj.flags.magicitems.enabled)
                {

                    try
                    {
                        var type = (string)obj.flags.ddbimporter.dndbeyond.filterType;

                        if (type.Contains("Armor"))
                        {
                            magicArmorPack.Objects.Add(obj);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    if (obj.type == "weapon")
                    {
                        magicWeaponsPack.Objects.Add(obj);
                    }
                    else if (obj.type == "consumable")
                    {
                        magicConsumablePack.Objects.Add(obj);
                    }
                    else
                    {
                        magicPack.Objects.Add(obj);
                    }
                }
                else if (obj.type == "loot")
                {
                    var temp = (string) obj.flags.ddbimporter.dndbeyond.type;
                    if (temp.Contains("Holy") || temp.Contains("Focus"))
                    {
                        mundanePack.Objects.Add(obj);
                        continue;
                    }

                    switch (obj.flags.ddbimporter.dndbeyond.type)
                    {
                        case "Mount":
                            trashPack.Objects.Add(obj);
                            break;
                        case "Adventuring Gear":
                            mundanePack.Objects.Add(obj);
                            break;
                        case "Vehicle":
                            transportPack.Objects.Add(obj);
                            break;
                        default:
                            lootPack.Objects.Add(obj);
                            break;
                    }
                }
                else if (obj.type == "consumable")
                {
                    switch (obj.data.consumableType)
                    {
                        case "ammo":
                            ammoPack.Objects.Add(obj);
                            break;
                        case "potion":
                            consumablePack.Objects.Add(obj);
                            break;
                        default:
                            toSortPack.Objects.Add(obj);
                            break;
                    }
                }
                else
                {
                    try
                    {
                        var type = (string) obj.flags.ddbimporter.dndbeyond.filterType;

                        if (type.Contains("Armor"))
                        {
                            armoryPack.Objects.Add(obj);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    if (obj.type == "weapon")
                    {
                        armoryPack.Objects.Add(obj);
                    }
                    else
                    {
                        mundanePack.Objects.Add(obj);
                    }
                }

            }

            module.Packs.Add(magicPack);
            module.Packs.Add(magicArmorPack);
            module.Packs.Add(magicWeaponsPack);
            module.Packs.Add(magicConsumablePack);
            module.Packs.Add(lootPack);
            module.Packs.Add(ammoPack);
            module.Packs.Add(consumablePack);
            module.Packs.Add(transportPack);
            module.Packs.Add(mundanePack);
            module.Packs.Add(armoryPack);
            module.Packs.Add(toSortPack);
            module.Packs.Add(trashPack);

            await CreateModule(module);
        }

        private class Npc
        {
            public string name { get; set; }
            public string source { get; set; }
        }
    }
}
