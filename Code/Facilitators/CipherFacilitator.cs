using System;
using System.IO;
using System.Security.Cryptography;

namespace indyClient
{
    class CipherFacilitator
    {
        private RijndaelManaged d_rmCrypto;

        public CipherFacilitator()
        {
            d_rmCrypto = new RijndaelManaged();
            d_rmCrypto.GenerateIV();
            d_rmCrypto.GenerateKey();
        }

        public string getKey()
        {
            return Convert.ToBase64String(d_rmCrypto.Key);
        }

        public string getIV()
        {
            return Convert.ToBase64String(d_rmCrypto.IV);
        }

        public void setKey(string key)
        {
            d_rmCrypto.Key = Convert.FromBase64String(key);
        }

        public void setIV(string iv)
        {
            d_rmCrypto.IV = Convert.FromBase64String(iv);
        }

        public string encrypt(string plaintext)
        {
            try
            {
                ICryptoTransform encryptor =
                    d_rmCrypto.CreateEncryptor(d_rmCrypto.Key, d_rmCrypto.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                        new CryptoStream(msEncrypt, encryptor,
                        CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt =
                            new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plaintext);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while encrypting the medical dossier: {e.Message}");
                throw e;
            }
        }

        public string decrypt(string cipherText)
        {
            try
            {
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor =
                    d_rmCrypto.CreateDecryptor(d_rmCrypto.Key, d_rmCrypto.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt =
                    new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            string plaintext = srDecrypt.ReadToEnd();
                            return plaintext;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while decrypting the medical dossier: {e.Message}");
                throw e;
            }

        }

    }
}
