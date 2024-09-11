using WDBJsonTool.Support;

namespace WDBJsonTool.Conversion
{
    internal class ConversionMain
    {
        public static void StartConversion(string inJsonFile)
        {
            var wdbVars = new WDBVariables();

            JsonDeserializer.DeserializeData(inJsonFile, wdbVars);

            Console.WriteLine("");
            Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            Console.WriteLine($"Total records (with sections): {wdbVars.RecordCountWithSections}");
            Console.WriteLine("");

            Console.WriteLine("Building records....");
            Console.WriteLine("");
            Thread.Sleep(1000);

            if (wdbVars.HasStrArraySection)
            {
                RecordsConversion.ConvertRecordsStrArray(wdbVars);
                WDBbuilder.BuildWDBStrArray(wdbVars);
            }
            else
            {
                RecordsConversion.ConvertRecords(wdbVars);
                WDBbuilder.BuildWDB(wdbVars);
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished building wdb file for extracted json data");
        }
    }
}