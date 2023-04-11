using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RSAChat
{
    public class RSAHelper
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        public byte[] GetKeyPair(bool includePrivateKey)
        {
            byte[] key = null;
            //Get the key that was generated when we made the class
            //RSAParameters rsaKeyInfo = rsa.ExportParameters(includePrivateKey);
            string keyInfoXML = rsa.ToXmlString(includePrivateKey);
            key = Encoding.UTF8.GetBytes(keyInfoXML);
            return key;
        }

        public List<string> GetAllMessages(string privatekey, string myDir)
        {
            List<string> messages = new List<string>();

            this.LoadPrivateKey(privatekey);
            foreach(string file in Directory.GetFiles(myDir))
            {
                byte[] contents = File.ReadAllBytes(file);
                //byte[] encryptedText = Encoding.UTF8.GetBytes(contents);
                var x = rsa.Decrypt(contents, true);
                messages.Add(file+ ": " +Encoding.UTF8.GetString(x));
            }

            return messages;
        }
        public void LoadPrivateKey(string filepath)
        {
            try
            {
                string x = File.ReadAllText(filepath);
                rsa.FromXmlString(x);
            } catch
            {
                throw;
            }
        }

        public void LoadPublicKey(string filepath)
        {
            try
            {
                string x = File.ReadAllText(filepath);
                rsa.FromXmlString(x);
            } catch
            {
                throw;
            }
        }

        public void Encrypt(string filepath, string message, string publicKey)
        {
            //Much better
            byte[] plainTextMessage = Encoding.UTF8.GetBytes(message);
            LoadPublicKey(publicKey);
            byte[] encryptedMessage = rsa.Encrypt(plainTextMessage,true);
            File.WriteAllBytes(filepath, encryptedMessage);
        }


    }
}
