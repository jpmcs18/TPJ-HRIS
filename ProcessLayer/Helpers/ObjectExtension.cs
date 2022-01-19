using ProcessLayer.Helpers.Enumerable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers
{
    public static class ObjectExtension
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return (byte[])obj;
        }
        public static bool ToBoolean(this object obj)
        {
            return Convert.ToBoolean(obj);
        }
        public static byte ToByte(this object obj)
        {
            return Convert.ToByte(obj);
        }
        public static short ToShort(this object obj)
        {
            return Convert.ToInt16(obj);
        }
        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }
        public static long ToLong(this object obj)
        {
            return Convert.ToInt64(obj);
        }
        public static ushort ToUnsignShort(this object obj)
        {
            return Convert.ToUInt16(obj);
        }
        public static uint ToUnsignInt(this object obj)
        {
            return Convert.ToUInt32(obj);
        }
        public static ulong ToUnsignLong(this object obj)
        {
            return Convert.ToUInt64(obj);
        }
        public static float ToFloat(this object obj)
        {
            return Convert.ToSingle(obj);
        }
        public static double ToDouble(this object obj)
        {
            return Convert.ToDouble(obj);
        }
        public static decimal ToDecimal(this object obj)
        {
            return Convert.ToDecimal(obj);
        }
        public static DateTime ToDateTime(this object obj)
        {
            return Convert.ToDateTime(obj);
        }
        public static TimeSpan ToTimeSpan(this object obj)
        {
            TimeSpan.TryParse(obj.ToString(), out TimeSpan res);
            return res;
        }

        public static bool? ToNullableBoolean(this object obj)
        {
            if (obj == DBNull.Value) return null;
            return Convert.ToBoolean(obj);
        }
        public static byte? ToNullableByte(this object obj)
        {
            if (obj == DBNull.Value) return null;
            return Convert.ToByte(obj);
        }
        public static short? ToNullableShort(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToInt16(obj);
        }
        public static int? ToNullableInt(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToInt32(obj);
        }
        public static long? ToNullableLong(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToInt64(obj);
        }
        public static ushort? ToNullableUnsignShort(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToUInt16(obj);
        }
        public static uint? ToNullableUnsignInt(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToUInt32(obj);
        }
        public static ulong? ToNullableUnsignLong(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToUInt64(obj);
        }
        public static float? ToNullableFloat(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToSingle(obj);
        }
        public static double? ToNullableDouble(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToDouble(obj);
        }
        public static decimal? ToNullableDecimal(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else return Convert.ToDecimal(obj);
        }
        public static DateTime? ToNullableDateTime(this object obj)
        {
            if (obj == DBNull.Value || string.IsNullOrEmpty(obj.ToString())) return null;
            else return Convert.ToDateTime(obj);
        }
        public static TimeSpan? ToNullableTimeSpan(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else {
                TimeSpan.TryParse(obj.ToString(), out TimeSpan res);
                return res;
            }
        }
        public static SecureString ToSecuredString(this object obj)
        {
            return new NetworkCredential("", obj.ToString()).SecurePassword;
        }
        public static PayrollSheet ToPayrollSheet(this object obj)
        {
            return (PayrollSheet)(obj.ToNullableInt() ?? 1);
        }
        public static ComputationType ToComputationType(this object obj)
        {
            return (ComputationType)(obj.ToNullableInt() ?? 1);
        }
        public static ComputationType? ToNullableComputationType(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else
            {
                return (ComputationType)(obj.ToNullableInt() ?? 1);
            }
        }
        public static KioskNotificationFilter ToKioskNotificationFilter(this object obj)
        {
            return (KioskNotificationFilter)(obj.ToNullableInt() ?? 1);
        }
        public static KioskNotificationFilter? ToNullableKioskNotificationFilter(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else
            {
                return (KioskNotificationFilter)(obj.ToNullableInt() ?? 1);
            }
        }
        public static KioskNotoficationType ToKioskNotificationType(this object obj)
        {
            return (KioskNotoficationType)(obj.ToNullableInt() ?? 1);
        }
        public static OTType ToOTTYpe(this object obj)
        {
            return (OTType)(obj.ToNullableInt() ?? 1);
        }
        public static KioskNotoficationType? ToNullableKioskNotificationType(this object obj)
        {
            if (obj == DBNull.Value) return null;
            else
            {
                return (KioskNotoficationType)(obj.ToNullableInt() ?? 1);
            }
        }
        public static decimal ToDecimalPlaces(this decimal obj, int places)
        {
            return Math.Round(obj, places, MidpointRounding.AwayFromZero);
            
        }
        
    }

    public static class Global
    {
        public const String strPermutation = "tignalnam";
        public const Int32 bytePermutation1 = 0x19;
        public const Int32 bytePermutation2 = 0x59;
        public const Int32 bytePermutation3 = 0x17;
        public const Int32 bytePermutation4 = 0x41;
    }

    public static class StringExtension {
        // encrypt
        public static byte[] Encrypt(this string strData)
        {
            var str = Encoding.UTF8.GetBytes(strData);

            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Global.strPermutation,
            new byte[] { Global.bytePermutation1,
                         Global.bytePermutation2,
                         Global.bytePermutation3,
                         Global.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(str, 0, str.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        public static string Decrypt(this byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Global.strPermutation,
            new byte[] { Global.bytePermutation1,
                         Global.bytePermutation2,
                         Global.bytePermutation3,
                         Global.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return Encoding.UTF8.GetString(memstream.ToArray());
        }
    }
}

