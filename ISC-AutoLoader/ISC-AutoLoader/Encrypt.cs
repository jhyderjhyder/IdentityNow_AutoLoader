using System.Security.Cryptography;
using System;
using System.Text;


namespace ISC_AutoLoader
{/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.dpapidataprotector?view=netframework-4.8.1
/// https://www.c-sharpcorner.com/article/encryption-and-decryption-using-a-symmetric-key-in-c-sharp/
/// </summary>
    public class Encrypt
    {
        public Encrypt(String input) {
            if (input != null)
            {
                key = input;
            }
            else
            {
                Console.WriteLine("Danger using default key");
            }

        }

        private string key = "b14ca5898a4e4133bbce2ea2315a1916";



        private string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(this.key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public String encrypt(string input)
        {

            var encryptedString = EncryptString(input);
            return encryptedString;

        }
        public string decrypt(string input)
        {
            return DecryptString(input);

        }
            

    }
}

    

