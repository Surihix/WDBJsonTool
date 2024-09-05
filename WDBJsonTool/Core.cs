using WDBJsonTool.Conversion;
using WDBJsonTool.Extraction;
using WDBJsonTool.Support;

namespace WDBJsonTool
{
    internal class Core
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");

            if (args.Length == 0)
            {
                SharedMethods.ErrorExit("Enough arguments not specified. please launch the program with -? switch for more info.");
            }

            if (args[0] == "-?")
            {
                Console.WriteLine("Tool actions:");
                Console.WriteLine("WDBJsonTool -? = Display this help page");
                Console.WriteLine("WDBJsonTool \"auto_clip.wdb\" = Extracts the wdb data into a new json file.");
                Console.WriteLine("WDBJsonTool \"auto_clip.json\" = Converts the wdb data in the json file, to a new wdb file.");
                Console.WriteLine("");
                Console.WriteLine("Examples:");
                Console.WriteLine("WDBJsonTool.exe -?");
                Console.WriteLine("WDBJsonTool.exe \"auto_clip.wdb\"");
                Console.WriteLine("WDBJsonTool.exe \"auto_clip.json\"");

                Environment.Exit(0);
            }

            var inFile = args[0];

            if (!File.Exists(inFile))
            {
                SharedMethods.ErrorExit("Specified WDB or json file is missing");
            }

            switch (Path.GetExtension(inFile))
            {
                case ".wdb":
                    ExtractionMain.StartExtraction(inFile);
                    break;

                case ".json":
                    ConversionMain.StartConversion(inFile);
                    break;
            }

            Environment.Exit(0);
        }
    }
}