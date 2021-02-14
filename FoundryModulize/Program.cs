using System;
using System.Threading.Tasks;

namespace FoundryModulize
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var testRoot = @"C:\Users\recto\Desktop\FoundryTest";

            var dataRoot = @"C:\Users\recto\Desktop\FoundryTest\Data";

            var outputRoot = @"C:\Users\recto\Desktop\FoundryTest\out";

            var modulizer = new Modulizer(testRoot, dataRoot, outputRoot);

            await modulizer.PackageMonsters("monsters.db");

            //await modulizer.PackageSpells("spells.db");

            //await modulizer.SplitOutItems("items.db");

            return;

            Console.ReadLine();
        }
    }
}
