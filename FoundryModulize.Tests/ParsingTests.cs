
using FoundryModulize.Core.Models;
using Newtonsoft.Json;
using Xunit;

namespace FoundryModulize.Tests
{
    public class ParsingTests
    {
        [Fact]
        public void ManifestJsonTest()
        {
            var expected =
                @"{""name"":""this-is-a-test"",""title"":""This is a Test"",""description"":""A manifest testiny"",""author"":""Ima Test"",""version"":""0.0.1"",""minimumCoreVersion"":""0.7.0"",""compatibleCoreVersion"":null,""packs"":[]}";

            
             var manifest = new Manifest
            {
                
                Name = "Thi&s is a Te<st",
                Title = "This is a Test",
                Description = "A manifest testiny",
                Author = "Ima Test",
                Version = "0.0.1",
                MinCoreVersion = "0.7.0"
            };

            var json = JsonConvert.SerializeObject(manifest);

            Assert.Equal(expected, json);
        }
    }
}
