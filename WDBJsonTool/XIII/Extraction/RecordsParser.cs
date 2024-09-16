﻿using System.Text.Json;
using WDBJsonTool.Support;

namespace WDBJsonTool.XIII.Extraction
{
    internal class RecordsParser
    {
        public static void ParseRecordsWithFields(BinaryReader wdbReader, WDBVariables wdbVars, Utf8JsonWriter jsonWriter)
        {
            // Process each record's data
            jsonWriter.WriteStartArray(JsonVariables.RecordsArrayToken);

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
                jsonWriter.WriteString(JsonVariables.RecordToken, currentRecordName);

                currentRecordData = SharedMethods.SaveSectionData(wdbReader, false);

                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        // bitpacked
                        case 0:
                            var binaryData = BitOperationHelpers.UIntToBinary(SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true));
                            var binaryDataIndex = binaryData.Length;
                            var fieldBitsToProcess = 32;

                            int iTypedataVal;
                            uint uTypeDataVal;
                            int fTypeDataVal;

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

                                    // float (bitpacked as int)
                                    case "f":
                                        if (fieldNum == 0)
                                        {
                                            fTypeDataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], fTypeDataVal);

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

                                            fTypeDataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                            jsonWriter.WriteNumber(wdbVars.Fields[f], fTypeDataVal);

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

                        // uint value
                        case 3:
                            if (wdbVars.Fields[f].StartsWith("u64"))
                            {
                                var processArray = new byte[8];
                                Array.ConstrainedCopy(currentRecordData, currentRecordDataIndex, processArray, 0, 8);
                                Array.Reverse(processArray);

                                var ulTypeDataVal = BitConverter.ToUInt64(processArray, 0);

                                Console.WriteLine($"{wdbVars.Fields[f]}(uint64): {ulTypeDataVal}");
                                jsonWriter.WriteNumber($"{wdbVars.Fields[f]}(uint64)", ulTypeDataVal);

                                strtypelistIndex++;
                                currentRecordDataIndex += 8;
                                break;
                            }

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


        public static void ParseRecordsWithoutFields(BinaryReader wdbReader, WDBVariables wdbVars, Utf8JsonWriter jsonWriter)
        {
            // Process each record's data
            jsonWriter.WriteStartArray(JsonVariables.RecordsArrayToken);

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
                jsonWriter.WriteString(JsonVariables.RecordToken, currentRecordName);

                currentRecordData = SharedMethods.SaveSectionData(wdbReader, false);

                var bitpackedFieldCounter = 0;
                var floatFieldCounter = 0;
                var stringFieldCounter = 0;
                var uintFieldCounter = 0;

                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        // bitpacked
                        case 0:
                            var bitpackedData = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var hexDataVal = "0x" + bitpackedData.ToString("X").PadLeft(8, '0');

                            Console.WriteLine($"bitpacked-field_{bitpackedFieldCounter}: {hexDataVal}");
                            jsonWriter.WriteString($"bitpacked-field_{bitpackedFieldCounter}", hexDataVal);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            bitpackedFieldCounter++;
                            break;

                        // float value
                        case 1:
                            var floatDataVal = SharedMethods.DeriveFloatFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"float-field_{floatFieldCounter}: {floatDataVal}");
                            jsonWriter.WriteNumber($"float-field_{floatFieldCounter}", floatDataVal);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            floatFieldCounter++;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"!!string-field_{stringFieldCounter}: {derivedString}");
                            jsonWriter.WriteString($"!!string-field_{stringFieldCounter}", derivedString);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            stringFieldCounter++;
                            break;

                        // uint value
                        case 3:
                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"uint-field_{uintFieldCounter}: {uintDataVal}");
                            jsonWriter.WriteNumber($"uint-field_{uintFieldCounter}", uintDataVal);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            uintFieldCounter++;
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