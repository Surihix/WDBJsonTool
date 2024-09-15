using System.Text;
using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.XIII2LR.Conversion
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
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "recordCount");
            JsonMethods.CheckPropertyName(ref jsonReader, "recordCount");

            JsonMethods.CheckTokenType("Number", ref jsonReader, "recordCount");
            wdbVars.RecordCount = jsonReader.GetUInt32();


            // Get sheetName
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, wdbVars.SheetNameSectionName);
            JsonMethods.CheckPropertyName(ref jsonReader, wdbVars.SheetNameSectionName);

            JsonMethods.CheckTokenType("String", ref jsonReader, wdbVars.SheetNameSectionName);
            wdbVars.SheetName = jsonReader.GetString();

            if (wdbVars.SheetName != "Not Specified")
            {
                wdbVars.RecordCountWithSections++;

                wdbVars.SheetName += "\0";
                wdbVars.SheetNameData = Encoding.UTF8.GetBytes(wdbVars.SheetName);
            }


            // Check if strArray is
            // present
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "hasStrArray");
            JsonMethods.CheckPropertyName(ref jsonReader, "hasStrArray");

            JsonMethods.CheckTokenType("Bool", ref jsonReader, "hasStrArray");
            wdbVars.HasStrArraySection = jsonReader.GetBoolean();

            if (wdbVars.HasStrArraySection)
            {
                wdbVars.RecordCountWithSections += 3;

                // Get bitsPerOffset value
                JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "bitsPerOffset");
                JsonMethods.CheckPropertyName(ref jsonReader, "bitsPerOffset");

                JsonMethods.CheckTokenType("Number", ref jsonReader, "bitsPerOffset");
                wdbVars.BitsPerOffset = jsonReader.GetByte();

                // Get offsetsPerValue value
                JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "offsetsPerValue");
                JsonMethods.CheckPropertyName(ref jsonReader, "offsetsPerValue");

                JsonMethods.CheckTokenType("Number", ref jsonReader, "offsetsPerValue");
                wdbVars.OffsetsPerValue = jsonReader.GetByte();

                wdbVars.StrArrayInfoData = new byte[4];
                wdbVars.StrArrayInfoData[2] = wdbVars.OffsetsPerValue;
                wdbVars.StrArrayInfoData[3] = wdbVars.BitsPerOffset;
            }


            // Check and determine how to parse
            // strtypelist
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "isStrTypelistV1");
            JsonMethods.CheckPropertyName(ref jsonReader, "isStrTypelistV1");

            JsonMethods.CheckTokenType("Bool", ref jsonReader, "isStrTypelistV1");
            wdbVars.ParseStrtypelistAsV1 = jsonReader.GetBoolean();

            var strtypelistSecNameProcess = wdbVars.ParseStrtypelistAsV1 ? wdbVars.StrtypelistSectionName : wdbVars.StrtypelistbSectionName;
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, strtypelistSecNameProcess);
            JsonMethods.CheckPropertyName(ref jsonReader, strtypelistSecNameProcess);

            wdbVars.RecordCountWithSections++;


            // Get strtypelist values
            JsonMethods.CheckTokenType("Array", ref jsonReader, strtypelistSecNameProcess);
            wdbVars.StrtypelistValues = JsonMethods.GetNumbersFromArrayPropertyInt(ref jsonReader, strtypelistSecNameProcess);

            if (wdbVars.ParseStrtypelistAsV1)
            {
                wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count * 4];
                wdbVars.StrtypelistData = SharedMethods.CreateArrayFromIntList(wdbVars.StrtypelistValues, 4);
            }
            else
            {
                wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count];
                wdbVars.StrtypelistData = SharedMethods.CreateArrayFromIntList(wdbVars.StrtypelistValues, 1);
            }


            // Check if typelist is
            // present 
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "hasTypelist");
            JsonMethods.CheckPropertyName(ref jsonReader, "hasTypelist");

            JsonMethods.CheckTokenType("Bool", ref jsonReader, "hasTypelist");
            wdbVars.HasTypelistSection = jsonReader.GetBoolean();

            if (wdbVars.HasTypelistSection)
            {
                wdbVars.RecordCountWithSections++;

                // Get typelist values
                JsonMethods.CheckTokenType("PropertyName", ref jsonReader, wdbVars.TypelistSectionName);
                JsonMethods.CheckTokenType("Array", ref jsonReader, wdbVars.TypelistSectionName);
                var typelistValues = JsonMethods.GetNumbersFromArrayPropertyInt(ref jsonReader, wdbVars.TypelistSectionName);

                wdbVars.TypelistData = new byte[typelistValues.Count * 4];
                wdbVars.TypelistData = SharedMethods.CreateArrayFromIntList(typelistValues, 4);
            }


            // Get version
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, wdbVars.VersionSectionName);
            JsonMethods.CheckPropertyName(ref jsonReader, wdbVars.VersionSectionName);

            JsonMethods.CheckTokenType("Number", ref jsonReader, wdbVars.VersionSectionName);
            wdbVars.VersionData = BitConverter.GetBytes(jsonReader.GetUInt32());
            Array.Reverse(wdbVars.VersionData);

            wdbVars.RecordCountWithSections++;


            // Get structitem values
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, wdbVars.StructItemSectionName);
            JsonMethods.CheckPropertyName(ref jsonReader, wdbVars.StructItemSectionName);

            JsonMethods.CheckTokenType("Array", ref jsonReader, wdbVars.StructItemSectionName);
            wdbVars.RecordCountWithSections += 2;

            wdbVars.Fields = JsonMethods.GetStringsFromArrayProperty(ref jsonReader, wdbVars.StructItemSectionName).ToArray();
            wdbVars.FieldCount = (uint)wdbVars.Fields.Length;
            var structItemsList = new List<byte>();

            for (int i = 0; i < wdbVars.FieldCount; i++)
            {
                structItemsList.AddRange(Encoding.UTF8.GetBytes(wdbVars.Fields[i] + "\0"));
            }

            wdbVars.StructItemData = structItemsList.ToArray();
            wdbVars.StructItemNumData = BitConverter.GetBytes(wdbVars.FieldCount);
            Array.Reverse(wdbVars.StructItemNumData);

            wdbVars.RecordCountWithSections += wdbVars.RecordCount;


            // Determine whether there is
            // a string section
            if (!wdbVars.HasStringSection)
            {
                if (wdbVars.StrtypelistValues.Contains(2))
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.RecordCountWithSections++;
                }
                else if (wdbVars.HasStrArraySection)
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.RecordCountWithSections++;
                }
            }
        }


        private static void DeserializeRecords(ref Utf8JsonReader jsonReader, WDBVariables wdbVars)
        {
            // Get record values
            JsonMethods.CheckTokenType("PropertyName", ref jsonReader, "records");
            JsonMethods.CheckPropertyName(ref jsonReader, "records");

            JsonMethods.CheckTokenType("Array", ref jsonReader, "records");

            var recordName = string.Empty;
            string fieldName;

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

                if (jsonReader.TokenType != JsonTokenType.String)
                {
                    if (recordName == "")
                    {
                        SharedMethods.ErrorExit("The first record's property value does not begin with a valid name");
                    }
                    else
                    {
                        SharedMethods.ErrorExit($"Invalid property value specified for 'record' property. previous record read was {recordName}");
                    }
                }

                recordName = jsonReader.GetString();
                var currentDataList = new List<object>();

                // Get record data
                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    _ = jsonReader.Read();

                    if (jsonReader.TokenType != JsonTokenType.PropertyName)
                    {
                        SharedMethods.ErrorExit($"Field name PropertyType was invalid. occured when parsing {recordName} data.");
                    }

                    fieldName = jsonReader.GetString();

                    if (fieldName.StartsWith("s"))
                    {
                        _ = jsonReader.Read();

                        if (jsonReader.TokenType != JsonTokenType.String)
                        {
                            SharedMethods.ErrorExit($"{fieldName} property's value was invalid. occured when parsing {recordName} data.");
                        }

                        currentDataList.Add(jsonReader.GetString());
                    }
                    else if (fieldName.StartsWith("f") && SharedMethods.DeriveFieldNumber(fieldName) != 0)
                    {
                        _ = jsonReader.Read();

                        if (jsonReader.TokenType != JsonTokenType.String)
                        {
                            SharedMethods.ErrorExit($"{fieldName} property's value was invalid. occured when parsing {recordName} data.");
                        }

                        currentDataList.Add(jsonReader.GetString());
                    }
                    else
                    {
                        _ = jsonReader.Read();

                        if (jsonReader.TokenType != JsonTokenType.Number)
                        {
                            SharedMethods.ErrorExit($"{fieldName} property's value was invalid. occured when parsing {recordName} data.");
                        }

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