using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;

namespace SecuroteckClient
{
    #region Task 10 and beyond
    class Client
    {
        const string UniqueKey = "3716722";
        static public string username = null;
        static public string APIKey = null;
        static public string publicKey = null;
        static public byte[] aesKey = null;
        static public byte[] ivKey = null;
        static private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        static private AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello. What would you like to do?");
            while (true)
            {
                string input = Console.ReadLine();
                if(input == "exit")
                { break; }
                string[] splitInput = input.Split(' ');
                RunAsync(splitInput).Wait();
                Console.WriteLine("What would you like to do?");
            }
        }

        static async Task<string> TalkBack(string[] pInput)
        {
            string responseString = "";
            if(pInput[1] == "Hello")
            {
                using (var httpClient = new HttpClient())
                {                    
                    HttpResponseMessage response = await httpClient.GetAsync("http://localhost:24702/api/talkback/hello");
                    responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }
            else if(pInput[1] == "Sort")
            {
                using (var httpClient = new HttpClient())
                {

                    pInput[2] = pInput[2].TrimStart('[');
                    pInput[2] = pInput[2].TrimEnd(']');
                    string[] numArray = pInput[2].Split(',');                    
                    string messageString = "";
                    for (int i = 0; i < numArray.Length; i++)
                    {
                        if(i != 0 )
                        {
                            messageString += "&";
                        }
                        messageString += "integers=" + numArray[i];
                    }
                    string path = "http://localhost:24702/api/talkback/sort?" + messageString;
                    HttpResponseMessage response = await httpClient.GetAsync(path);
                    responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }
            else
            {
                responseString = "Invalid Request";
                return responseString;
            }
        }

        static async Task<string> User(string[] pInput)
        {
            using (var httpClient = new HttpClient())
            {
                string path = "http://localhost:24702/api/user";
                string responseString = "";
                switch (pInput[1])
                {
                    case ("Get"):
                        path += "/new?username=" + pInput[2];
                        HttpResponseMessage getResponse = await httpClient.GetAsync(path);
                        responseString = await getResponse.Content.ReadAsStringAsync();
                        return responseString;
                    case ("Post"):
                        path += "/new";
                        var postContent = new StringContent('"' + pInput[2] + '"', Encoding.UTF8, "application/json");
                        HttpResponseMessage postResponse = await httpClient.PostAsync(path, postContent);
                        responseString = await postResponse.Content.ReadAsStringAsync();
                        APIKey = responseString.TrimEnd('"').TrimStart('"');
                        username = pInput[2];
                        return responseString;
                    case ("Set"):
                        if (pInput[2] != null && pInput[3] != null)
                        {
                            username = pInput[2];
                            APIKey = pInput[3];
                            responseString = "Sorted";
                            return responseString;
                        }
                        else
                        {
                            responseString = "Please Enter a Username and API-Key.";
                            return responseString;
                        }
                    case ("Delete"):
                        if(username != null && APIKey != null)
                        {
                            path += "/removeuser?username=" + username;
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage deleteResponse = await httpClient.DeleteAsync(path);
                            responseString = await deleteResponse.Content.ReadAsStringAsync();
                            return responseString;
                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first.";
                            return responseString;
                        }
                    case ("Role"):
                        if(APIKey != null)
                        {
                            path += "/changerole";
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            var roleContent = new StringContent("{\"username\":\"" + pInput[2] + "\",\"role\":\"" + pInput[3] + "\"}",Encoding.UTF8, "application/json");
                            HttpResponseMessage roleResponse = await httpClient.PostAsync(path, roleContent);
                            responseString = await roleResponse.Content.ReadAsStringAsync();
                            return responseString.TrimStart('"').TrimEnd('"');
                        }
                        else
                        {
                            responseString = "you need to do a User Post or User Set first";
                            return responseString;
                        }
                    default:
                        responseString = "Invalid Request";
                        return responseString;
                }
            }
        }

        static async Task<string> Protected(string[] pInput)
        {
            using (var httpClient = new HttpClient())
            {
                string path = "http://localhost:24702/api/protected";
                string responseString = "";
                switch (pInput[1])
                {
                    case ("Hello"):
                        if(APIKey != null)
                        {
                            path += "/hello";
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage helloResponse = await httpClient.GetAsync(path);
                            responseString = await helloResponse.Content.ReadAsStringAsync();
                            return responseString;
                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first.";
                            return responseString;
                        }
                    case ("SHA1"):
                        if (APIKey != null)
                        {
                            try
                            {
                                path += "/sha1?message=" + pInput[2];
                            }
                            catch
                            {
                                responseString = "Please Enter a message with SHA1 e.g. Protected SHA1 Hello";
                                return responseString;
                            }
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage helloResponse = await httpClient.GetAsync(path);
                            responseString = await helloResponse.Content.ReadAsStringAsync();
                            return responseString;
                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first.";
                            return responseString;
                        }
                    case ("SHA256"):
                        if (APIKey != null)
                        {
                            try
                            {
                                path += "/sha256?message=" + pInput[2];
                            }
                            catch
                            {
                                responseString = "Please Enter a message with SHA256 e.g. Protected SHA1 Hello";
                                return responseString;
                            }
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage helloResponse = await httpClient.GetAsync(path);
                            responseString = await helloResponse.Content.ReadAsStringAsync();
                            return responseString;
                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first.";
                            return responseString;
                        }
                    case ("Get"):
                        if(pInput[2] != "PublicKey")
                        {
                            responseString = "Invalid Request";
                            return responseString;
                        }
                        else if (APIKey != null)
                        {
                            path += "/getpublickey";
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage getKeyResponse = await httpClient.GetAsync(path);
                            responseString = await getKeyResponse.Content.ReadAsStringAsync();
                            if (responseString != null)
                            {
                                publicKey = responseString.TrimStart('"').TrimEnd('"');
                                responseString = "Got Public key";
                                return responseString;
                            }
                            else
                            {
                                responseString = "Couldn't Get the Public Key";
                                return responseString;
                            }
                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first.";
                            return responseString;
                        }
                    case ("Sign"):
                        if(publicKey != null)
                        {
                            path += "/sign?message=" + pInput[2];
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
                            HttpResponseMessage signResponse = await httpClient.GetAsync(path);
                            string response = await signResponse.Content.ReadAsStringAsync();
                            rsa.FromXmlString(publicKey);
                            response = response.TrimStart('"').TrimEnd('"');
                            string[] responseArray = response.Split('-');
                            byte[] responseAsBytes = new byte[responseArray.Length];
                            for(int i = 0; i < responseArray.Length; i++)
                            {
                               responseAsBytes[i] = Convert.ToByte(responseArray[i], 16);
                            }
                            byte[] messageAsBytes = Encoding.ASCII.GetBytes(pInput[2]);

                            if( rsa.VerifyData(messageAsBytes, CryptoConfig.MapNameToOID("SHA1"), responseAsBytes))
                            {
                                responseString = "Message was successfully signed";
                                return responseString;
                            }
                            else
                            {
                                responseString = "Message was not successfully signed";
                                return responseString;
                            }
                        }
                        else
                        {
                            responseString = "Client doesn't yet have the public key";
                            return responseString;
                        }
                    case ("AddFifty"):
                        if(publicKey != null)
                        {
                            aes.GenerateKey();
                            aesKey = aes.Key;
                            aes.GenerateIV();
                            ivKey = aes.IV;
                            rsa.FromXmlString(publicKey);
                            int intput = 0;
                            try
                            {
                                intput = int.Parse(pInput[2]);
                            }
                            catch { responseString = "A valid Integer must be given!"; return responseString; }
                            byte[] inputAsByte = BitConverter.GetBytes(intput);
                            byte[] signedMessage = rsa.Encrypt(inputAsByte, true);
                            byte[] signedAesKey = rsa.Encrypt(aesKey, true);
                            byte[] signedIvKey = rsa.Encrypt(ivKey, true);

                            string signedMessageAsString = BitConverter.ToString(signedMessage);
                            string signedAesKeyAsString = BitConverter.ToString(signedAesKey);
                            string signedIvKeyAsString = BitConverter.ToString(signedIvKey);



                            path += "/addfifty?encryptedInteger=" + signedMessageAsString + "&encryptedsymkey=" + signedAesKeyAsString + "&encryptedIV=" + signedIvKeyAsString;
                            httpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);

                            HttpResponseMessage signResponse = await httpClient.GetAsync(path);
                            string response = await signResponse.Content.ReadAsStringAsync();

                            response = response.TrimStart('"').TrimEnd('"');
                            string[] responseArray = response.Split('-');
                            byte[] responseMessageAsBytes = new byte[responseArray.Length];
                            for (int i = 0; i < responseArray.Length; i++)
                            {
                                responseMessageAsBytes[i] = Convert.ToByte(responseArray[i], 16);
                            }
                            string text = "";

                            ICryptoTransform decryptor = aes.CreateDecryptor();
                            using (MemoryStream memoryStream = new MemoryStream(responseMessageAsBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                                    {
                                        text = streamReader.ReadToEnd();
                                    }
                                }
                            }

                            int test;
                            try
                            {
                                test = int.Parse(text);
                            }
                            catch
                            {
                                responseString = "An error Occured";
                                return responseString;
                            }

                            return text;

                        }
                        else
                        {
                            responseString = "You need to do a User Post or User Set first";
                            return responseString;
                        }
                    default:
                        responseString = "Invalid Request";
                        return responseString;
                }
            }
        }

        static async Task RunAsync(string[] pInput)
        {
            switch (pInput[0])
            {
                case ("TalkBack"):
                    Console.Clear();
                    Task<string> talkBackTask = TalkBack(pInput);
                    Console.WriteLine("...please wait...");
                    if(await Task.WhenAny(talkBackTask, Task.Delay(20000)) == talkBackTask)
                    { Console.Clear(); Console.WriteLine(talkBackTask.Result); }
                    else { Console.WriteLine("Request Timed Out"); }
                    break;
                case ("User"):
                    Console.Clear();
                    Task<string> usertask = User(pInput);
                    Console.WriteLine("...please wait...");
                    if (await Task.WhenAny(usertask, Task.Delay(20000)) == usertask)
                    { Console.Clear(); Console.WriteLine(usertask.Result); }
                    else { Console.WriteLine("Request Timed Out"); }
                    break;
                case ("Protected"):
                    Console.Clear();
                    Task<string> protectedTask = Protected(pInput);
                    Console.WriteLine("...please wait...");
                    if (await Task.WhenAny(protectedTask, Task.Delay(20000)) == protectedTask)
                    { Console.Clear(); Console.WriteLine(protectedTask.Result); }
                    else { Console.WriteLine("Request Timed Out"); }
                    break;
                default:
                    Console.WriteLine("Invalid Request");
                    break;
            }
        }
    }
    #endregion
}
