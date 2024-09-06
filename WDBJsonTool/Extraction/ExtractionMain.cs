using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.Extraction
{
    internal class ExtractionMain
    {
        public static void StartExtraction(string inWDBfile)
        {
            var wdbVars = new WDBVariables();

            using (var wdbReader = new BinaryReader(File.Open(inWDBfile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                wdbVars.WDBName = Path.GetFileNameWithoutExtension(inWDBfile);
                wdbVars.JsonName = Path.Combine(Path.GetDirectoryName(inWDBfile), wdbVars.WDBName + ".json");

                _ = wdbReader.BaseStream.Position = 0;
                if (wdbReader.ReadBytesString(3, false) != "WPD")
                {
                    SharedMethods.ErrorExit("Not a valid WPD file");
                }

                _ = wdbReader.BaseStream.Position += 1;
                wdbVars.RecordCount = wdbReader.ReadBytesUInt32(true);

                if (wdbVars.RecordCount == 0)
                {
                    SharedMethods.ErrorExit("No records/sections are present in this file");
                }

                SectionsParser.MainSections(wdbReader, wdbVars);

                Console.WriteLine("");
                Console.WriteLine($"Total records: {wdbVars.RecordCount}");
                Console.WriteLine("");

                Console.WriteLine("Parsing records....");
                Console.WriteLine("");
                Thread.Sleep(1000);


                using (var jsonStream = new MemoryStream())
                {
                    var options = new JsonWriterOptions
                    {
                        Indented = true
                    };

                    using (var jsonWriter = new Utf8JsonWriter(jsonStream, options))
                    {
                        jsonWriter.WriteStartObject();

                        SectionsParser.MainSectionsToJson(wdbVars, jsonWriter);
                        RecordsParser.ProcessRecords(wdbReader, wdbVars, jsonWriter);

                        jsonWriter.WriteEndObject();
                    }
                  
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Writing wdb data to json file....");

                    if (File.Exists(wdbVars.JsonName))
                    {
                        File.Delete(wdbVars.JsonName);
                    }

                    jsonStream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(wdbVars.JsonName, jsonStream.ToArray());
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished extracting wdb data to json file");
        }
    }
}