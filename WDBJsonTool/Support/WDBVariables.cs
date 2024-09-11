namespace WDBJsonTool.Support
{
    internal class WDBVariables
    {
        // Important variables
        public string? WDBFilePath;
        public string? JsonFilePath;
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
        public readonly string SheetNameSectionName = "!!sheetname";
        public readonly string StrArraySectionName = "!!strArray";
        public readonly string StrArrayInfoSectionName = "!!strArrayInfo";
        public readonly string StrArrayListSectionName = "!!strArrayList";
        public readonly string StringSectionName = "!!string";
        public readonly string StrtypelistSectionName = "!!strtypelist";
        public readonly string StrtypelistbSectionName = "!!strtypelistb";
        public readonly string TypelistSectionName = "!!typelist";
        public readonly string VersionSectionName = "!!version";
        public readonly string StructItemSectionName = "!structitem";
        public readonly string StructItemNumSectionName = "!structitemnum";

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