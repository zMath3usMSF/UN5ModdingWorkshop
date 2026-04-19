using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace howto_crypt_file
{
    public static class StringExtensions
    {
        // Return a string that represents the byte array
        // as a series of hexadecimal values separated
        // by a separator character.
        public static string ToHex(this byte[] the_bytes, char separator)
        {
            return BitConverter.ToString(the_bytes, 0).Replace('-', separator);
        }

        // Convert a string containing 2-digit hexadecimal
        // values into a byte array.
        public static byte[] ToBytes(this string the_string)
        {
            // Get the separator character.
            char separator = the_string[2];

            // Split at the separators.
            string[] pairs = the_string.Split(separator);
            byte[] bytes = new byte[pairs.Length];
            for (int i = 0; i < pairs.Length; i++)
                bytes[i] = Convert.ToByte(pairs[i], 16);
            return bytes;
        }
    }
}
