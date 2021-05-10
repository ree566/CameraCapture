using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Camera_Record
{
    class RSAService
    {
        //產生公鑰及私鑰

        public static Tuple<string, string> GenerateRSAKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);

            return Tuple.Create<string, string>(publicKey, privateKey);
        }

        //加密字串

        public static string Encrypt(string publicKey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var encryptString = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), false));

            return encryptString;
        }

        //解密字串

        public static string Decrypt(string privateKey, string encryptedContent)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            var decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedContent), false));

            return decryptString;
        }

        //加密檔案

        public static void EncryptFile(string publicKey, string rawFilePath, string encryptedFilePath)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            using (FileStream testDataStream = File.OpenRead(rawFilePath))
            using (FileStream encrytpStream = File.OpenWrite(encryptedFilePath))
            {
                var testDataByteArray = new byte[testDataStream.Length];
                testDataStream.Read(testDataByteArray, 0, testDataByteArray.Length);

                var encryptDataByteArray = rsa.Encrypt(testDataByteArray, false);

                encrytpStream.Write(encryptDataByteArray, 0, encryptDataByteArray.Length);
            }
        }

        //解密檔案

        public static void DecryptFile(string privateKey, string encryptedFilePath, string decryptedFilePath)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            using (FileStream encrytpStream = File.OpenRead(encryptedFilePath))
            using (FileStream decrytpStream = File.OpenWrite(decryptedFilePath))
            {
                var encryptDataByteArray = new byte[encrytpStream.Length];
                encrytpStream.Read(encryptDataByteArray, 0, encryptDataByteArray.Length);

                var decryptDataByteArray = rsa.Decrypt(encryptDataByteArray, false);

                decrytpStream.Write(decryptDataByteArray, 0, decryptDataByteArray.Length);
            }
        }
    }
}
