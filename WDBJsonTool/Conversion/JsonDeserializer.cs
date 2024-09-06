using System.Text;
using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.Conversion
{
    internal class JsonDeserializer
    {
        public static void DeserializeData(string inJsonFile, WDBVariables wdbVars)
        {
            var jsonData = File.ReadAllBytes(inJsonFile);
            wdbVars.WDBName = Path.Combine(Path.GetDirectoryName(inJsonFile), Path.GetFileNameWithoutExtension(inJsonFile) + ".wdb");

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
            wdbVars.TotalRecordCount++;

            if (jsonReader.GetString() != wdbVars.SheetNameSectionName)
            {
                SharedMethods.ErrorExit($"Missing {wdbVars.SheetNameSectionName} property at expected position");
            }

            JsonMethods.CheckJsonTokenType("String", ref jsonReader, wdbVars.SheetNameSectionName);

            wdbVars.SheetName = jsonReader.GetString();

            if (wdbVars.SheetName == wdbVars.SheetNameSectionName)
            {
                wdbVars.TotalRecordCount--;
                wdbVars.SheetName = "Not Specified";
            }
            else
            {
                wdbVars.SheetName += "\0";
                wdbVars.SheetNameData = Encoding.UTF8.GetBytes(wdbVars.SheetName);
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
                wdbVars.TotalRecordCount += 3;

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

                wdbVars.StrArrayInfoData = new byte[4];
                wdbVars.StrArrayInfoData[2] = wdbVars.OffsetsPerValue;
                wdbVars.StrArrayInfoData[3] = wdbVars.BitsPerOffset;
            }


            // Check and determine how to parse
            // strtypelist
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "isStrTypelistV1");
            wdbVars.TotalRecordCount++;

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
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "hasTypelist");

            if (jsonReader.GetString() != "hasTypelist")
            {
                SharedMethods.ErrorExit("Missing hasTypelist property at expected position");
            }

            JsonMethods.CheckJsonTokenType("Bool", ref jsonReader, "hasTypelist");

            wdbVars.HasTypelistSection = jsonReader.GetBoolean();

            if (wdbVars.HasTypelistSection)
            {
                wdbVars.TotalRecordCount++;

                // Get typelist values
                JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.TypelistSectionName);
                JsonMethods.CheckJsonTokenType("Array", ref jsonReader, wdbVars.TypelistSectionName);

                var typelistValues = JsonMethods.GetNumbersFromArrayProperty(ref jsonReader, wdbVars.TypelistSectionName);

                wdbVars.TypelistData = new byte[typelistValues.Count * 4];
                wdbVars.TypelistData = SharedMethods.CreateArrayFromIntList(typelistValues, 4);
            }


            // Get version
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, wdbVars.VersionSectionName);
            wdbVars.TotalRecordCount++;

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
            wdbVars.TotalRecordCount += 2;

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

            wdbVars.TotalRecordCount += wdbVars.RecordCount;


            // Determine whether there is
            // a string section
            if (!wdbVars.HasStringSection)
            {
                if (wdbVars.HasStrArraySection)
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.TotalRecordCount++;
                }
                else if (wdbVars.StrtypelistValues.Contains(2))
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.TotalRecordCount++;
                }
                else
                {
                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        if (wdbVars.Fields[f].StartsWith("s"))
                        {
                            wdbVars.HasStringSection = true;
                            wdbVars.TotalRecordCount++;
                            break;
                        }
                    }
                }
            }
        }


        private static void DeserializeRecords(ref Utf8JsonReader jsonReader, WDBVariables wdbVars)
        {
            // Get record values
            JsonMethods.CheckJsonTokenType("PropertyName", ref jsonReader, "records");
            JsonMethods.CheckJsonTokenType("Array", ref jsonReader, "records");

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