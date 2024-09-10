using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.Extraction
{
    internal class RecordsParser
    {
        public static void ProcessRecords(BinaryReader wdbReader, WDBVariables wdbVars, Utf8JsonWriter jsonWriter)
        {
            // Process each record's data
            jsonWriter.WriteStartArray("records");

            var sectionPos = wdbReader.BaseStream.Position;
            string currentRecordName;
            byte[] currentRecordData;
            var strtypelistIndex = 0;
            var currentRecordDataIndex = 0;

            for (int r = 0; r < wdbVars.RecordCount; r++)
            {
                jsonWriter.WriteStartObject();

                _ = wdbReader.BaseStream.Position = sectionPos;
                currentRecordName = wdbReader.ReadBytesString(16, false);

                Console.WriteLine($"Record: {currentRecordName}");
                jsonWriter.WriteString("record", currentRecordName);

                currentRecordData = SharedMethods.SaveSectionData(wdbReader, false);
                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        case 0:
                            var binaryData = BitOperationHelpers.UIntToBinary(SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true));
                            var binaryDataIndex = binaryData.Length;
                            var fieldBitsToProcess = 32;

                            int iTypedataVal;
                            uint uTypeDataVal;
                            string fTypeBinary;
                            float fTypeDataVal;
                            uint strArrayTypeDataVal;
                            string strArrayTypeDictKey;
                            List<string> strArrayTypeDictList;

                            while (fieldBitsToProcess != 0 && f < wdbVars.FieldCount)
                            {
                                var fieldType = wdbVars.Fields[f].Substring(0, 1);
                                var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                                switch (fieldType)
                                {
                                    // sint
                                    case "i":
                                        if (fieldNum == 0)
                                        {
                                            iTypedataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {iTypedataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], iTypedataVal);

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            iTypedataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {iTypedataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], iTypedataVal);

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // uint 
                                    case "u":
                                        if (fieldNum == 0)
                                        {
                                            uTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {uTypeDataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], uTypeDataVal);

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            uTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {uTypeDataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], uTypeDataVal);

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // float (dump as binary) 
                                    case "f":
                                        if (fieldNum == 0)
                                        {
                                            fTypeBinary = binaryData.Substring(binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeBinary}");
                                            jsonWriter.WriteString(wdbVars.Fields[f], fTypeBinary);

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            fTypeBinary = binaryData.Substring(binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeBinary}");
                                            jsonWriter.WriteString(wdbVars.Fields[f], fTypeBinary);

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    //// float 
                                    //case "f":
                                    //    if (fieldNum == 0)
                                    //    {
                                    //        fTypeDataVal = BitOperationHelpers.BinaryToFloat(binaryData, binaryDataIndex - 32, 32);
                                    //        fieldBitsToProcess = 0;

                                    //        Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                    //        jsonWriter.WriteNumber(wdbVars.Fields[f], fTypeDataVal);

                                    //        break;
                                    //    }
                                    //    if (fieldNum > fieldBitsToProcess)
                                    //    {
                                    //        f--;
                                    //        fieldBitsToProcess = 0;
                                    //        continue;
                                    //    }
                                    //    else
                                    //    {
                                    //        binaryDataIndex -= fieldNum;

                                    //        fTypeDataVal = BitOperationHelpers.BinaryToFloat(binaryData, binaryDataIndex, fieldNum);
                                    //        fieldBitsToProcess -= fieldNum;

                                    //        Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                    //        jsonWriter.WriteNumber(wdbVars.Fields[f], fTypeDataVal);

                                    //        if (fieldBitsToProcess != 0)
                                    //        {
                                    //            f++;
                                    //        }
                                    //    }
                                    //    break;

                                    // (s#) strArray item index
                                    case "s":
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            strArrayTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            strArrayTypeDictKey = wdbVars.Fields[f];
                                            strArrayTypeDictList = wdbVars.StrArrayDict[strArrayTypeDictKey];

                                            Console.WriteLine($"{strArrayTypeDictKey}: {strArrayTypeDictList[(int)strArrayTypeDataVal]}");
                                            jsonWriter.WriteString(strArrayTypeDictKey, strArrayTypeDictList[(int)strArrayTypeDataVal]);

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;
                                }
                            }

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // float value
                        case 1:
                            var floatDataVal = SharedMethods.DeriveFloatFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {floatDataVal}");
                            jsonWriter.WriteNumber(wdbVars.Fields[f], floatDataVal);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {derivedString}");
                            jsonWriter.WriteString(wdbVars.Fields[f], derivedString);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // uint
                        case 3:
                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {uintDataVal}");
                            jsonWriter.WriteNumber(wdbVars.Fields[f], uintDataVal);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;
                    }
                }

                jsonWriter.WriteEndObject();

                Console.WriteLine("");

                strtypelistIndex = 0;
                currentRecordDataIndex = 0;
                sectionPos += 32;
            }

            jsonWriter.WriteEndArray();
        }
    }
}