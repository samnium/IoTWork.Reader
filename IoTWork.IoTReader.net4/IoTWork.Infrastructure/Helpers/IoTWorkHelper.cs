using IoTWork.Infrastructure.Compressors;
using IoTWork.Infrastructure.Formatters;
using IoTWork.Infrastructure.Interfaces;
using IoTWork.Infrastructure.Management;
using IoTWork.Infrastructure.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IotWork.Utils.Helpers
{
    public static class IoTWorkHelper
    {
        public static byte CodeOfCompressor(ICompressor compressor)
        {
            if (compressor.GetType() == typeof(VoidCompressor))
                return 1;
            else if (compressor.GetType() == typeof(GZipCompressor))
                return 2;
            else
                return 0;
        }

        public static byte CodeOfFormatter<T>(IoTWork.Infrastructure.Interfaces.IFormatter<T> formatter)
        {
            if (formatter.GetType() == typeof(VoidFormatter<T>))
                return 1;
            else if (formatter.GetType() == typeof(BinaryFormatter<T>))
                return 2;
            else if (formatter.GetType() == typeof(JSonFormatter<T>))
                return 3;
            else if (formatter.GetType() == typeof(XmlFormatter<T>))
                return 4;
            else
                return 0;
        }

        public static byte CodeOfSigner<T>(ISigner<T> signer)
        {
            if (signer.GetType() == typeof(VoidSigner<T>))
                return 1;
            else
                return 0;
        }

        public static ICompressor AllocateCompressor(int code)
        {
            ICompressor compressor = new VoidCompressor();

            switch (code)
            {
                case 1:
                    compressor = new VoidCompressor();
                    break;
                case 2:
                    compressor = new GZipCompressor();
                    break;
                default:
                    compressor = new VoidCompressor();
                    break;
            }

            return compressor;
        }

        public static IFormatter<T> AllocateFormatter<T>(int code)
        {
            IFormatter<T> formatter = new VoidFormatter<T>();

            switch (code)
            {
                case 1:
                    formatter = new VoidFormatter<T>();
                    break;
                case 2:
                    formatter = new BinaryFormatter<T>();
                    break;
                case 3:
                    formatter = new JSonFormatter<T>();
                    break;
                case 4:
                    formatter = new XmlFormatter<T>();
                    break;
                default:
                    formatter = new VoidFormatter<T>();
                    break;
            }

            return formatter;
        }

        public static ISigner<T> AllocateSigner<T>(int code)
        {
            ISigner<T> signer = new VoidSigner<T>();

            switch (code)
            {
                case 1:
                    signer = new VoidSigner<T>();
                    break;
                case 2:
                    signer = new VoidSigner<T>();
                    break;
                default:
                    signer = new VoidSigner<T>();
                    break;
            }

            return signer;
        }

        public static T[] ArrayMerge<T>(T[] a1, T[] a2)
        {
            T[] rv = new T[a1.Length + a2.Length];
            System.Buffer.BlockCopy(a1, 0, rv, 0, a1.Length);
            System.Buffer.BlockCopy(a2, 0, rv, a1.Length, a2.Length);
            return rv;
        }

        public static T[] SubArray<T>(T[] buffer, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(buffer, index, result, 0, length);
            return result;
        }

        public static String UTF8ByteArrayToString(Byte[] buffer)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(buffer);
            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String message)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(message);
            return byteArray;
        }

        public static String Base64ByteArrayToString(Byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        public static Byte[] StringToBase64ByteArray(String message)
        {
            return Convert.FromBase64String(message);
        }

        public static string ComputeHmacSha1(string value, string key)
        {
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                return System.Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(value)));
            }
        }

        public static byte[] ComputeHmacSha1(byte[] value, string key)
        {
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                return hmac.ComputeHash(value);
            }
        }

        public static void WriteToFile(string Path, string Content)
        {
            File.WriteAllText(Path, Content);
        }

        public static ErrorResume ToErrorResume(Journal<ExceptionContainer> JExceptions)
        {
            ErrorResume _resume = new ErrorResume();
            for (int i = 0; i < JExceptions.Count; i++)
            {
                var _jex = JExceptions.GetAt(i);

                String ExMessage = _jex.Data.ex.Message;
                var e = _jex.Data.ex;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    ExMessage += ":: " + e.Message;
                }

                String Message = String.Format("{0} : {1} : {2}", _jex.MillisecondsFromBegin, _jex.MillisecondsFromLast, ExMessage);

                String Module = _jex.Data.Module + " " + _jex.Data.UniqueName;

                _resume.Add(_jex.Data.When, Module, Message, e);
            }
            return _resume;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}
