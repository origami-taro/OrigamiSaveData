using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Origami.Cryptography
{
    /// <summary>
    /// 文字列の暗号化・復号化を行うクラスです。
    /// </summary>
    public class StringEncryptor
    {
        /// <summary>
        /// 指定するブロックサイズです。
        /// </summary>
        private const int BlockSize = 128;

        /// <summary>
        /// 指定する暗号鍵の長さです。
        /// </summary>
        private const int KeySize = 256;


        /// <summary>
        /// 初期化ベクトル
        /// 半角16文字(1byte=8bit, 8bit*16=128bit)
        /// </summary>
        private string _aesIV = @"AAAAAAAAAAAAAAAA";

        /// <summary>
        /// 暗号化鍵
        /// 半角32文字(8bit*32文字=256bit)
        /// </summary>
        private string _aesKey = @"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="key">暗号化鍵</param>
        public StringEncryptor(string iv, string key)
        {
            _aesIV = iv;
            _aesKey = key;
        }

        /// <summary>
        /// 暗号化を行います。
        /// </summary>
        /// <param name="plainText">暗号化したい文字列</param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                // AESの設定
                aes.BlockSize = BlockSize;
                aes.KeySize = KeySize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(_aesIV);
                aes.Key = Encoding.UTF8.GetBytes(_aesKey);

                // 暗号化を行うICryptoTransformを作成します。
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // MemoryStreamから暗号化されたバイトを文字列にして返します。
            return System.Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// 復号を行う関数です。
        /// </summary>
        /// <param name="cipherText">暗号化されたデータ</param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            string plainText = null;

            using(Aes aes = Aes.Create())
            {
                // AESの設定
                aes.BlockSize = BlockSize;
                aes.KeySize = KeySize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(_aesIV);
                aes.Key = Encoding.UTF8.GetBytes(_aesKey);

                // 復号化を行うICryptoTransformを生成します。
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // CryptoStreamから復号化されたバイトを読み取ります。
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }
    }
}
