using System;
using System.IO;
using UnityEngine;

namespace Systems.SaveLoad
{
    public class ObfuscatedSaveService : ISaveService
    {
        private readonly string _saveFileName = "gamedata.sav";
        private readonly string _checksumKey = "data_integrity";

        private readonly byte[] _obfuscationKey = { 0x7A, 0x36, 0x51, 0x4B, 0x2D, 0x18, 0x63, 0x49 };

        public void SaveData<T>(T data, string identifier)
        {
            try
            {
                var json = JsonUtility.ToJson(data);
                var checksum = CalculateChecksum(json);

                var saveData = new SecureSaveData
                {
                    Data = json,
                    Checksum = checksum,
                    Timestamp = DateTime.UtcNow.Ticks,
                    Identifier = identifier
                };

                var serializedData = JsonUtility.ToJson(saveData);
                var obfuscatedData = ObfuscateData(serializedData);

                var savePath = Path.Combine(Application.persistentDataPath, _saveFileName);
                File.WriteAllBytes(savePath, obfuscatedData);

                Debug.Log($"Data saved successfully to {savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
        }

        public T LoadData<T>(string identifier) where T : class, new()
        {
            try
            {
                var savePath = Path.Combine(Application.persistentDataPath, _saveFileName);

                if (!File.Exists(savePath))
                {
                    Debug.LogWarning("Save file not found");
                    return new T();
                }

                var obfuscatedData = File.ReadAllBytes(savePath);
                var deobfuscatedData = DeobfuscateData(obfuscatedData);
                var saveData = JsonUtility.FromJson<SecureSaveData>(deobfuscatedData);

                var calculatedChecksum = CalculateChecksum(saveData.Data);
                if (calculatedChecksum != saveData.Checksum)
                {
                    Debug.LogError("Save data corrupted or tampered with!");
                    return new T(); // Return default data
                }

                if (saveData.Identifier != identifier)
                {
                    Debug.LogError("Save data identifier mismatch!");
                    return new T();
                }

                return JsonUtility.FromJson<T>(saveData.Data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
                return new T();
            }
        }

        private byte[] ObfuscateData(string data)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= _obfuscationKey[i % _obfuscationKey.Length];
            }

            return bytes;
        }

        private string DeobfuscateData(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= _obfuscationKey[i % _obfuscationKey.Length];
            }

            return System.Text.Encoding.UTF8.GetString(data);
        }

        private string CalculateChecksum(string data)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data + _checksumKey));
                return Convert.ToBase64String(hash);
            }
        }
    }
}