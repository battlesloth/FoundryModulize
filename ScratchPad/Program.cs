using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FoundryModulize.Core;
using Newtonsoft.Json;

namespace ScratchPad
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var path = AppDomain.CurrentDomain.BaseDirectory;

            var catPath = path.Replace(@"ScratchPad\bin\Debug\net5.0\", "") + "Catalogs";

            var catalog = new Catalog();

            await catalog.GenerateCatalog("sulex-spells", @"C:\Users\recto\Desktop\Modules\sulex-spells\packs", catPath);

            return;

            var regex = new Regex(@"^\d+/\d+$");
            var dict = new Dictionary<string, string>();
            var result = new List<monster>();

            var lines = File.ReadAllLines(@"C:\Users\recto\Desktop\FoundryTest\npc_generic.txt");

           



            foreach (var item in lines)
            {
                
                result.Add(JsonConvert.DeserializeObject<monster>(item));
            }
            Console.WriteLine(result.Count);
            var sorted = result.Distinct().Select(JsonConvert.SerializeObject).ToList();
            
            Console.WriteLine(sorted.Count);
            File.WriteAllLines(@"C:\Users\recto\Desktop\FoundryTest\npc_named.txt", sorted);

            Console.ReadLine();
        }

        class monster
        {
            public string name { get; set; }
            public string source { get; set; }
        }
    }
}
