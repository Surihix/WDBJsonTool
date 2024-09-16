﻿using System.Text;
using WDBJsonTool.Support;

namespace WDBJsonTool.XIII2LR.Conversion
{
    internal class RecordsConversion
    {
        public static void ConvertRecordsStrArray(WDBVariables wdbVars)
        {
            // s#Field & strings
            var processedSFieldStringsDict = new Dictionary<string, List<string>>();

            foreach (var recordData in wdbVars.RecordsDataDict)
            {
                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    var fieldType = wdbVars.Fields[f].Substring(0, 1);
                    var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                    if (fieldType == "s" && fieldNum != 0)
                    {
                        var currentSField = wdbVars.Fields[f];

                        if (!processedSFieldStringsDict.ContainsKey(currentSField))
                        {
                            processedSFieldStringsDict.Add(currentSField, new List<string>());
                        }

                        processedSFieldStringsDict[currentSField].Add(recordData.Value[f].ToString());
                    }
                }
            }


            uint stringPos = 1;
            wdbVars.ProcessedStringsDict.Add("", 0);

            var strArrayDict = new Dictionary<string, List<string>>();
            var strArrayValDict = new Dictionary<string, uint>();

            foreach (var sField in processedSFieldStringsDict)
            {
                var offsetValuesCount = wdbVars.OffsetsPerValue;

                for (int s = 0; s < sField.Value.Count; s++)
                {
                    while (offsetValuesCount != 0 && s < sField.Value.Count)
                    {

                    }
                }
            }
        }

