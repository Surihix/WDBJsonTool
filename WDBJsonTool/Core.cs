using WDBJsonTool.Extraction;
using WDBJsonTool.Support;

namespace WDBJsonTool
{
    internal class Core
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");

            if (args.Length < 1)
            {
                SharedMethods.ErrorExit("Enough arguments not specified. please launch the program with -h or -? switch.");
            }

            if (args[0] == "-h" || args[0] == "-?")
            {
                Console.WriteLine("Tool actions:");
                Console.WriteLine("-e = Extracts wdb data to a json file");
                Console.WriteLine("-c = Converts the extracted data from json file to wdb file");
                Console.WriteLine("");
                Console.WriteLine("Examples:");
                Console.WriteLine("WDBJsonTool.exe -?");
                Console.WriteLine("WDBJsonTool.exe -h");
                Console.WriteLine("WDBJsonTool.exe -e \"auto_clip.wdb\"");
                Console.WriteLine("WDBJsonTool.exe -c \"auto_clip.json\"");

                Environment.Exit(0);
            }

            if (Enum.TryParse(args[0].Replace("-", ""), out ActionSwitches actionSwitch) == false)
            {
                SharedMethods.ErrorExit("Specified tool action was invalid. please launch the program with -h or -? switch.");
            }

            if (args.Length > 2)
            {
                SharedMethods.ErrorExit("WDB or json file is not specified");
            }

            var inFile = args[1];

            if (!File.Exists(inFile))
            {
                SharedMethods.ErrorExit("Specified WDB or json file is missing");
            }


            switch (actionSwitch)
            {
                case ActionSwitches.e:
                    ExtractionMain.StartExtraction(inFile);
                    break;

                case ActionSwitches.c:
                    break;
            }

            Environment.Exit(0);
        }

        enum ActionSwitches
        {
            e,
            c
        }
    }
}