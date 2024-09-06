using System.Text;
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

                    var dataIndex = 0;
                    var fieldBitsToProcess = 32;
                    var strtypelistIndex = 0;

                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        if (fieldBitsToProcess == 0)
                        {
                            fieldBitsToProcess = 32;
                            dataIndex += 4;
                        }

                        var fieldType = wdbVars.Fields[f].Substring(0, 1);
                        var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                        int iTypedataVal;
                        float fTypeDataVal;
                        string stringVal;
                        bool addedString = false;
                        uint uTypeDataVal;

                        if (fieldBitsToProcess < fieldNum)
                        {
                            if (collectedBinaryList.Count > 0)
                            {
                                collectedBinaryList.Reverse();
                                WriteBinaryToArray(collectedBinaryList, currentOutData, dataIndex);
                                collectedBinaryList.Clear();
                            }

                            fieldBitsToProcess = 32;
                            dataIndex += 4;
                        }

                        switch (fieldType)
                        {
                            case "i":
                                iTypedataVal = Convert.ToInt32(recordData.Value[f]);

                                if (fieldNum == 0)
                                {
                                    if (iTypedataVal != 0)
                                    {
                                        var iTypedataValBytes = BitConverter.GetBytes(iTypedataVal);
                                        currentOutData[dataIndex] = iTypedataValBytes[3];
                                        currentOutData[dataIndex + 1] = iTypedataValBytes[2];
                                        currentOutData[dataIndex + 2] = iTypedataValBytes[1];
                                        currentOutData[dataIndex + 3] = iTypedataValBytes[0];
                                    }

                                    fieldBitsToProcess = 0;
                                }
                                else
                                {

                                    fieldBitsToProcess -= fieldNum;
                                }
                                break;


                            case "f":
                                fTypeDataVal = Convert.ToSingle(recordData.Value[f]);

                                if (fieldNum == 0)
                                {
                                    if (fTypeDataVal != 0)
                                    {
                                        var fTypedataValBytes = BitConverter.GetBytes(fTypeDataVal);
                                        currentOutData[dataIndex] = fTypedataValBytes[3];
                                        currentOutData[dataIndex + 1] = fTypedataValBytes[2];
                                        currentOutData[dataIndex + 2] = fTypedataValBytes[1];
                                        currentOutData[dataIndex + 3] = fTypedataValBytes[0];
                                    }

                                    fieldBitsToProcess = 0;
                                }
                                else
                                {

                                    fieldBitsToProcess -= fieldNum;
                                }
                                break;


                            case "s":
                                stringVal = recordData.Value[f].ToString();

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
                                break;


                            case "u":
                                uTypeDataVal = Convert.ToUInt32(recordData.Value[f]);

                                if (fieldNum == 0)
                                {
                                    if (uTypeDataVal != 0)
                                    {
                                        var uTypedataValBytes = BitConverter.GetBytes(uTypeDataVal);
                                        currentOutData[dataIndex] = uTypedataValBytes[3];
                                        currentOutData[dataIndex + 1] = uTypedataValBytes[2];
                                        currentOutData[dataIndex + 2] = uTypedataValBytes[1];
                                        currentOutData[dataIndex + 3] = uTypedataValBytes[0];
                                    }

                                    fieldBitsToProcess = 0;
                                }
                                else
                                {
                                    var uTypedataValBinary = uTypeDataVal.UIntToBinaryPadded(fieldNum).Reverse();
                                    collectedBinaryList.AddRange(uTypedataValBinary);

                                    fieldBitsToProcess -= fieldNum;
                                }
                                break;
                        }
                    }

                    if (collectedBinaryList.Count > 0)
                    {
                        collectedBinaryList.Reverse();
                        WriteBinaryToArray(collectedBinaryList, currentOutData, dataIndex);
                    }

                    wdbVars.OutPerRecordData.Add(recordData.Key, currentOutData);
                }

                WDBbuilder.BuildWDB(wdbVars);
            }
        }


        private static void WriteBinaryToArray(List<char> collectedBinaryList, byte[] currentOutData, int dataIndex)
        {
            var collectiveBinary = string.Concat(collectedBinaryList);
            var collectiveBinaryBytes = BitConverter.GetBytes(Convert.ToUInt32(collectiveBinary, 2));

            currentOutData[dataIndex] = collectiveBinaryBytes[3];
            currentOutData[dataIndex + 1] = collectiveBinaryBytes[2];
            currentOutData[dataIndex + 2] = collectiveBinaryBytes[1];
            currentOutData[dataIndex + 3] = collectiveBinaryBytes[0];
        }
    }
}