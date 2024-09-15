namespace WDBJsonTool.XIII.Conversion
{
    internal class ConversionMain
    {
        public static void StartConversion(string inJsonFile)
        {
            var wdbVars = new WDBVariables();

            JsonDeserializer.DeserializeData(inJsonFile, wdbVars);

            Console.WriteLine("");

            if (wdbVars.IsKnown)
            {
                Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            }

            Console.WriteLine($"Total records (with sections): {wdbVars.RecordCountWithSections}");
            Console.WriteLine("");

            Console.WriteLine("Building records....");
            Console.WriteLine("");
            Thread.Sleep(1000);

            wdbVars.WDBFilePath = Path.Combine(Path.GetDirectoryName(inJsonFile), Path.GetFileNameWithoutExtension(inJsonFile) + ".wdb");

            if (wdbVars.IsKnown)
            {
                RecordsConversion.ConvertRecordsWithFields(wdbVars);
            }
            else
            {
                RecordsConversion.ConvertRecordsNoFields(wdbVars);
            }

            WDBbuilder.BuildWDB(wdbVars);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished building wdb file for extracted json data");
        }
    }
}