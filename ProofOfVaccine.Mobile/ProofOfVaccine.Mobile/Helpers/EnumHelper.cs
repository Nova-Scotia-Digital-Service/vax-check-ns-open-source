using System;
using System.Collections.Generic;
using System.Text;

namespace ProofOfVaccine.Mobile.Helpers
{
    public static class EnumHelper
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] EnumArray = (T[])Enum.GetValues(src.GetType());
            int index = Array.IndexOf<T>(EnumArray, src) + 1;

            return (EnumArray.Length == index) ? EnumArray[0] : EnumArray[index];
        }

        public static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] EnumArray = (T[])Enum.GetValues(src.GetType());
            int index = Array.IndexOf<T>(EnumArray, src) - 1;

            return (index < 0) ? EnumArray[0] : EnumArray[index];
        }
    }
}
