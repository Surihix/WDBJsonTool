public static class BitOperationHelpers
{
    public static string UShortToBinary(this ushort valToParse)
    {
        var valArray = BitConverter.GetBytes(valToParse);

        return Convert.ToString(valArray[1], 2).PadLeft(8, '0') + "" +
            Convert.ToString(valArray[0], 2).PadLeft(8, '0');
    }


    public static string UIntToBinary(this uint valToParse)
    {
        var valArray = BitConverter.GetBytes(valToParse);

        return Convert.ToString(valArray[3], 2).PadLeft(8, '0') + "" +
            Convert.ToString(valArray[2], 2).PadLeft(8, '0') + "" +
            Convert.ToString(valArray[1], 2).PadLeft(8, '0') + "" +
            Convert.ToString(valArray[0], 2).PadLeft(8, '0');
    }


    public static string UIntToBinaryPadded(this uint valToParse, int padLength)
    {
        return Convert.ToString(valToParse, 2).PadLeft(padLength, '0');
    }


    public static string IntToBinaryPadded(this int valToParse, int padLength)
    {
        return Convert.ToString(valToParse, 2).PadLeft(padLength, '0');
    }


    public static uint BinaryToUInt(this string binaryVal, int startPosition, int count)
    {
        return Convert.ToUInt32(binaryVal.Substring(startPosition, count), 2);
    }


    public static int BinaryToInt(this string binaryVal, int startPosition, int count)
    {
        var pass1 = binaryVal.Substring(startPosition, count);

        if (pass1[0] == '0')
        {
            return (int)Convert.ToUInt32(pass1, 2);
        }
        else
        {
            int pass1BinaryVal;
            var iterationCount = count - 1;
            int computedValWithPower;

            var significantBitPower = -(int)Math.Pow(2, iterationCount);
            var finalComputedVal = significantBitPower;

            for (int i = 0; i < iterationCount; i++)
            {
                pass1BinaryVal = int.Parse(pass1[i + 1].ToString());
                computedValWithPower = (int)Math.Pow(2, (count - 2) - i);
                finalComputedVal += (computedValWithPower * pass1BinaryVal);
            }

            return finalComputedVal;
        }
    }
}