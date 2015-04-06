using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager.Logic
{
    /// <summary>
    /// Utility class that offers usefull methods that are used in different classes
    /// </summary>
    public class Util
    {
        public const String devEmail = "mailto:jan@endlesscoderz.ch";
        public const String devWeb = "http://www.jan-bucher.ch/";

        /// <summary>
        /// Util class cannot be instantiated, all methods are static
        /// </summary>
        private Util()
        {
        }

        /// <summary>
        /// Escape a string that the input can be stored into a file
        /// </summary>
        /// <param name="input">Field that will be escaped</param>
        /// <returns>Escaped field</returns>
        public static String escape(String input)
        {
            String output = input;
            if (output != null)
            {
                // Remove all ";" because this char is used as separator
                output = output.Replace(';', ' ');
                // Replace all newlines because otherwise the field channot be encoded into the file correctly
                output = output.Replace("\r\n", "<NEWLINE>");
            }
            return output;
        }

        /// <summary>
        /// Unescape a string that was proceeded with <see cref="LicenseManager.Logic.Util.escape"/>
        ///  (Reverse the escape method)
        /// </summary>
        /// <param name="input">Escaped input</param>
        /// <returns>Original string</returns>
        public static String unescape(String input)
        {
            String output = input;
            if (output != null)
            {
                output = output.Replace("<NEWLINE>", "\r\n");
            }
            return output;
        }
    }
}
