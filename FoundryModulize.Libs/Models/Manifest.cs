using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FoundryModulize.Libs.Models
{
    public class Manifest
    {
        private string name;

        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set => MinifyName(value);
        }
        
        [JsonProperty("title")] 
        public string Title { get; set; }
        
        [JsonProperty("description")] 
        public string Description { get; set; }
        
        [JsonProperty("author")] 
        public string Author { get; set; }
        
        [JsonProperty("version")] 
        public string Version { get; set; }
        
        [JsonProperty("minimumCoreVersion")] 
        public string MinCoreVersion { get; set; }
        
        [JsonProperty("compatibleCoreVersion")]
        public string CompatibleCoreVersion { get; set; }
        
        [JsonProperty("packs")] 
        public List<Pack> Packs { get; set; }

        public Manifest()
        {
            Packs = new List<Pack>();
        }

        private void MinifyName(string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 _-]");
            var cleaned = rgx.Replace(value, "");
            name = cleaned.ToLower().Replace(' ', '-');
        }

    }
}
