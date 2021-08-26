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
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(Encoding.UTF8.GetBytes(message));
        }

        public static string hmacHex(byte[] key, string message)
        {
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
            var keyDate = hash(Encoding.UTF8.GetBytes("AWS4" + key), datestamp);
            var keyString = hash(keyDate, region);
            var keyService = hash(keyString, service);
            var keySigning = hash(keyService, "aws4_request");
            return keySigning;
        }

    }
}
