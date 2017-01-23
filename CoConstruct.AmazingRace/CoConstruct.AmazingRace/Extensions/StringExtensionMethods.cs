namespace CoConstruct.AmazingRace.Extensions
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Remove the given acii character from the string
        /// </summary>
        /// <param name="s">original string</param>
        /// <param name="asciiChar">ascii character to remove</param>
        /// <returns>opiginal string minus the ascii character</returns>
        public static string RemoveAsciiChar(this string s, char asciiChar)
        {
            var stringChar = asciiChar.ToString();
            return s.Replace(stringChar, "");
        }
    }
}