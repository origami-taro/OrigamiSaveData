
using Newtonsoft.Json;
using Origami.Cryptography;
using System.IO;
using UnityEngine;

namespace Origami.SaveData
{
    /// <summary>
    /// セーブデータの読み込み・書き込みを行うクラスです。
    /// </summary>
    public class SaveDataHelper
    {
        /// <summary>
        /// 暗号化・復号化を行うクラスです。
        /// </summary>
        private StringEncryptor _encryptor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="encryptor">暗号化クラス</param>
        public SaveDataHelper(StringEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        /// <summary>
        /// 読み込みを行います。
        /// </summary>
        /// <typeparam name="SaveData"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="saveData"></param>
        /// <param name="decryption">復号化を行う</param>
        /// <returns></returns>
        public bool Load<SaveData>(string fileName, out SaveData saveData, bool decryption = true)
            where SaveData : SaveDataBase
        {
            string filePath = Application.persistentDataPath + "/" + fileName;

            // ファイルが存在しなければ読み込まない
            if (File.Exists(filePath) == false)
            {
                saveData = null;
                return false;
            }

            string json = string.Empty;
            using (StreamReader stream = new StreamReader(filePath))
            {
                if (decryption)
                {
                    // 復号化を行う
                    var cipherText = stream.ReadToEnd();
                    json = _encryptor.Decrypt(cipherText);
                }
                else
                {
                    json = stream.ReadToEnd();
                }
            }

            saveData = JsonConvert.DeserializeObject<SaveData>(json);

            return true;
        }

        /// <summary>
        /// 保存を行います。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveData"></param>
        /// <param name="encryption">暗号化を行う</param>
        public void Save(string fileName, SaveDataBase saveData, bool encryption = true)
        {
            string filePath = Application.persistentDataPath + "/" + fileName;
            string json = JsonConvert.SerializeObject(saveData);

            using (StreamWriter stream = new StreamWriter(filePath, false))
            {
                if (encryption)
                {
                    // 暗号化を行う
                    var encrypted = _encryptor.Encrypt(json);
                    stream.WriteLine(encrypted);
                }
                else
                {
                    // 文字列のまま出力する
                    stream.WriteLine(json);
                }
            }
        }
    }
}
