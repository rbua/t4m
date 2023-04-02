using System;
using System.Collections;

namespace t4m.Helpers;

public static class NumericTypesHelper
{
    public static byte[] ConvertArray(int[] array)
    {
        if (!array.All(i => i >= 0 && i <= 255))
        {
            throw new ArgumentException("All values in the int array must be between 0 and 255.");
        }

        return array.Select(i => Convert.ToByte(i)).ToArray();
    }
}
