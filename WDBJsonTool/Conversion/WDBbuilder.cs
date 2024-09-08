using System.Text;
using WDBJsonTool.Support;

namespace WDBJsonTool.Conversion
{
    internal class WDBbuilder
    {
        public static void BuildWDB(WDBVariables wdbVars)
        {
            // Build base wdb file
            CreateBase(wdbVars);
 
            // Start writing the data and update offsets
            using (var outWDBdataWriter = new BinaryWriter(File.Open(wdbVars.WDBName, FileMode.Open, FileAccess.Write)))
            {
                uint secPos = 0;
                long offsetUpdatePos = 32;

                // sheetname
                if (wdbVars.SheetName != "Not Specified")
                {
                    outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                    secPos = (uint)outWDBdataWriter.BaseStream.Position;

                    var sheetNameBytes = Encoding.UTF8.GetBytes(wdbVars.SheetName);

                    outWDBdataWriter.Write(sheetNameBytes);
                    PadBytesAfterSection(outWDBdataWriter);

                    UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)sheetNameBytes.Length);
                    offsetUpdatePos += 32;
                }


                // string 
                if (wdbVars.HasStringSection)
                {
                    outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                    secPos = (uint)outWDBdataWriter.BaseStream.Position;

                    uint stringSectionSize = 0;

                    foreach (var stringKey in wdbVars.ProcessedStringsDict.Keys)
                    {
                        if (stringKey == "")
                        {
                            outWDBdataWriter.Write((byte)0);
                            stringSectionSize++;
                        }
                        else
                        {
                            var stringKeyBytes = Encoding.UTF8.GetBytes(stringKey + "\0");
                            outWDBdataWriter.Write(stringKeyBytes);
                            stringSectionSize += (uint)stringKeyBytes.Length;
                        }
                    }

                    PadBytesAfterSection(outWDBdataWriter);

                    UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, stringSectionSize);
                    offsetUpdatePos += 32;
                }


                // strtypelist
                outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                secPos = (uint)outWDBdataWriter.BaseStream.Position;
                outWDBdataWriter.Write(wdbVars.StrtypelistData);

                PadBytesAfterSection(outWDBdataWriter);

                UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)wdbVars.StrtypelistData.Length);
                offsetUpdatePos += 32;


                // typelist
                if (wdbVars.HasTypelistSection)
                {
                    outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                    secPos = (uint)outWDBdataWriter.BaseStream.Position;
                    outWDBdataWriter.Write(wdbVars.TypelistData);

                    PadBytesAfterSection(outWDBdataWriter);

                    UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)wdbVars.TypelistData.Length);
                    offsetUpdatePos += 32;
                }


                // version
                outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                secPos = (uint)outWDBdataWriter.BaseStream.Position;
                outWDBdataWriter.Write(wdbVars.VersionData);

                PadBytesAfterSection(outWDBdataWriter);

                UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)wdbVars.VersionData.Length);
                offsetUpdatePos += 32;


                // structitem
                outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                secPos = (uint)outWDBdataWriter.BaseStream.Position;
                outWDBdataWriter.Write(wdbVars.StructItemData);

                PadBytesAfterSection(outWDBdataWriter);

                UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)wdbVars.StructItemData.Length);
                offsetUpdatePos += 32;


                // structitemnum
                outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                secPos = (uint)outWDBdataWriter.BaseStream.Position;
                outWDBdataWriter.Write(wdbVars.StructItemNumData);

                PadBytesAfterSection(outWDBdataWriter);

                UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, 4);
                offsetUpdatePos += 32;


                // records
                foreach (var recordkey in wdbVars.OutPerRecordData.Keys)
                {
                    var currentRecordData = wdbVars.OutPerRecordData[recordkey];

                    outWDBdataWriter.BaseStream.Position = outWDBdataWriter.BaseStream.Length;
                    secPos = (uint)outWDBdataWriter.BaseStream.Position;
                    outWDBdataWriter.Write(currentRecordData);

                    PadBytesAfterSection(outWDBdataWriter);

                    UpdateOffsets(outWDBdataWriter, offsetUpdatePos, secPos, (uint)currentRecordData.Length);
                    offsetUpdatePos += 32;
                }
            }
        }


        public static void BuildWDBStrArray(WDBVariables wdbVars)
        {
            // Build base wdb file
            CreateBase(wdbVars);

        }


        private static void CreateBase(WDBVariables wdbVars)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Building wdb file....");

            if (File.Exists(wdbVars.WDBName))
            {
                File.Delete(wdbVars.WDBName);
            }

            using (var outWDBwriter = new BinaryWriter(File.Open(wdbVars.WDBName, FileMode.Append, FileAccess.Write)))
            {
                outWDBwriter.Write(Encoding.UTF8.GetBytes("WPD\0"));
                outWDBwriter.WriteBytesUInt32(wdbVars.TotalRecordCount, true);
                outWDBwriter.BaseStream.PadNull(8);

                // sheetname
                if (wdbVars.SheetName != "Not Specified")
                {
                    var sheetNameSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.SheetNameSectionName);
                    outWDBwriter.Write(sheetNameSectionNameBytes);

                    outWDBwriter.BaseStream.PadNull(16 - sheetNameSectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);
                }

                // strarray
                if (wdbVars.HasStrArraySection)
                {
                    var strArraySectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.StrArraySectionName);
                    outWDBwriter.Write(strArraySectionNameBytes);
                    outWDBwriter.BaseStream.PadNull(16 - strArraySectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);

                    var strArrayInfoSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.StrArrayInfoSectionName);
                    outWDBwriter.Write(strArrayInfoSectionNameBytes);
                    outWDBwriter.BaseStream.PadNull(16 - strArrayInfoSectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);

                    var strArrayListSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.StrArrayListSectionName);
                    outWDBwriter.Write(strArrayListSectionNameBytes);
                    outWDBwriter.BaseStream.PadNull(16 - strArrayListSectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);
                }

                // string
                if (wdbVars.HasStringSection)
                {
                    var stringSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.StringSectionName);
                    outWDBwriter.Write(stringSectionNameBytes);

                    outWDBwriter.BaseStream.PadNull(16 - stringSectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);
                }

                // strtypelist
                var strtypelistSectionName = wdbVars.ParseStrtypelistAsV1 ? wdbVars.StrtypelistSectionName : wdbVars.StrtypelistbSectionName;
                var strtypelistSectionNameBytes = Encoding.UTF8.GetBytes(strtypelistSectionName);
                outWDBwriter.Write(strtypelistSectionNameBytes);

                outWDBwriter.BaseStream.PadNull(16 - strtypelistSectionNameBytes.Length);
                outWDBwriter.BaseStream.PadNull(16);

                // typelist
                if (wdbVars.HasTypelistSection)
                {
                    var typelistSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.TypelistSectionName);
                    outWDBwriter.Write(typelistSectionNameBytes);

                    outWDBwriter.BaseStream.PadNull(16 - typelistSectionNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);
                }

                // version
                var versionSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.VersionSectionName);
                outWDBwriter.Write(versionSectionNameBytes);

                outWDBwriter.BaseStream.PadNull(16 - versionSectionNameBytes.Length);
                outWDBwriter.BaseStream.PadNull(16);

                // structitem
                var structItemSectionNameBytes = Encoding.UTF8.GetBytes(wdbVars.StructItemSectionName);
                outWDBwriter.Write(structItemSectionNameBytes);

                outWDBwriter.BaseStream.PadNull(16 - structItemSectionNameBytes.Length);
                outWDBwriter.BaseStream.PadNull(16);

                // structitemnum
                var structItemSectionNumNameBytes = Encoding.UTF8.GetBytes(wdbVars.StructItemNumSectionName);
                outWDBwriter.Write(structItemSectionNumNameBytes);

                outWDBwriter.BaseStream.PadNull(16 - structItemSectionNumNameBytes.Length);
                outWDBwriter.BaseStream.PadNull(16);

                // record names
                foreach (var recordName in wdbVars.RecordsDataDict.Keys)
                {
                    var recordNameBytes = Encoding.UTF8.GetBytes(recordName);
                    outWDBwriter.Write(recordNameBytes);

                    outWDBwriter.BaseStream.PadNull(16 - recordNameBytes.Length);
                    outWDBwriter.BaseStream.PadNull(16);
                }
            }
        }


        private static void PadBytesAfterSection(BinaryWriter outWDBdataWriter)
        {
            var currentPos = outWDBdataWriter.BaseStream.Length;
            var padValue = 4;

            if (currentPos % padValue != 0)
            {
                var remainder = currentPos % padValue;
                var increaseBytes = padValue - remainder;
                var newPos = currentPos + increaseBytes;
                var nullBytesAmount = newPos - currentPos;

                outWDBdataWriter.BaseStream.PadNull((int)nullBytesAmount);
            }
        }


        private static void UpdateOffsets(BinaryWriter outWDBdataWriter, long pos, uint secPos, uint size)
        {
            outWDBdataWriter.BaseStream.Position = pos;
            outWDBdataWriter.WriteBytesUInt32(secPos, true);
            outWDBdataWriter.WriteBytesUInt32(size, true);
        }
    }
}