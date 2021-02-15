using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoundryModulize.Libs.Models;
using Newtonsoft.Json;

namespace FoundryModulize.Libs
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
                portraitImg = (string)obj.img;
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
                    tokenImg = (string)obj.token.img;
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

                    File.Copy(imgPath, path);
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
    }
}
