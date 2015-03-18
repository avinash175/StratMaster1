using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace CommonLib
{
    public class RE
    {
        // Function To test for lower Alphabets. 
        public static bool IsLowerAlpha(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^a-z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }
        // Function To test for upper Alphabets. 
        public static bool IsUpperAlpha(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^A-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        // Function To test for Alphabets. 
        public static bool IsAlpha(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }
        // Function to Check for AlphaNumeric.
        public static bool IsAlphaNumeric(String strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

        public static bool IsNumeric(String strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

    }
}
