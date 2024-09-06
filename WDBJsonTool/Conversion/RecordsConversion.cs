﻿using System.Text;
using WDBJsonTool.Support;

namespace WDBJsonTool.Conversion
{
    internal class RecordsConversion
    {
        public static void ConvertRecords(WDBVariables wdbVars)
        {
            uint stringPos = 1;
            wdbVars.ProcessedStringsDict.Add("", 0);

            var outPerRecordSize = wdbVars.StrtypelistValues.Count * 4;

            if (wdbVars.HasStrArraySection)
            {

            }
            else
            {
                foreach (var recordData in wdbVars.RecordsDataDict)
                {
                    var currentOutData = new byte[outPerRecordSize];
                    var collectedBinaryList = new List<char>();

                    Console.WriteLine($"Record: {recordData.Key}");

                    var dataIndex = 0;
                    var strtypelistIndex = 0;

                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        var fieldBitsToProcess = 32;
                        bool addedString = false;

                        switch (wdbVars.StrtypelistValues[strtypelistIndex])
                        {
                            case 0:
                                int iTypedataVal;
                                uint uTypeDataVal;

                                while (fieldBitsToProcess != 0 && f < wdbVars.FieldCount)
                                {
                                    var fieldType = wdbVars.Fields[f].Substring(0, 1);
                                    var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                                    switch (fieldType)
                                    {
                                        // sint
                                        case "i":
                                            iTypedataVal = Convert.ToInt32(recordData.Value[f]);
                                            Console.WriteLine($"{wdbVars.Fields[f]}: {iTypedataVal}");

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
                                                var iTypedataValBinary = iTypedataVal.IntToBinaryPadded(fieldNum);

                                                if (iTypedataValBinary.Length > fieldNum)
                                                {
                                                    iTypedataValBinary = iTypedataValBinary.Substring(iTypedataValBinary.Length - fieldNum, fieldNum);
                                                }

                                                var fixedValBinary = iTypedataValBinary.Reverse();

                                                collectedBinaryList.AddRange(fixedValBinary);

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
                                                var uTypedataValBinary = uTypeDataVal.UIntToBinaryPadded(fieldNum).Reverse();
                                                collectedBinaryList.AddRange(uTypedataValBinary);

                                                fieldBitsToProcess -= fieldNum;

                                                if (fieldBitsToProcess != 0)
                                                {
                                                    f++;
                                                }
                                            }
                                            break;
                                    }
                                }

                                collectedBinaryList.Reverse();

                                var collectiveBinary = string.Concat(collectedBinaryList);
                                var collectiveBinaryBytes = BitConverter.GetBytes(Convert.ToUInt32(collectiveBinary, 2));

                                currentOutData[dataIndex] = collectiveBinaryBytes[3];
                                currentOutData[dataIndex + 1] = collectiveBinaryBytes[2];
                                currentOutData[dataIndex + 2] = collectiveBinaryBytes[1];
                                currentOutData[dataIndex + 3] = collectiveBinaryBytes[0];
                                collectedBinaryList.Clear();

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

                                fieldBitsToProcess = 0;

                                strtypelistIndex++;
                                dataIndex += 4;
                                break;


                            // uint
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
}