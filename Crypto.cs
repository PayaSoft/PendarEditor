namespace Paya.Cryptography
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// 	Specifies the <see cref="Crypto" /> class
    /// </summary>
    public static class Crypto
    {
        #region Constants

        private const int RsaKeySize = 512;

        #endregion

        #region Fields

        private static readonly byte[] _Salt = Encoding.ASCII.GetBytes("A4662A559A5D416FAE8EADD1DF4F530D");

        private static readonly HashAlgorithm _SimpleHasher = SHA1.Create();

        #endregion

        #region Public Methods

        public static byte[] DecryptTDES(byte[] toDecryptArray, string key, bool useHasing)
        {
            byte[] keyArray;

            if (useHasing)
            {
                using (var hashmd = new MD5CryptoServiceProvider())
                {
                    keyArray = hashmd.ComputeHash(Encoding.UTF8.GetBytes(key));
                    hashmd.Clear();
                }
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(key);
            }

            using (var tDes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                using (ICryptoTransform cTransform = tDes.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                    tDes.Clear();
                    return resultArray;
                }
            }
        }

        public static byte[] EncryptTDES(byte[] toEncryptArray, string Key, bool useHasing)
        {
            return EncryptTDES(toEncryptArray, Encoding.UTF8.GetBytes(Key), useHasing);
        }

        public static byte[] EncryptTDES(byte[] toEncryptArray, byte[] keyArray, bool useHasing)
        {
            if (useHasing)
            {
                using (var hashmd5 = new MD5CryptoServiceProvider())
                {
                    keyArray = hashmd5.ComputeHash(keyArray);
                    hashmd5.Clear();
                }
            }

            using (var tDes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                using (ICryptoTransform cTransform = tDes.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    tDes.Clear();
                    return resultArray;
                }
            }
        }

        public static byte[] EncryptAndSign(byte[] data, string key, string rsaXmlPrivateKeys = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            byte[] crypt1 = EncryptTDES(data, key, true);
            byte[] sign = rsaXmlPrivateKeys != null ? null : GetSignature(crypt1, key);

            byte[] initialData;
            using (var ms = new MemoryStream())
            {
                using (var br = new BinaryWriter(ms))
                {
                    br.Write(crypt1.Length);
                    br.Write(crypt1);
                    if (sign != null)
                    {
                        br.Write(sign.Length);
                        br.Write(sign);
                    }
                }

                initialData = ms.ToArray();
            }

            if (rsaXmlPrivateKeys == null)
            {
                return initialData;
            }

            var csParameters = new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore };

            using (var rsa = new RSACryptoServiceProvider(RsaKeySize, csParameters))
            {
                rsa.FromXmlString(rsaXmlPrivateKeys);
                var digitalSignatureData = rsa.SignData(initialData, CryptoConfig.MapNameToOID("MD5"));
                using (var ms2 = new MemoryStream())
                {
                    using (var bw = new BinaryWriter(ms2))
                    {
                        bw.Write(initialData.Length);
                        bw.Write(digitalSignatureData.Length);
                        bw.Write(initialData);
                        bw.Write(digitalSignatureData);
                    }

                    //return EncryptTDES(ms2.ToArray(), key, true);
                    return ms2.ToArray();
                }
            }
        }

        public static byte[] DecryptAndVerify(byte[] cipherData, string key, string rsaXmlPublicKey = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (cipherData == null)
            {
                throw new ArgumentNullException("cipherData");
            }
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            byte[] rsaData;
            if (rsaXmlPublicKey != null)
            {
                using (var ms = new MemoryStream(cipherData))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        int rsaDataLength = br.ReadInt32();
                        int rsaSignDataLength = br.ReadInt32();
                        rsaData = br.ReadBytes(rsaDataLength);
                        byte[] rsaSignData = br.ReadBytes(rsaSignDataLength);

                        var csParameters = new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore };

                        using (var rsa = new RSACryptoServiceProvider(RsaKeySize, csParameters))
                        {
                            rsa.FromXmlString(rsaXmlPublicKey);
                            if (!rsa.VerifyData(rsaData, CryptoConfig.MapNameToOID("MD5"), rsaSignData))
                            {
                                throw new CryptographicException("Invalid signature for data");
                            }
                        }
                    }
                }
            }
            else
            {
                rsaData = cipherData;
            }

            using (var ms = new MemoryStream(rsaData, false))
            {
                using (var br = new BinaryReader(ms))
                {
                    int dataLength = br.ReadInt32();
                    byte[] data = br.ReadBytes(dataLength);
                    if (rsaXmlPublicKey == null)
                    {
                        int signLength = br.ReadInt32();
                        byte[] sign = br.ReadBytes(signLength);
                        if (!Equals(GetSignature(data, key), sign))
                        {
                            throw new CryptographicException("Invalid signature");
                        }
                    }

                    var plain = DecryptTDES(data, key, true);

                    return plain;
                }
            }
        }

        public static void GenerateRsaKeys(out string privateKey, out string publicKey)
        {
            var csParameters = new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore };
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize, csParameters))
            {
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
            }
        }

        public static string Compress(string text)
        {
            return Convert.ToBase64String(CompressData(text));
        }

        public static byte[] CompressData(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            return CompressData(buffer);
        }

        public static byte[] CompressData(byte[] buffer)
        {
            if (buffer == null)
                return null;
            using (var ms = new MemoryStream())
            {
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(buffer, 0, buffer.Length);

                ms.Position = 0;

                var compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);

                var gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                return gzBuffer;
            }
        }

        public static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            return DecompressData(gzBuffer);
        }

        public static string DecompressData(byte[] gzBuffer)
        {
            using (var ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                var buffer = new byte[msgLength];

                ms.Position = 0;
                var zip = new GZipStream(ms, CompressionMode.Decompress);
                zip.Read(buffer, 0, buffer.Length);


                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// 	Decrypt the given string. Assumes the string was encrypted using EncryptAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText"> The text to decrypt. </param>
        /// <param name="sharedSecret"> A password used to generate a key for decryption. </param>
        public static string DecryptAES(string cipherText, string sharedSecret)
        {
            if (sharedSecret == null)
                throw new ArgumentNullException("sharedSecret");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (cipherText == null)
                cipherText = string.Empty;

            return DecryptAES(Convert.FromBase64String(cipherText), sharedSecret);
        }

        /// <summary>
        /// 	Decrypt the given string. Assumes the string was encrypted using EncryptAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="bytes"> The bytes to decrypt. </param>
        /// <param name="sharedSecret"> A password used to generate a key for decryption. </param>
        public static string DecryptAES(byte[] bytes, string sharedSecret)
        {
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            try
            {
                // generate the key from the shared secret and the salt
                using (var key = new Rfc2898DeriveBytes(sharedSecret, _Salt))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    using (aesAlg = new RijndaelManaged())
                    {
                        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                        aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                        // Create a decrytor to perform the stream transform.
                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                        // Create the streams used for decryption.                
                        using (var msDecrypt = new MemoryStream(bytes))
                        {
                            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                            var srDecrypt = new StreamReader(csDecrypt);

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        public static string DecryptAndVerify(string cipherText, string key, bool compression)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (cipherText == null)
                throw new ArgumentNullException("cipherText");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            string text2 = DecryptTDES(cipherText, key, true);
            string[] parts = text2.Split('\r');
            if (parts.Length >= 2)
            {
                string encrypted = parts[0];
                string sign = parts[1];
                if (GetSignature(encrypted, key) != sign)
                    throw new CryptographicException("Invalid signature");
                string text = DecryptAES(encrypted, key);
                return compression ? Decompress(text) : text;
            }

            throw new CryptographicException("Invalid string");
        }

        public static string DecryptTDES(string cypherString, string key, bool useHasing)
        {
            byte[] keyArray;
            byte[] toDecryptArray = Convert.FromBase64String(cypherString);

            if (useHasing)
            {
                using (var hashmd = new MD5CryptoServiceProvider())
                {
                    keyArray = hashmd.ComputeHash(Encoding.UTF8.GetBytes(key));
                    hashmd.Clear();
                }
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            using (var tDes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                using (ICryptoTransform cTransform = tDes.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                    tDes.Clear();
                    return Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
                }
            }
        }

        /// <summary>
        /// 	Encrypt the given string using AES. The string can be decrypted using DecryptAES(). <para>The sharedSecret parameters must match.</para>
        /// </summary>
        /// <param name="plainText"> The text to encrypt. </param>
        /// <param name="sharedSecret"> A password used to generate a key for encryption. </param>
        public static string EncryptAES(string plainText, string sharedSecret)
        {
            return Convert.ToBase64String(EncryptAES(plainText.ToCharArray(), sharedSecret));
        }

        public static byte[] EncryptAES(char[] data, string sharedSecret)
        {
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                using (DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _Salt))
                {

                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                    // Create a decrytor to perform the stream transform.
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        // Create the streams used for encryption.
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (TextWriter swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    // Write all data to the stream.
                                    swEncrypt.Write(data, 0, data.Length);
                                }
                            }

                            return msEncrypt.ToArray();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
        }

        [JetBrains.Annotations.NotNull]
        public static string EncryptAndSign([JetBrains.Annotations.CanBeNull]string text, [JetBrains.Annotations.NotNull]string key, bool compression)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (text == null)
                text = string.Empty;

            if (compression)
                text = Compress(text);
            string crypt1 = EncryptAES(text, key);
            string sign = GetSignature(crypt1, key);
            string text2 = crypt1 + "\r" + sign;
            string crypto2 = EncryptTDES(text2, key, true);

            return crypto2;
        }

        public static string EncryptTDES(string ToEncrypt, string Key, bool useHasing)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(ToEncrypt);

            var resultArray = EncryptTDES(toEncryptArray, Key, useHasing);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string GetHexString(byte[] data)
        {
            if (data == null)
                return null;
            var sb = new StringBuilder();

            foreach (byte b in data)
            {
                var c = (char)b;
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else
                    sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string GetLicenseNumber(string cipher)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(cipher));
                var l = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    if (i % 4 == 0 && i > 0)
                        l.Append('-');
                    l.Append(hash[i].ToString("X", CultureInfo.InvariantCulture));
                }

                return l.ToString();
            }
        }

        public static string GetSignature(string data, string key)
        {
            using (var h = new HMACMD5(Encoding.UTF8.GetBytes(key)))
            {
                return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }

        public static byte[] GetSignature(byte[] data, string key)
        {
            return GetSignature(data, Encoding.UTF8.GetBytes(key));
        }
        public static byte[] GetSignature(byte[] data, byte[] key)
        {
            using (var h = new HMACMD5(key))
            {
                return h.ComputeHash(data);
            }
        }


        public static string Hash(string input)
        {
            var bytes = _SimpleHasher.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty).Concat(_Salt).ToArray());
            return GetHexString(bytes);
        }

        /// <summary>
        /// The equivalent of the SQL server <code>HASHBYTES('sha1',N'')</code> function.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static byte[] HashBytes(string input)
        {
            if (input == null)
                return null;

            var b = Encoding.UTF8.GetBytes(input);
            return _SimpleHasher.ComputeHash(b);
        }

        #endregion
    }
}