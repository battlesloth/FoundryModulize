using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using FoundryModulize.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Core.Parsers.DdbImporter
{
    public class DbbItemParser : BaseParser
    {    
       private const string allPack = "sulex-all";
       private const string magicPack = "sulex-magic";
       private const string magicArmorPack = "sulex-magic-armor";
       private const string magicWeaponsPack = "sulex-magic-weapons";
       private const string magicConsumablePack = "sulex-magic-consumable";
       private const string lootPack = "sulex-loot";
       private const string consumablePack = "sulex-consumable";
       private const string ammoPack = "sulex-ammo";
       private const string transportPack = "sulex-transport";
       private const string mundanePack = "sulex-mundane";
       private const string armoryPack = "sulex-armory";
       private const string toSortPack = "sulex-to-sort";


        private Catalog catalog;

        public DbbItemParser() : base(
            "sulex-emporium",
            "Sulex's Wondrous Emporium",
            "A collection of items from the mundane to the magical for D&D 5E",
            "sulex",
            "0.0.1",
            "0.7.0",
            EntityType.Actor) {

            catalog = new Catalog();
        }

        public override async Task LoadCatalog(string path)
        {
            await catalog.LoadCatalog(path);
        }

        public override async Task<Manifest> ParsePack(string path)
        {

            var all= GeneratePack(allPack, "Sulex's All Items");
            var magic = GeneratePack(magicPack, "Sulex's Magic Items");
            var magicArmor = GeneratePack(magicArmorPack, "Sulex's Magic Armor");
            var magicWeapons = GeneratePack(magicWeaponsPack,"Sulex's Magic Weapons");
            var magicConsumable = GeneratePack(magicConsumablePack,"Sulex's Magic Consumable");
            var loot = GeneratePack(lootPack, "Sulex's Loot");
            var consumable = GeneratePack(consumablePack, "Sulex's Consumables");
            var ammo = GeneratePack(ammoPack, "Sulex's Ammunition");
            var transport = GeneratePack(transportPack, "Sulex's Transportation");
            var mundane = GeneratePack(mundanePack, "Sulex's Mundane Items");
            var armory = GeneratePack(armoryPack, "Sulex's Armory");
            var toSort = GeneratePack(toSortPack, "Sulex's To Sort");

            var vals = await File.ReadAllLinesAsync(path);

            foreach (var val in vals)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                all.Objects.Add(obj);

                var name = (string)obj.name;

                if (catalog.IsInACatalog(name.Trim(), out var category))
                {
                    switch (category)
                    {
                        case allPack:
                            all.Objects.Add(obj);
                            continue;
                        case magicPack:
                            magic.Objects.Add(obj);
                            continue;
                        case magicArmorPack:
                            magicArmor.Objects.Add(obj);
                            continue;
                        case magicWeaponsPack:
                            magicWeapons.Objects.Add(obj);
                            continue;
                        case magicConsumablePack:
                            magicConsumable.Objects.Add(obj);
                            continue;
                        case lootPack:
                            loot.Objects.Add(obj);
                            continue;
                        case consumablePack:
                            consumable.Objects.Add(obj);
                            continue;
                        case ammoPack:
                            ammo.Objects.Add(obj);
                            continue;
                        case transportPack:
                            transport.Objects.Add(obj);
                            continue;
                        case mundanePack:
                            mundane.Objects.Add(obj);
                            continue;
                        case armoryPack:
                            armory.Objects.Add(obj);
                            continue;
                    }
                }

                if (obj.flags.magicitems.enabled)
                {

                    try
                    {
                        var type = (string)obj.flags.ddbimporter.dndbeyond.filterType;

                        if (type.Contains("Armor"))
                        {
                            magicArmor.Objects.Add(obj);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    if (obj.type == "weapon")
                    {
                        magicWeapons.Objects.Add(obj);
                    }
                    else if (obj.type == "consumable")
                    {
                        magicConsumable.Objects.Add(obj);
                    }
                    else
                    {
                        magic.Objects.Add(obj);
                    }
                }
                else if (obj.type == "loot")
                {
                    var temp = (string)obj.flags.ddbimporter.dndbeyond.type;
                    if (temp.Contains("Holy") || temp.Contains("Focus"))
                    {
                        mundane.Objects.Add(obj);
                        continue;
                    }

                    switch (obj.flags.ddbimporter.dndbeyond.type)
                    {
                        case "Mount":
                            //skip mounts
                            continue;
                        case "Adventuring Gear":
                            mundane.Objects.Add(obj);
                            break;
                        case "Vehicle":
                            transport.Objects.Add(obj);
                            break;
                        default:
                            loot.Objects.Add(obj);
                            break;
                    }
                }
                else if (obj.type == "consumable")
                {
                    switch (obj.data.consumableType)
                    {
                        case "ammo":
                            ammo.Objects.Add(obj);
                            break;
                        case "potion":
                            consumable.Objects.Add(obj);
                            break;
                        default:
                            toSort.Objects.Add(obj);
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
                            armory.Objects.Add(obj);
                            continue;
                        }
                    }
                    catch
                    {
                    }

                    if (obj.type == "weapon")
                    {
                        armory.Objects.Add(obj);
                    }
                    else
                    {
                        mundane.Objects.Add(obj);
                    }
                }
            }

            module.Packs.Add(magic);
            module.Packs.Add(magicArmor);
            module.Packs.Add(magicWeapons);
            module.Packs.Add(magicConsumable);
            module.Packs.Add(loot);
            module.Packs.Add(ammo);
            module.Packs.Add(consumable);
            module.Packs.Add(transport);
            module.Packs.Add(mundane);
            module.Packs.Add(armory);
            module.Packs.Add(toSort);
            module.Packs.Add(all);

            return module;
        }
    }
}
