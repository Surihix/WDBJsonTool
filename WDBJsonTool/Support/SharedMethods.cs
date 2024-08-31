using System.Text;

namespace WDBJsonTool.Support
{
    internal class SharedMethods
    {
        public static void ErrorExit(string errorMsg)
        {
            Console.WriteLine($"Error: {errorMsg}");
            Environment.Exit(1);
        }

        public static void CheckSectionName(BinaryReader br, string sectionName)
        {
            if (br.ReadBytesString(16, false) != sectionName)
            {
                ErrorExit($"{sectionName} is not present in the expected position");
            }
        }


        public static byte[] SaveSectionData(BinaryReader br, bool reverse)
        {
            var sectionOffset = br.ReadBytesUInt32(true);
            var sectionLength = br.ReadBytesUInt32(true);

            _ = br.BaseStream.Position = sectionOffset;
            var sectionData = br.ReadBytes((int)sectionLength);

            if (reverse)
            {
                Array.Reverse(sectionData);
            }

            return sectionData;
        }


        public static string DeriveStringFromArray(byte[] dataArray, int stringOffset)
        {
            var length = 0;
            for (int s = stringOffset; s < dataArray.Length; s++)
            {
                if (dataArray[s] == 0)
                {
                    break;
                }

                length++;
            }

            return Encoding.UTF8.GetString(dataArray, stringOffset, length);
        }


        public static int DeriveFieldNumber(string fieldName)
        {
            var foundNumsList = new List<int>();

            for (int i = 1; i < 3; i++)
            {
                if (i == 1 && !char.IsDigit(fieldName[i]))
                {
                    break;
                }

                if (char.IsDigit(fieldName[i]))
                {
                    foundNumsList.Add(int.Parse(Convert.ToString(fieldName[i])));
                }
            }

            var foundNumStr = "";
            foreach (var n in foundNumsList)
            {
                foundNumStr += n;
            }

            var hasParsed = int.TryParse(foundNumStr, out int foundNum);

            if (hasParsed)
            {
                return foundNum;
            }
            else
            {
                return 0;
            }
        }


        public static uint DeriveUIntFromSectionData(byte[] dataArray, int dataArrayIndex, bool reverse)
        {
            var processArray = new byte[4];
            Array.ConstrainedCopy(dataArray, dataArrayIndex, processArray, 0, 4);

            if (reverse)
            {
                Array.Reverse(processArray);
            }

            return BitConverter.ToUInt32(processArray, 0);
        }


        public static float DeriveFloatFromSectionData(byte[] dataArray, int dataArrayIndex, bool reverse)
        {
            var processArray = new byte[4];
            Array.ConstrainedCopy(dataArray, dataArrayIndex, processArray, 0, 4);

            if (reverse)
            {
                Array.Reverse(processArray);
            }

            return BitConverter.ToSingle(processArray, 0);
        }
    }
}