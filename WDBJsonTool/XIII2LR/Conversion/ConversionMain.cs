namespace WDBJsonTool.XIII2LR.Conversion
{
    internal class ConversionMain
    {
        public static void StartConversion(string inJsonFile)
        {
            var wdbVars = new WDBVariablesXIII2LR();

            JsonDeserializer.DeserializeData(inJsonFile, wdbVars);

            Console.WriteLine("");
            Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            Console.WriteLine($"Total records (with sections): {wdbVars.RecordCountWithSections}");
            Console.WriteLine("");

            wdbVars.WDBFilePath = Path.Combine(Path.GetDirectoryName(inJsonFile), Path.GetFileNameWithoutExtension(inJsonFile) + ".wdb");

            if (wdbVars.HasStrArraySection)
            {
                RecordsConversion.ConvertRecordsStrArray(wdbVars);
            }
            else
            {
                RecordsConversion.ConvertRecords(wdbVars);
            }


            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Building wdb file....");

            WDBbuilder.BuildWDB(wdbVars);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished building wdb file for extracted json data");
        }
    }
}