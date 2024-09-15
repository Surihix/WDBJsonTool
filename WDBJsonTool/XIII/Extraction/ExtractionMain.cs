﻿using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.XIII.Extraction
{
    internal class ExtractionMain
    {
        public static void StartExtraction(string inWDBfile, bool shouldIgnoreKnown)
        {
            var wdbVars = new WDBVariables
            {
                IgnoreKnown = shouldIgnoreKnown
            };

            using (var wdbReader = new BinaryReader(File.Open(inWDBfile, FileMode.Open, FileAccess.Read)))
            {
                wdbVars.WDBName = Path.GetFileNameWithoutExtension(inWDBfile);
                wdbVars.JsonFilePath = Path.Combine(Path.GetDirectoryName(inWDBfile), wdbVars.WDBName + ".json");

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


                using (var jsonStream = new MemoryStream())
                {
                    var options = new JsonWriterOptions
                    {
                        Indented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    using (var jsonWriter = new Utf8JsonWriter(jsonStream, options))
                    {
                        jsonWriter.WriteStartObject();

                        SectionsParser.MainSectionsToJson(wdbVars, jsonWriter);

                        Console.WriteLine("Parsing records....");
                        Console.WriteLine("");
                        Thread.Sleep(1000);

                        if (wdbVars.IsKnown)
                        {
                            RecordsParser.ParseRecordsWithFields(wdbReader, wdbVars, jsonWriter);
                        }
                        else
                        {
                            RecordsParser.ParseRecordsWithoutFields(wdbReader, wdbVars, jsonWriter);
                        }

                        jsonWriter.WriteEndObject();
                    }

                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Writing wdb data to json file....");

                    if (File.Exists(wdbVars.JsonFilePath))
                    {
                        File.Delete(wdbVars.JsonFilePath);
                    }

                    jsonStream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(wdbVars.JsonFilePath, jsonStream.ToArray());
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished extracting wdb data to json file");
        }
    }
}