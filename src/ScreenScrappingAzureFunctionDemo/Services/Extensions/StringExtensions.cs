using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ScreenScrappingAzureFunctionDemo.Services.Extensions
{
    /// <summary>
    ///     This represents the extension class for <see cref="string" />.
    /// </summary>
    public static class StringExtensions
    {
        private const string RGB_IV = "DLmKZ2iLHp7MuG9A2QVka2B==";
        private const string RGB_KEY = "gPrG6H3wBbWjKSGXX2C7QG==";
        private static readonly Regex _validEncription = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}[AEIMQUYcgkosw048]=|[A-Za-z0-9+/][AQgw]==)?$");

        /// <summary>
        /// Compresses the given <paramref name="input"/> to <c>Base64</c> string.
        /// </summary>
        /// <param name="input">The string to be compressed</param>
        /// <returns>The compressed string in <c>Base64</c></returns>
        [DebuggerStepThrough]
        public static string Compress(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }
            var buffer = Encoding.UTF8.GetBytes(input);
            using (var memStream = new MemoryStream())
            using (var zipStream = new GZipStream(memStream, CompressionMode.Compress, true))
            {
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();
                memStream.Position = 0;
                var compressedData = new byte[memStream.Length];
                memStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            }
        }

        /// <summary>
        ///     Checks whether the given list of items contains the item or not, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value contains the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool ContainsEquivalent(this string value, string comparer)
        {
            value.ThrowIfNullOrWhiteSpace();
            return !comparer.IsNullOrWhiteSpace() && value.ToLowerInvariant().Contains(comparer.ToLowerInvariant());
        }

        /// <summary>
        ///     Checks whether the given list of items contains the item or not, regardless of casing.
        /// </summary>
        /// <param name="items">List of items.</param>
        /// <param name="item">Item to check.</param>
        /// <returns>Returns <c>True</c>, if the list of items contains the item; otherwise returns <c>False</c>.</returns>
        public static bool ContainsEquivalent(this IEnumerable<string> items, string item)
        {
            items.ThrowIfNullOrDefault();
            return items.Any(p => p.IsEquivalentTo(item));
        }

        /// <summary>
        /// Decompresses a <c>Base64</c> compressed string.
        /// </summary>
        /// <param name="compressedInput">The string compressed in <c>Base64</c></param>
        /// <returns>The uncompressed string</returns>
        [DebuggerStepThrough]
        public static string Decompress(this string compressedInput)
        {
            if (string.IsNullOrWhiteSpace(compressedInput))
            {
                throw new ArgumentNullException(nameof(compressedInput));
            }
            var gZipBuffer = Convert.FromBase64String(compressedInput);
            using (var memStream = new MemoryStream())
            {
                var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                memStream.Position = 0;
                var buffer = new byte[dataLength];
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    zipStream.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string Decrypt(this string self)
        {
            if (string.IsNullOrWhiteSpace(self) || (self.Length % 4 != 0) || !_validEncription.IsMatch(self))
            {
                return self;
            }
            using (var rc2CryptoServiceProvider = new RC2CryptoServiceProvider())
            {
                var decryptor = rc2CryptoServiceProvider.CreateDecryptor(Convert.FromBase64String(RGB_KEY), Convert.FromBase64String(RGB_IV));
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(self)))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        var bytes = new List<byte>();
                        int readByte;
                        do
                        {
                            readByte = cryptoStream.ReadByte();
                            if (readByte != -1)
                            {
                                bytes.Add(Convert.ToByte(readByte));
                            }
                        } while (readByte != -1);
                        return Encoding.Unicode.GetString(bytes.ToArray());
                    }
                }
            }
        }

        public static string Encrypt(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return self;
            }
            using (var rc2CryptoServiceProvider = new RC2CryptoServiceProvider())
            {
                var encryptor = rc2CryptoServiceProvider.CreateEncryptor(Convert.FromBase64String(RGB_KEY), Convert.FromBase64String(RGB_IV));
                using (var msEncrypt = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        var toEncrypt = Encoding.Unicode.GetBytes(self);
                        cryptoStream.Write(toEncrypt, 0, toEncrypt.Length);
                        cryptoStream.FlushFinalBlock();
                        var encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
        }

        /// <summary>
        ///     Checks whether the string value ends with the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value ends with the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool EndsWithEquivalent(this string value, string comparer)
        {
            value.ThrowIfNullOrWhiteSpace();
            return !comparer.IsNullOrWhiteSpace() && value.EndsWith(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the size of the given <paramref name="input"/> encoded 
        /// as <c>UTF-16</c> characters in bytes.
        /// </summary>
        /// <param name="input">The string</param>
        [DebuggerStepThrough]
        public static int GetSize(this string input) => input.Length * sizeof(char);

        /// <summary>
        ///     Checks whether the string value is equal to the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value is equal to the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool IsEquivalentTo(this string value, string comparer)
        {
            return value.Equals(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// A nice way of calling the inverse of <see cref="string.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNotNullOrEmpty(this string value) => !value.IsNullOrEmpty();

        /// <summary>
        /// A nice way of checking the inverse of (if a string is null, empty or whitespace) 
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNotNullOrEmptyOrWhiteSpace(this string value) => !value.IsNullOrEmptyOrWhiteSpace();

        /// <summary>
        /// A nicer way of calling <see cref="string.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
        
        /// <summary>
        /// A nice way of checking if a string is null, empty or whitespace 
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>
        /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmptyOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
        
        /// <summary>
        ///     Checks whether the string value is either <c>null</c> or white space.
        /// </summary>
        /// <param name="value"><see cref="string" /> value to check.</param>
        /// <returns>Returns <c>True</c>, if the string value is either <c>null</c> or white space; otherwise returns <c>False</c>.</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// This method fulfills following requirements:
        ///  - returns empty string if start-index is out of range
        ///  - returns string from start to given length, if start-index is smaller than 0
        ///  - returns string from start-index to end of string, if length argument is beyond number of characters left
        ///  - returns appropriate string if arguments in valid range
        /// </summary>
        /// <param name="self"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns>string</returns>
        public static string SafeSubstring(this string self, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return string.Empty;
            }
            if (self.Length >= startIndex + length)
            {
                return self.Substring(startIndex, length);
            }
            return self.Length >= startIndex ? self.Substring(startIndex) : string.Empty;
        }

        /// <summary>
        ///     Checks whether the string value starts with the comparer, regardless of casing.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="comparer">Comparing value.</param>
        /// <returns>
        ///     Returns <c>True</c>, if the string value starts with the comparer, regardless of casing; otherwise returns
        ///     <c>False</c>.
        /// </returns>
        public static bool StartsWithEquivalent(this string value, string comparer)
        {
            value.ThrowIfNullOrWhiteSpace();
            return !comparer.IsNullOrWhiteSpace() && value.StartsWith(comparer, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if the given value is <c>null</c> or white space.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///     Returns the original value, if the value is NOT <c>null</c>; otherwise throws an
        ///     <see cref="ArgumentNullException" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        public static string ThrowIfNullOrWhiteSpace(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(value));
            }
            return value;
        }

        /// <summary>
        ///     Converts the string value to <see cref="bool" /> value.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <returns>Returns the <see cref="bool" /> value converted.</returns>
        public static bool ToBoolean(this string value)
        {
            return !value.IsNullOrWhiteSpace() && Convert.ToBoolean(value);
        }

        /// <summary>
        ///     Converts the string value to <see cref="int" /> value.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <returns>Returns the <see cref="int" /> value converted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
        public static int ToInt32(this string value)
        {
            value.ThrowIfNullOrWhiteSpace();
            return Convert.ToInt32(value);
        }

        public static string TrimWhiteSpace(this string self)
        {
            return Regex.Replace(self, @"\s{2,}", " ").Trim();
        }

        /// <summary>
        /// Truncate string to max bytes length
        /// </summary>
        /// <param name="self">Input string</param>
        /// <param name="maxBytesLength">Max bytes length</param>
        /// <returns></returns>
	    public static string Truncate(this string self, int maxBytesLength)
        {
            var result = self;
            var byteCount = Encoding.UTF8.GetByteCount(self);
            if (byteCount <= maxBytesLength)
            {
                return result;
            }
            var byteArray = Encoding.UTF8.GetBytes(self);
            result = Encoding.UTF8.GetString(byteArray, 0, maxBytesLength);
            return result;
        }
        
        /// <summary>
        ///     Decodes the string value.
        /// </summary>
        /// <param name="value">String value to decode.</param>
        /// <returns>Returns URL decoded value.</returns>
        public static string UrlDecode(this string value)
        {
            return value.IsNullOrWhiteSpace()
                       ? value
                       : WebUtility.UrlDecode(value.Replace("%20", "+"));
        }

        /// <summary>
        ///     Encodes the string value.
        /// </summary>
        /// <param name="value">String value to encode.</param>
        /// <returns>Returns URL encoded value.</returns>
        public static string UrlEncode(this string value)
        {
            return value.IsNullOrWhiteSpace()
                       ? value
                       : WebUtility.UrlEncode(value)?.Replace("+", "%20");
        }
    }
}
