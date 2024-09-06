namespace WDBJsonTool.Support
{
    internal class WDBVariables
    {
        // Important variables
        public string? WDBName;
        public string? JsonName;
        public uint RecordCount;
        public bool HasStrArraySection;
        public bool HasStringSection;
        public bool ParseStrtypelistAsV1;
        public bool HasTypelistSection;
        public string[]? Fields;
        public List<uint> StrArrayOffsets = new();
        public List<string> NumStringFields = new();
        public List<string> ProcessStringsList = new();
        public Dictionary<string, List<string>> StrArrayDict = new();
        public List<int> StrtypelistValues = new();
        public uint TotalRecordCount;
        public Dictionary<string, List<object>> RecordsDataDict = new();
        public Dictionary<string, uint> ProcessedStringsDict = new();
        public Dictionary<string, byte[]> OutPerRecordData = new();

        // Section names
        public string SheetNameSectionName = "!!sheetname";
        public string StrArraySectionName = "!!strArray";
        public string StrArrayInfoSectionName = "!!strArrayInfo";
        public string StrArrayListSectionName = "!!strArrayList";
        public string StringSectionName = "!!string";
        public string StrtypelistSectionName = "!!strtypelist";
        public string StrtypelistbSectionName = "!!strtypelistb";
        public string TypelistSectionName = "!!typelist";
        public string VersionSectionName = "!!version";
        public string StructItemSectionName = "!structitem";
        public string StructItemNumSectionName = "!structitemnum";

        // Section data
        public string? SheetName;
        public byte[]? SheetNameData;
        public byte[]? StrArrayData;
        public byte[]? StrArrayInfoData;
        public byte OffsetsPerValue;
        public byte BitsPerOffset;
        public byte[]? StrArrayListData;
        public byte[]? StringsData;
        public byte[]? StrtypelistData;
        public byte[]? TypelistData;
        public byte[]? VersionData;
        public byte[]? StructItemData;
        public byte[]? StructItemNumData;
        public uint FieldCount;
    }
}