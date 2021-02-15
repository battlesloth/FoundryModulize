using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Libs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Libs.Parsers.DdbImporter
{
    public class DbbItemParser : IPackParser
    {
        public async Task<Manifest> ParsePack(string path)
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


            var allPack = new Pack
            {
                Name = "sulex-all",
                Label = "Sulex's All Items",
                Module = module.Name,
                System = "dnd5e",
                Path = "sulex-all",
                Entity = EntityType.Item.ToString()
            };


            var vals = await File.ReadAllLinesAsync(Path.Combine(root, "items.db"));

            foreach (var val in vals)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                allPack.Objects.Add(obj);

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
                    var temp = (string)obj.flags.ddbimporter.dndbeyond.type;
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
                        var type = (string)obj.flags.ddbimporter.dndbeyond.filterType;

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
            module.Packs.Add(allPack);

            return module;
        }
    }
}