        public static void ConvertRecords(WDBVariables wdbVars)
        {
            uint stringPos = 1;
            wdbVars.ProcessedStringsDict.Add("", 0);

            var outPerRecordSize = wdbVars.StrtypelistValues.Count * 4;
            foreach (var recordData in wdbVars.RecordsDataDict)
            {
                var currentOutData = new byte[outPerRecordSize];

                Console.WriteLine($"Record: {recordData.Key}");

                var dataIndex = 0;
                var strtypelistIndex = 0;

                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    var fieldBitsToProcess = 32;
                    var collectedBinary = string.Empty;
                    var addedString = false;

                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        // bitpacked
                        case 0:
                            int iTypeDataVal;
                            uint uTypeDataVal;
                            int fTypeDataVal;
                            //string fTypeBinary;

                            while (fieldBitsToProcess != 0 && f < wdbVars.FieldCount)
                            {
                                var fieldType = wdbVars.Fields[f].Substring(0, 1);
                                var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                                switch (fieldType)
                                {
                                    // sint
                                    case "i":
                                        iTypeDataVal = Convert.ToInt32(recordData.Value[f]);

                                        if (fieldNum != 0)
                                        {
                                            SharedMethods.ValidateInt(fieldNum, ref iTypeDataVal);
                                        }

                                        Console.WriteLine($"{wdbVars.Fields[f]}: {iTypeDataVal}");

                                        if (fieldNum == 0)
                                        {
                                            fieldNum = 32;
                                        }

                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            var iTypedataValBinary = iTypeDataVal.IntToBinaryFixed(fieldNum);

                                            if (iTypedataValBinary.Length > fieldNum)
                                            {
                                                iTypedataValBinary = iTypedataValBinary.Substring(iTypedataValBinary.Length - fieldNum, fieldNum);
                                            }

                                            iTypedataValBinary = iTypedataValBinary.ReverseBinary();
                                            collectedBinary += iTypedataValBinary;

                                            fieldBitsToProcess -= fieldNum;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // uint 
                                    case "u":
                                        uTypeDataVal = Convert.ToUInt32(recordData.Value[f]);

                                        if (fieldNum != 0)
                                        {
                                            SharedMethods.ValidateUInt(fieldNum, ref uTypeDataVal);
                                        }

                                        Console.WriteLine($"{wdbVars.Fields[f]}: {uTypeDataVal}");

                                        if (fieldNum == 0)
                                        {
                                            fieldNum = 32;
                                        }

                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            var uTypedataValBinary = uTypeDataVal.UIntToBinaryFixed(fieldNum).ReverseBinary();
                                            collectedBinary += uTypedataValBinary;

                                            fieldBitsToProcess -= fieldNum;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // float (bitpacked as int)
                                    case "f":
                                        fTypeDataVal = Convert.ToInt32(recordData.Value[f]);

                                        if (fieldNum != 0)
                                        {
                                            SharedMethods.ValidateInt(fieldNum, ref fTypeDataVal);
                                        }

                                        Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");

                                        if (fieldNum == 0)
                                        {
                                            fieldNum = 32;
                                        }

                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            var fTypedataValBinary = fTypeDataVal.IntToBinaryFixed(fieldNum);

                                            if (fTypedataValBinary.Length > fieldNum)
                                            {
                                                fTypedataValBinary = fTypedataValBinary.Substring(fTypedataValBinary.Length - fieldNum, fieldNum);
                                            }

                                            fTypedataValBinary = fTypedataValBinary.ReverseBinary();
                                            collectedBinary += fTypedataValBinary;

                                            fieldBitsToProcess -= fieldNum;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                        //// float (dump as binary) 
                                        //case "f":
                                        //    fTypeBinary = (string)recordData.Value[f];

                                        //    if (fieldNum != 0)
                                        //    {
                                        //        SharedMethods.ValidateFloatBinary(fieldNum, ref fTypeBinary);
                                        //    }

                                        //    Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeBinary}");

                                        //    if (fieldNum > fieldBitsToProcess)
                                        //    {
                                        //        f--;
                                        //        fieldBitsToProcess = 0;
                                        //        continue;
                                        //    }
                                        //    else
                                        //    {
                                        //        fTypeBinary = fTypeBinary.ReverseBinary();
                                        //        collectedBinary += fTypeBinary;

                                        //        fieldBitsToProcess -= fieldNum;

                                        //        if (fieldBitsToProcess != 0)
                                        //        {
                                        //            f++;
                                        //        }
                                        //    }
                                        //    break;
                                }
                            }

                            collectedBinary = collectedBinary.ReverseBinary();
                            var collectiveBinaryBytes = BitConverter.GetBytes(Convert.ToUInt32(collectedBinary, 2));

                            currentOutData[dataIndex] = collectiveBinaryBytes[3];
                            currentOutData[dataIndex + 1] = collectiveBinaryBytes[2];
                            currentOutData[dataIndex + 2] = collectiveBinaryBytes[1];
                            currentOutData[dataIndex + 3] = collectiveBinaryBytes[0];

                            strtypelistIndex++;
                            dataIndex += 4;
                            break;

                        // float value
                        case 1:
                            var floatVal = Convert.ToSingle(recordData.Value[f]);
                            Console.WriteLine($"{wdbVars.Fields[f]}: {floatVal}");

                            var floatValBytes = BitConverter.GetBytes(floatVal);

                            currentOutData[dataIndex] = floatValBytes[3];
                            currentOutData[dataIndex + 1] = floatValBytes[2];
                            currentOutData[dataIndex + 2] = floatValBytes[1];
                            currentOutData[dataIndex + 3] = floatValBytes[0];

                            strtypelistIndex++;
                            dataIndex += 4;
                            break;

                        // string section offset
                        case 2:
                            var stringVal = recordData.Value[f].ToString();
                            Console.WriteLine($"{wdbVars.Fields[f]}: {stringVal}");

                            if (stringVal != "")
                            {
                                if (!wdbVars.ProcessedStringsDict.ContainsKey(stringVal))
                                {
                                    wdbVars.ProcessedStringsDict.Add(stringVal, stringPos);
                                    addedString = true;
                                }

                                var stringPosBytes = BitConverter.GetBytes(wdbVars.ProcessedStringsDict[stringVal]);

                                currentOutData[dataIndex] = stringPosBytes[3];
                                currentOutData[dataIndex + 1] = stringPosBytes[2];
                                currentOutData[dataIndex + 2] = stringPosBytes[1];
                                currentOutData[dataIndex + 3] = stringPosBytes[0];

                                if (addedString)
                                {
                                    stringPos += (uint)Encoding.UTF8.GetByteCount(stringVal + "\0");
                                    addedString = false;
                                }
                            }

                            strtypelistIndex++;
                            dataIndex += 4;
                            break;

                        // uint value
                        case 3:
                            var uintVal = Convert.ToUInt32(recordData.Value[f]);
                            Console.WriteLine($"{wdbVars.Fields[f]}: {uintVal}");

                            var uintValBytes = BitConverter.GetBytes(uintVal);

                            currentOutData[dataIndex] = uintValBytes[3];
                            currentOutData[dataIndex + 1] = uintValBytes[2];
                            currentOutData[dataIndex + 2] = uintValBytes[1];
                            currentOutData[dataIndex + 3] = uintValBytes[0];

                            strtypelistIndex++;
                            dataIndex += 4;
                            break;
                    }
                }

                Console.WriteLine("");

                wdbVars.OutPerRecordData.Add(recordData.Key, currentOutData);
            }
        }
    }
}