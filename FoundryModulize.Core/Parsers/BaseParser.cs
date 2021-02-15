using System.Threading.Tasks;
using FoundryModulize.Core.Models;

namespace FoundryModulize.Core.Parsers
{
    public abstract class BaseParser : IPackParser
    {
        protected string moduleName;
        protected string title;
        protected string description;
        protected string author;
        protected string version;
        protected string minCoreVersion;

        protected Manifest module;

        protected EntityType entity;

        protected BaseParser(string moduleName, string title, string description, string author, string version, string minCoreVersion, EntityType entity)
        {
            this.moduleName = moduleName;
            this.title = title;
            this.description = description;
            this.author = author;
            this.version = version;
            this.minCoreVersion = minCoreVersion;
            this.entity = entity;

            GenerateManifest();
        }

        protected void GenerateManifest()
        {
            module = new Manifest
            {
                Name = moduleName,
                Title = title,
                Description = description,
                Author = author,
                Version = version,
                MinCoreVersion = minCoreVersion
            };
        }

        protected Pack GeneratePack(string name, string label)
        {
            return new Pack
            {
                Name = name,
                Label = label,
                Module = moduleName,
                System = "dnd5e",
                Path = name,
                Entity = entity.ToString()
            };
        }

        public abstract Task<Manifest> ParsePack(string path);

        public abstract Task LoadCatalog(string path);

    }
}
