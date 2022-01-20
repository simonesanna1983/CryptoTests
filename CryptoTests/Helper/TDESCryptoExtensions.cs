using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoTests.Helper
{

    public sealed class TDESCryptoExtensions : IDisposable
    {
        private readonly SymmetricAlgorithm _cryptoProvider;
        private readonly Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDESCryptoExtensions"/> class.
        /// </summary>
        /// <param name="symmetricAlgorithm"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="encoding"></param>
        public TDESCryptoExtensions(SymmetricAlgorithm symmetricAlgorithm, string key, string iv, Encoding encoding)
        {
            _cryptoProvider = symmetricAlgorithm;
            _cryptoProvider.Key = CryptoUtils.GetBytes(key);
            _cryptoProvider.IV = CryptoUtils.GetBytes(iv);
            _cryptoProvider.Mode = CipherMode.CBC;
            _cryptoProvider.Padding = PaddingMode.PKCS7;
            _encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDESCryptoExtensions"/> class.
        /// </summary>
        /// <param name="symmetricAlgorithm"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public TDESCryptoExtensions(SymmetricAlgorithm symmetricAlgorithm, string key, string iv)
            : this(symmetricAlgorithm, key, iv, Encoding.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDESCryptoExtensions"/> class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public TDESCryptoExtensions(string key, string iv)
            : this(new TripleDESCryptoServiceProvider(), key, iv, Encoding.Default)
        { }

        /// <summary>
        /// Encrypt a byte array
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] inputArray)
        {
            return Transform(inputArray, _cryptoProvider.CreateEncryptor());
        }

        /// <summary>
        /// Decrypt a byte array
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] inputArray)
        {
            return Transform(inputArray, _cryptoProvider.CreateDecryptor());
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            var inputBytes = _encoding.GetBytes(value);
            var output = Encrypt(inputBytes);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Decrypt(string value)
        {
            var inputBytes = Convert.FromBase64String(value);
            var decrypt = Decrypt(inputBytes);
            var result = _encoding.GetString(decrypt);
            return result;
        }

        private static byte[] Transform(byte[] inputArray, ICryptoTransform transform)
        {
            byte[] result;
            var stream = new MemoryStream();

            using (var encryptStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                encryptStream.Write(inputArray, 0, inputArray.Length);
                encryptStream.FlushFinalBlock();
                stream.Position = 0;
                result = new Byte[(Int32)stream.Length];
                stream.Read(result, 0, result.Length);
            }
            return result;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_cryptoProvider != null)
            {
                //_cryptoProvider.Dispose();
                Dispose();
                GC.SuppressFinalize(this);
            }
        }

    }

    public class CryptoUtils
    {
        /// <summary>
        /// Get byte array from string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static Byte[] GetBytes(string hexString)
        {
            String newString = String.Empty;
            char c;
            for (Int32 i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
            }
            if (newString.Length % 2 != 0)
            {
                newString = newString.Substring(0, newString.Length - 1);
            }

            Int32 byteLength = newString.Length / 2;
            Byte[] bytes = new Byte[byteLength];
            String hex;
            Int32 j = 0;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                hex = new String(new[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        /// <summary>
        /// Determines if a character is hex digit
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsHexDigit(Char c)
        {
            Int32 numChar;
            Int32 numA = Convert.ToInt32('A');
            Int32 num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        private static byte HexToByte(String hex)
        {
            if ((hex.Length > 2) || (hex.Length <= 0))
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            var newByte = Byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
    }
}
