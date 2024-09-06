﻿using WDBJsonTool.Support;

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
            Console.WriteLine($"Total records: {wdbVars.TotalRecordCount}");
            Console.WriteLine("");

            Console.WriteLine("Building records....");
            Console.WriteLine("");
            Thread.Sleep(1000);

            RecordsConversion.ConvertRecords(wdbVars);
        }
    }
}