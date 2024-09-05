using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.Conversion
{
    internal class JsonDeserializer
    {
        public static void DeserializeData(string inJsonFile, WDBVariables wdbVars)
        {
            var jsonData = File.ReadAllBytes(inJsonFile);

            var options = new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            var jsonReader = new Utf8JsonReader(jsonData, options);
            _ = jsonReader.Read();

            Console.WriteLine("Deserializing main sections....");
            Console.WriteLine("");
            DeserializeMainSections(ref jsonReader, wdbVars);

            Console.WriteLine("Deserializing records....");
            Console.WriteLine("");
            DeserializeRecords(ref jsonReader, wdbVars);
        }


        private static void DeserializeMainSections(ref Utf8JsonReader jsonReader, WDBVariables wdbVars)
        {
            // Get recordCount
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "recordCount");

            if (jsonReader.GetString() != "recordCount")
            {
                SharedMethods.ErrorExit("Missing recordCount property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Number", ref jsonReader, "recordCount");

            wdbVars.RecordCount = jsonReader.GetUInt32();


            // Get sheetName
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.SheetNameSectionName);
            wdbVars.CompleteRecordCount++;

            if (jsonReader.GetString() != wdbVars.SheetNameSectionName)
            {
                SharedMethods.ErrorExit($"Missing {wdbVars.SheetNameSectionName} property at expected position");
            }

            JsonMethods.CheckJsonTokenType("String", ref jsonReader, wdbVars.SheetNameSectionName);

            wdbVars.SheetName = jsonReader.GetString();

            if (wdbVars.SheetName == wdbVars.SheetNameSectionName)
            {
                wdbVars.CompleteRecordCount--;

                wdbVars.SheetName = "";
            }


            // Check if strArray is
            // present
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "hasStrArray");

            if (jsonReader.GetString() != "hasStrArray")
            {
                SharedMethods.ErrorExit("Missing hasStrArray property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Bool", ref jsonReader, "hasStrArray");

            wdbVars.HasStrArraySection = jsonReader.GetBoolean();

            if (wdbVars.HasStrArraySection)
            {
                wdbVars.CompleteRecordCount += 3;

                // Get bitsPerOffset value
                JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "bitsPerOffset");

                if (jsonReader.GetString() != "bitsPerOffset")
                {
                    SharedMethods.ErrorExit("Missing bitsPerOffset property at expected position");
                }

                JsonMethods.CheckJsonTokenType("Number", ref jsonReader, "bitsPerOffset");
                wdbVars.BitsPerOffset = jsonReader.GetByte();

                // Get offsetsPerValue value
                JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "offsetsPerValue");

                if (jsonReader.GetString() != "offsetsPerValue")
                {
                    SharedMethods.ErrorExit("Missing offsetsPerValue property at expected position");
                }

                JsonMethods.CheckJsonTokenType("Number", ref jsonReader, "offsetsPerValue");
                wdbVars.OffsetsPerValue = jsonReader.GetByte();
            }


            // Check and determine how to parse
            // strtypelist
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "isStrTypelistV1");
            wdbVars.CompleteRecordCount++;

            if (jsonReader.GetString() != "isStrTypelistV1")
            {
                SharedMethods.ErrorExit("Missing isStrTypelistV1 property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Bool", ref jsonReader, "isStrTypelistV1");

            wdbVars.ParseStrtypelistAsV1 = jsonReader.GetBoolean();

            var strtypelistSecNameProcess = wdbVars.ParseStrtypelistAsV1 ? wdbVars.StrtypelistSectionName : wdbVars.StrtypelistbSectionName;
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, strtypelistSecNameProcess);

            if (jsonReader.GetString() != strtypelistSecNameProcess)
            {
                SharedMethods.ErrorExit($"Missing {strtypelistSecNameProcess} property at expected position");
            }


            // Get strtypelist values
            JsonMethods.CheckJsonTokenType("Array", ref jsonReader, strtypelistSecNameProcess);

            wdbVars.StrtypelistValues = JsonMethods.GetNumbersFromArrayProperty(ref jsonReader, strtypelistSecNameProcess);


            // Check if typelist is
            // present 
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "hasTypelist");

            if (jsonReader.GetString() != "hasTypelist")
            {
                SharedMethods.ErrorExit("Missing hasTypelist property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Bool", ref jsonReader, "hasTypelist");

            wdbVars.HasTypelistSection = jsonReader.GetBoolean();

            if (wdbVars.HasTypelistSection)
            {
                wdbVars.CompleteRecordCount++;

                // Get typelist values
                JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.TypelistSectionName);
                JsonMethods.CheckJsonTokenType("Array", ref jsonReader, wdbVars.TypelistSectionName);

                var typelistValues = JsonMethods.GetNumbersFromArrayProperty(ref jsonReader, wdbVars.TypelistSectionName);

                wdbVars.TypelistData = new byte[typelistValues.Count * 4];
                byte[] currentValue;
                var index = 0;

                for (int i = 0; i < typelistValues.Count; i++)
                {
                    currentValue = BitConverter.GetBytes(typelistValues[i]);

                    wdbVars.TypelistData[index] = currentValue[3];
                    wdbVars.TypelistData[index + 1] = currentValue[2];
                    wdbVars.TypelistData[index + 2] = currentValue[1];
                    wdbVars.TypelistData[index + 3] = currentValue[0];

                    index += 4;
                }
            }


            // Get version
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.VersionSectionName);
            wdbVars.CompleteRecordCount++;

            if (jsonReader.GetString() != wdbVars.VersionSectionName)
            {
                SharedMethods.ErrorExit($"Missing {wdbVars.VersionSectionName} property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Number", ref jsonReader, wdbVars.VersionSectionName);

            wdbVars.VersionData = BitConverter.GetBytes(jsonReader.GetUInt32());
            Array.Reverse(wdbVars.VersionData);


            // Get structitem values
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.StructItemSectionName);
            JsonMethods.CheckJsonTokenType("Array", ref jsonReader, wdbVars.StructItemSectionName);
            wdbVars.CompleteRecordCount += 2;

            wdbVars.Fields = JsonMethods.GetStringsFromArrayProperty(ref jsonReader, wdbVars.StructItemSectionName).ToArray();
            wdbVars.FieldCount = (uint)wdbVars.Fields.Length;

            wdbVars.CompleteRecordCount += wdbVars.RecordCount;
        }


        private static void DeserializeRecords(ref Utf8JsonReader jsonReader, WDBVariables wdbVars)
        {
            // Get record values
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "records");
            JsonMethods.CheckJsonTokenType("Array", ref jsonReader, "records");

            var recordName = string.Empty;
            var hasStringSection = false;

            for (int i = 0; i < wdbVars.RecordCount; i++)
            {
                // Read start object
                _ = jsonReader.Read();

                if (jsonReader.TokenType != JsonTokenType.StartObject)
                {
                    if (recordName == "")
                    {
                        SharedMethods.ErrorExit("The record array does not begin with a valid start object character");
                    }
                    else
                    {
                        SharedMethods.ErrorExit($"A valid start object character was not present after this record {recordName}");
                    }
                }

                // Get record name
                _ = jsonReader.Read();
                _ = jsonReader.Read();

                recordName = jsonReader.GetString();
                var currentDataList = new List<object>();

                // Get record data
                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    _ = jsonReader.Read();
                    if (jsonReader.GetString().StartsWith("s"))
                    {
                        // Determine whether to
                        // increase record count
                        // if string section is
                        // present
                        if (!hasStringSection)
                        {
                            hasStringSection = true;
                            wdbVars.CompleteRecordCount++;
                        }

                        _ = jsonReader.Read();
                        currentDataList.Add(jsonReader.GetString());
                    }
                    else
                    {
                        _ = jsonReader.Read();
                        currentDataList.Add(jsonReader.GetDecimal());
                    }
                }

                wdbVars.RecordsDataDict.Add(recordName, currentDataList);

                // Read end object
                _ = jsonReader.Read();

                if (jsonReader.TokenType != JsonTokenType.EndObject)
                {
                    SharedMethods.ErrorExit($"A valid end object character was not present after this record {recordName}");
                }
            }
        }
    }
}