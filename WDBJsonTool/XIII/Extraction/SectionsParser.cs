﻿using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.XIII.Extraction
{
    internal class SectionsParser
    {
        public static void MainSections(BinaryReader wdbReader, WDBVariables wdbVars)
        {
            // Parse main sections
            long currentSectionNamePos = 16;
            string sectioNameRead;

            while (true)
            {
                wdbReader.BaseStream.Position = currentSectionNamePos;
                sectioNameRead = wdbReader.ReadBytesString(16, false);

                // Break the loop if its
                // not a valid "!" section
                if (!sectioNameRead.StartsWith("!!"))
                {
                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    break;
                }

                // !!sheetname check
                if (sectioNameRead == "!!sheetname")
                {
                    SharedMethods.ErrorExit("Specified WDB file is from XIII-2 or LR. set the gamecode to -ff132 to extract this file.");
                }

                // !!strArray check
                if (sectioNameRead == "!!strArray")
                {
                    SharedMethods.ErrorExit("Specified WDB file is from XIII-2 or LR. set the gamecode to -ff132 to extract this file.");
                }

                // !!string
                if (sectioNameRead == wdbVars.StringSectionName)
                {
                    wdbVars.HasStringSection = true;

                    wdbVars.StringsData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!strtypelist
                if (sectioNameRead == wdbVars.StrtypelistSectionName)
                {
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);

                    if (wdbVars.StrtypelistData.Length != 0)
                    {
                        wdbVars.StrtypelistValues = SharedMethods.GetSectionDataValues(wdbVars.StrtypelistData);
                        wdbVars.FieldCount = (uint)wdbVars.StrtypelistValues.Count;
                    }

                    wdbVars.RecordCount--;
                }

                // !!typelist
                if (sectioNameRead == wdbVars.TypelistSectionName)
                {
                    wdbVars.TypelistData = SharedMethods.SaveSectionData(wdbReader, false);

                    if (wdbVars.TypelistData.Length != 0)
                    {
                        wdbVars.TypelistValues = SharedMethods.GetSectionDataValues(wdbVars.TypelistData);
                    }

                    wdbVars.RecordCount--;
                }

                // !!version
                if (sectioNameRead == wdbVars.VersionSectionName)
                {
                    wdbVars.VersionData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                currentSectionNamePos += 32;
            }

            // Check if the !!strtypelist
            // is parsed 
            if (wdbVars.StrtypelistData.Length == 0)
            {
                SharedMethods.ErrorExit("!!strtypelist section was not present in the file.");
            }
        }


        public static void MainSectionsToJson(WDBVariables wdbVars, Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteNumber(JsonVariables.RecordCountToken, wdbVars.RecordCount);

            if (WDBDicts.RecordIDs.ContainsKey(wdbVars.WDBName) && !wdbVars.IgnoreKnown)
            {
                wdbVars.IsKnown = true;
                jsonWriter.WriteBoolean(JsonVariables.IsKnownToken, wdbVars.IsKnown);

                wdbVars.SheetName = WDBDicts.RecordIDs[wdbVars.WDBName];
                jsonWriter.WriteString(wdbVars.SheetNameSectionName, wdbVars.SheetName);

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($"sheetName: {wdbVars.SheetName}");
                Console.WriteLine("");
                Console.WriteLine("");

                wdbVars.FieldCount = (uint)WDBDicts.FieldNames[wdbVars.SheetName].Count;
                wdbVars.Fields = new string[wdbVars.FieldCount];

                // Write all of the field names 
                // if the file is fully known
                for (int sf = 0; sf < wdbVars.FieldCount; sf++)
                {
                    var derivedString = WDBDicts.FieldNames[wdbVars.SheetName][sf];
                    wdbVars.Fields[sf] = derivedString;
                }
            }
            else
            {
                jsonWriter.WriteBoolean(JsonVariables.IsKnownToken, wdbVars.IsKnown);
            }


            // Parse and write the strtypelistData
            jsonWriter.WriteStartArray(wdbVars.StrtypelistSectionName);

            var strtypelistIndex = 0;
            var currentStrtypelistData = new byte[4];
            var strTypelistValueCount = wdbVars.StrtypelistData.Length / 4;
            uint strtypelistValue;

            for (int s = 0; s < strTypelistValueCount; s++)
            {
                Array.ConstrainedCopy(wdbVars.StrtypelistData, strtypelistIndex, currentStrtypelistData, 0, 4);
                Array.Reverse(currentStrtypelistData);
                strtypelistValue = BitConverter.ToUInt32(currentStrtypelistData, 0);

                wdbVars.StrtypelistValues.Add(strtypelistValue);
                jsonWriter.WriteNumberValue(strtypelistValue);
                strtypelistIndex += 4;
            }

            jsonWriter.WriteEndArray();


            // Write all the typelist data
            jsonWriter.WriteStartArray(wdbVars.TypelistSectionName);

            var typelistIndex = 0;
            var currentTypelistData = new byte[4];
            int typelistValue;

            for (int t = 0; t < wdbVars.TypelistData.Length / 4; t++)
            {
                Array.ConstrainedCopy(wdbVars.TypelistData, typelistIndex, currentTypelistData, 0, 4);
                Array.Reverse(currentTypelistData);
                typelistValue = (int)BitConverter.ToUInt32(currentTypelistData, 0);

                jsonWriter.WriteNumberValue(typelistValue);
                typelistIndex += 4;
            }

            jsonWriter.WriteEndArray();


            // Write version data
            jsonWriter.WriteNumber(wdbVars.VersionSectionName, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true));


            // Write fields data
            if (wdbVars.IsKnown && !wdbVars.IgnoreKnown)
            {
                jsonWriter.WriteStartArray(wdbVars.StructItemSectionName);

                for (int i = 0; i < wdbVars.FieldCount; i++)
                {
                    jsonWriter.WriteStringValue(wdbVars.Fields[i]);
                }

                jsonWriter.WriteEndArray();
            }
        }
    }
}