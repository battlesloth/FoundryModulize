using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FoundryModulize.Models
{
    public class Pack
    {
        private string name;
        private string path;

        [JsonProperty("name")]
        public string Name { get => name; set => SetName(value); }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("module")]
        public string Module { get; set; }
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("path")]
        public string Path { get => path; set => SetPath(value); }
        [JsonProperty("entity")]
        public string Entity { get; set; }
        [JsonIgnore]
        public List<ExpandoObject> Objects { get; set; }


        public Pack()
        {
            Objects = new List<ExpandoObject>();
        }

        private void SetName(string value)
        {
            name = MinifyString(value);
        }

        private void SetPath(string value)
        {
            var temp = MinifyString(value);

            path = $"./packs/{value}.db";
        }

        private string MinifyString(string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 _-]");
            var cleaned = rgx.Replace(value, "");
            return cleaned.ToLower().Replace(' ', '-');
        }
    }
}
