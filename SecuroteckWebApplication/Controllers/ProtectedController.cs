using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using SecuroteckWebApplication.Models;
using System.Text;
using System.IO;

namespace SecuroteckWebApplication.Controllers
{
    public class ProtectedController : ApiController
    {
        static private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        UserDatabaseAccess userDatabase = new UserDatabaseAccess();

        static string ByteArrayToHexString(byte[] byteArray)
        {
            string hexString = "";
            if (null != byteArray)
            {
                foreach (byte b in byteArray)
                {
                    hexString += b.ToString("x2");
                }
            }
            return hexString;
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage Hello()
        {

            IEnumerable<string> key;
            Request.Headers.TryGetValues("x-api-key", out key);
            User user = userDatabase.GetUser(key.First());
            string logString = "Protected Hello";
            Log log = new Log(logString, DateTime.UtcNow);
            user.Log.Add(log);
            return Request.CreateResponse(HttpStatusCode.OK, "Hello " + user.UserName);
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage SHA1([FromUri]string message)
        {
            if (message != null)
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);

                byte[] sha1ByteMessage;
                SHA1 sha1Provider = new SHA1CryptoServiceProvider();
                sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage);

                return Request.CreateResponse(HttpStatusCode.OK, ByteArrayToHexString(sha1ByteMessage).ToUpper());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage SHA256([FromUri]string message)
        {
            if (message != null)
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);

                byte[] sha1ByteMessage;
                SHA256 sha1Provider = new SHA256CryptoServiceProvider();
                sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage);

                return Request.CreateResponse(HttpStatusCode.OK, ByteArrayToHexString(sha1ByteMessage).ToUpper());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage getPublicKey()
        {
            IEnumerable<string> key;
            Request.Headers.TryGetValues("x-api-key", out key);

            User user = userDatabase.GetUser(key.First());
            string logString = "Protected Get PublicKey";
            Log log = new Log(logString, DateTime.UtcNow);
            user.Log.Add(log);

            if (userDatabase.CheckUser(key.First()))
            {
                return Request.CreateResponse(HttpStatusCode.OK, rsa.ToXmlString(false));
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Couldn't Get the Public Key");
            }
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage Sign([FromUri] string message)
        {
            IEnumerable<string> key;
            Request.Headers.TryGetValues("x-api-key", out key);
            byte[] messageAsBytes = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] signedMessage = rsa.SignData(messageAsBytes, CryptoConfig.MapNameToOID("SHA1"));

            User user = userDatabase.GetUser(key.First());
            string logString = "Protected Sign";
            Log log = new Log(logString, DateTime.UtcNow);
            user.Log.Add(log);

            return Request.CreateResponse(HttpStatusCode.OK, BitConverter.ToString(signedMessage));
        }

        [APIAuthorise]
        [HttpGet]
        public HttpResponseMessage AddFifty([FromUri] string encryptedInteger, string encryptedsymkey, string encryptedIV)
        {
            if(encryptedInteger != null && encryptedIV != null && encryptedsymkey != null)
            {
                string[] encryptedIntAsArray = encryptedInteger.Split('-');
                string[] encryptedAesKeyArray = encryptedsymkey.Split('-');
                string[] encryptedIvKeyArray = encryptedIV.Split('-');

                byte[] encryptedIntAsByte = new byte[encryptedIntAsArray.Length];
                byte[] EncryptedAesKeyAsByte = new byte[encryptedAesKeyArray.Length];
                byte[] encryptedIvKeyAsByte = new byte[encryptedIvKeyArray.Length];

                for(int i = 0; i < encryptedIntAsArray.Length; i++)
                {
                    encryptedIntAsByte[i] = Convert.ToByte(encryptedIntAsArray[i], 16);
                }
                for (int i = 0; i < encryptedAesKeyArray.Length; i++)
                {
                    EncryptedAesKeyAsByte[i] = Convert.ToByte(encryptedAesKeyArray[i], 16);
                }
                for (int i = 0; i < encryptedIvKeyArray.Length; i++)
                {
                    encryptedIvKeyAsByte[i] = Convert.ToByte(encryptedIvKeyArray[i], 16);
                }

                byte[] decryptedIntAsByte = rsa.Decrypt(encryptedIntAsByte, true);
                byte[] decryptedAesKeyByte = rsa.Decrypt(EncryptedAesKeyAsByte, true);
                byte[] decryptedIvKeyByte = rsa.Decrypt(encryptedIvKeyAsByte, true);

                int integer = BitConverter.ToInt32(decryptedIntAsByte, 0);
                string AesKey = Encoding.ASCII.GetString(decryptedAesKeyByte);
                string IvKey = Encoding.ASCII.GetString(decryptedIvKeyByte);

                integer += 50;
                string intPlusFifty = integer.ToString();

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.Key = decryptedAesKeyByte;
                aes.IV = decryptedIvKeyByte;

                byte[] encryptedMessageBytes;

                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(intPlusFifty);
                        }
                        encryptedMessageBytes = memoryStream.ToArray();
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, BitConverter.ToString(encryptedMessageBytes));

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
        }
    }
}
