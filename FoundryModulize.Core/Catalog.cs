using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FoundryModulize.Core
{
    public class Catalog
    {
        private Dictionary<string, HashSet<string>> catalog;

        public bool IsInACatalog(string itemName, out string category)
        {
            foreach (var key in catalog.Keys)
            {
                if (catalog[key].Contains(itemName))
                {
                    category = key;
                    return true;
                }
            }

            category = string.Empty;
            return false;
        }

        public bool InCatalog(string itemName, string catalogName)
        {
            return catalog.ContainsKey(catalogName) && catalog[catalogName].Contains(itemName);
        }

        public async Task LoadCatalog(string path)
        {
            catalog.Clear();

            var catalogFiles = Directory.EnumerateFiles(path);

            foreach (var catalogFile in catalogFiles)
            {
                var name = Path.GetFileNameWithoutExtension(catalogFile);

                var entries = await File.ReadAllLinesAsync(catalogFile);

                catalog[name] = new HashSet<string>(entries.Select(x => x.Trim()));
            }
        }

        public async Task GenerateCatalog(string moduleName, string catalogName, string packDbPath, string outputPath)
        {
            var output = new List<string>();

            var lines = await File.ReadAllLinesAsync(packDbPath);

            foreach (var val in lines)
            {
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(val, new ExpandoObjectConverter());

                output.Add(obj.name);
            }

            Directory.CreateDirectory(Path.Combine(outputPath, moduleName));

            await File.WriteAllLinesAsync(Path.Combine(outputPath, moduleName, $"{catalogName}.cat"), output);
        }
    }
}
