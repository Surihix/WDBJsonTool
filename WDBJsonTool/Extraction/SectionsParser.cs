using System.Text;
using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.Extraction
{
    internal class SectionsParser
    {
        public static void MainSections(BinaryReader wdbReader, WDBVariables wdbVars)
        {
            // Parse main sections
            long currentSectionNamePos = 16;
            string sectioNameRead;

            wdbVars.StrtypelistData = new byte[] { };
            wdbVars.StructItemData = new byte[] { };
            wdbVars.FieldCount = 0;


            while (true)
            {
                wdbReader.BaseStream.Position = currentSectionNamePos;
                sectioNameRead = wdbReader.ReadBytesString(16, false);

                // Break the loop if its
                // not a valid "!" section
                if (!sectioNameRead.StartsWith("!"))
                {
                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    break;
                }

                // !!sheetname
                if (sectioNameRead == wdbVars.SheetNameSectionName)
                {
                    _ = wdbReader.BaseStream.Position = wdbReader.ReadBytesUInt32(true);
                    wdbVars.SheetName = wdbReader.ReadStringTillNull();
                    wdbVars.RecordCount--;
                }

                // !!strArray
                if (sectioNameRead == wdbVars.StrArraySectionName)
                {
                    wdbVars.HasStrArraySection = true;

                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    StrArrayParser.SubSections(wdbReader, wdbVars);
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
                    wdbVars.parseStrtypelistAsV1 = true;
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!strtypelistb
                if (sectioNameRead == wdbVars.StrtypelistbSectionName)
                {
                    wdbVars.parseStrtypelistAsV1 = false;
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!typelist
                if (sectioNameRead == wdbVars.TypelistSectionName)
                {
                    wdbVars.hasTypelistSection = true;
                    wdbVars.TypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!version
                if (sectioNameRead == wdbVars.VersionSectionName)
                {
                    wdbVars.VersionData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !structitem
                if (sectioNameRead == wdbVars.StructItemSectionName)
                {
                    wdbVars.StructItemData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !structitemnum
                if (sectioNameRead == wdbVars.StructItemNumSectionName)
                {
                    wdbVars.FieldCount = BitConverter.ToUInt32(SharedMethods.SaveSectionData(wdbReader, true), 0);
                    wdbVars.RecordCount--;
                }

                currentSectionNamePos += 32;
            }


            // Check if the important 
            // sections are all parsed
            var imptSectionsParsed = wdbVars.StrtypelistData.Length != 0 && wdbVars.StructItemData.Length != 0 && wdbVars.FieldCount != 0;

            if (!imptSectionsParsed)
            {
                SharedMethods.ErrorExit("Necessary sections were unable to be processed correctly.");
            }

            if (wdbVars.SheetName == "" || wdbVars.SheetName == null)
            {
                wdbVars.SheetName = wdbVars.SheetNameSectionName;
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            Console.WriteLine("");
            Console.WriteLine("");


            // Process !structitem data
            wdbVars.Fields = new string[wdbVars.FieldCount];
            var stringStartPos = 0;

            for (int sf = 0; sf < wdbVars.FieldCount; sf++)
            {
                var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StructItemData, stringStartPos);

                if (derivedString == "")
                {
                    wdbVars.Fields[sf] = "{null}";
                }
                else
                {
                    wdbVars.Fields[sf] = derivedString;
                }

                stringStartPos += Encoding.UTF8.GetByteCount(derivedString) + 1;
            }


            // Process strArray sections
            // data
            if (wdbVars.HasStrArraySection)
            {
                Console.WriteLine($"Organizing {wdbVars.StrArraySectionName} data....");

                StrArrayParser.ArrangeArrayData(wdbVars);

                Console.WriteLine("");
                Console.WriteLine("");
            }
        }


        public static void MainSectionsToJson(WDBVariables wdbVars, Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteNumber("recordCount", wdbVars.RecordCount);
            jsonWriter.WriteString(wdbVars.SheetNameSectionName, wdbVars.SheetName);
            jsonWriter.WriteBoolean("hasStrArray", wdbVars.HasStrArraySection);


            // Write array info values
            // if strArray section is
            // present
            if (wdbVars.HasStrArraySection)
            {
                jsonWriter.WriteNumber("bitsPerOffset", wdbVars.BitsPerOffset);
                jsonWriter.WriteNumber("offsetsPerValue", wdbVars.OffsetsPerValue);
            }


            // Parse and write the strtypelistData
            jsonWriter.WriteBoolean("isStrTypelistV1", wdbVars.parseStrtypelistAsV1);

            if (wdbVars.parseStrtypelistAsV1)
            {
                jsonWriter.WriteStartArray(wdbVars.StrtypelistSectionName);
            }
            else
            {
                jsonWriter.WriteStartArray(wdbVars.StrtypelistbSectionName);
            }

            var strtypelistbIndex = 0;
            var currentStrtypelistData = new byte[4];
            var strtypelistIndexAdjust = wdbVars.parseStrtypelistAsV1 ? 4 : 1;
            var strTypelistValueCount = wdbVars.parseStrtypelistAsV1 ? wdbVars.StrtypelistData.Length / 4 : wdbVars.StrtypelistData.Length;
            int strtypelistValue;

            for (int s = 0; s < strTypelistValueCount; s++)
            {
                if (wdbVars.parseStrtypelistAsV1)
                {
                    Array.ConstrainedCopy(wdbVars.StrtypelistData, strtypelistbIndex, currentStrtypelistData, 0, 4);
                    Array.Reverse(currentStrtypelistData);
                    strtypelistValue = (int)BitConverter.ToUInt32(currentStrtypelistData, 0);
                }
                else
                {
                    strtypelistValue = wdbVars.StrtypelistData[strtypelistbIndex];
                }

                wdbVars.StrtypelistValues.Add(strtypelistValue);
                jsonWriter.WriteNumberValue(strtypelistValue);
                strtypelistbIndex += strtypelistIndexAdjust;
            }

            jsonWriter.WriteEndArray();


            // Write all the typelist data
            jsonWriter.WriteBoolean("hasTypelist", wdbVars.hasTypelistSection);
            if (wdbVars.hasTypelistSection)
            {
                jsonWriter.WriteStartArray(wdbVars.TypelistSectionName);

                var typelistbIndex = 0;
                var currentTypelistData = new byte[4];
                int typelistValue;

                for (int t = 0; t < wdbVars.TypelistData.Length / 4; t++)
                {
                    Array.ConstrainedCopy(wdbVars.TypelistData, typelistbIndex, currentTypelistData, 0, 4);
                    Array.Reverse(currentTypelistData);
                    typelistValue = (int)BitConverter.ToUInt32(currentTypelistData, 0);

                    jsonWriter.WriteNumberValue(typelistValue);
                    typelistbIndex += 4;
                }

                jsonWriter.WriteEndArray();
            }


            // Write version data
            jsonWriter.WriteNumber(wdbVars.VersionSectionName, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true));


            // Write structitem data
            jsonWriter.WriteStartArray(wdbVars.StructItemSectionName);

            for (int i = 0; i < wdbVars.FieldCount; i++)
            {
                jsonWriter.WriteStringValue(wdbVars.Fields[i]);
            }

            jsonWriter.WriteEndArray();
        }
    }
}