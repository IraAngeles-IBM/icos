using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace fileUpload
{


    public class Crypto
    {


        public static byte[] hash(byte[] key, string message)
        {
            // var keyByte = Encoding.UTF8.GetBytes(key);
            var hash = new HMACSHA256(key);
            // string hashStr = Convert.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(message)));
            // return Encoding.UTF8.GetString(Encoding.Default.GetBytes(hashStr));
            return hash.ComputeHash(Encoding.UTF8.GetBytes(message));
        }


        // public static string hash(string key, string message)
        // {
        //     var keyByte = Encoding.UTF8.GetBytes(key);
        //     var hash = new HMACSHA256(keyByte);
        //     return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", string.Empty).ToLower();
        //     // return Encoding.UTF8.GetString(Encoding.Default.GetBytes(hashStr));
        //     // return hash.ComputeHash(Encoding.UTF8.GetBytes(message));
        // }


        public static string hmacHex(byte[] key, string message)
        {
            // var keyByte = Encoding.UTF8.GetBytes(key);
            var hash = new HMACSHA256(key);
            return Convert.ToHexString(hash.ComputeHash(Encoding.UTF8.GetBytes(message))).ToLower();
        }

        public static string hashHex(string message)
        {
            return Convert.ToHexString(
                System.Security.Cryptography.SHA256.Create().
                ComputeHash(Encoding.UTF8.GetBytes(message))).ToLower();
        }


        public static byte[] createSignatureKey(string key, string datestamp, string region, string service) 
        {
            // var strDatestamp = Convert.ToHexString(Encoding.UTF8.GetBytes(key));
            var keyDate = hash(Encoding.UTF8.GetBytes("AWS4" + key), datestamp);
            var keyString = hash(keyDate, region);
            var keyService = hash(keyString, service);
            var keySigning = hash(keyService, "aws4_request");
            // return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(keySigning))).Replace("-", string.Empty);
            return keySigning;
        }

        // public static string createSignatureKey(string key, string datestamp, string region, string service) 
        // {
        //     // var strDatestamp = Convert.ToHexString(Encoding.UTF8.GetBytes(key));
        //     var keyDate = Convert.ToString(hash(("AWS4" + key), datestamp));
        //     var keyString = Convert.ToString(hash(keyDate, region));
        //     var keyService = Convert.ToString(hash(keyString, service));
        //     var keySigning = Convert.ToString(hash(keyService, "aws4_request"));
        //     return keySigning;
        // }

        // public static string createHexSignatureKey(string key, string datestamp, string region, string service) 
        // {
        //     // var strDatestamp = Convert.ToHexString(Encoding.UTF8.GetBytes(key));
        //     var keyDate = hashHex(("AWS4" + key), datestamp);
        //     var keyString = hashHex(keyDate, region);
        //     var keyService = hashHex(keyString, service);
        //     var keySigning = hashHex(keyService, "aws4_request");
        //     return keySigning;
        // }

        // public static string password_encrypt(string clearText)
        // {
        //     string EncryptionKey = "MAKV2SPBNI99212";
        //     byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        //     using (Aes encryptor = Aes.Create())
        //     {
        //         Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
        //     0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        // });
        //         encryptor.Key = pdb.GetBytes(32);
        //         encryptor.IV = pdb.GetBytes(16);
        //         using (MemoryStream ms = new MemoryStream())
        //         {
        //             using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //             {
        //                 cs.Write(clearBytes, 0, clearBytes.Length);
        //                 cs.Close();
        //             }
        //             clearText = Convert.ToBase64String(ms.ToArray());
        //         }
        //     }
        //     return clearText;
        // }

        // public static string password_decrypt(string cipherText)
        // {
        //     if (cipherText != null)
        //     {
        //         string EncryptionKey = "MAKV2SPBNI99212";
        //         byte[] cipherBytes = Convert.FromBase64String(cipherText);
        //         using (Aes encryptor = Aes.Create())
        //         {
        //             Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
        //         0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        //     });
        //             encryptor.Key = pdb.GetBytes(32);
        //             encryptor.IV = pdb.GetBytes(16);
        //             using (MemoryStream ms = new MemoryStream())
        //             {
        //                 using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //                 {
        //                     cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                     cs.Close();
        //                 }
        //                 cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //             }
        //         }
        //     }
        //     return cipherText;
        // }

        // public static string url_encrypt(string clearText)
        // {
        //     string EncryptionKey = "MAKV2SPBNI99212";
        //     byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        //     using (Aes encryptor = Aes.Create())
        //     {
        //         Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
        //     0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        // });
        //         encryptor.Key = pdb.GetBytes(32);
        //         encryptor.IV = pdb.GetBytes(16);
        //         using (MemoryStream ms = new MemoryStream())
        //         {
        //             using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //             {
        //                 cs.Write(clearBytes, 0, clearBytes.Length);
        //                 cs.Close();
        //             }
        //             clearText = Convert.ToBase64String(ms.ToArray());
        //         }
        //     }
        //     //return clearText;

        //     return HttpUtility.UrlEncode(clearText);
        //     //return HttpContext.Current.Server.UrlEncode(password_encrypt(clearText));
        // }

        // public static string url_decrypt(string cipherText)
        // {

        //     cipherText = HttpUtility.UrlDecode(cipherText);
        //     cipherText = HttpUtility.UrlDecode(cipherText);
        //     cipherText = cipherText.Replace(' ', '+');

        //     if (cipherText != null)
        //     {
        //         string EncryptionKey = "MAKV2SPBNI99212";
        //         byte[] cipherBytes = Convert.FromBase64String(cipherText);
        //         using (Aes encryptor = Aes.Create())
        //         {
        //             Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
        //         0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        //     });
        //             encryptor.Key = pdb.GetBytes(32);
        //             encryptor.IV = pdb.GetBytes(16);
        //             using (MemoryStream ms = new MemoryStream())
        //             {
        //                 using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //                 {
        //                     cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                     cs.Close();
        //                 }
        //                 cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //             }
        //         }
        //     }
        //     return cipherText;

        //     //return password_decrypt(HttpContext.Current.Server.UrlDecode(cipherText));
        // }

    }
}
