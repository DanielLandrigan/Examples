using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Cawblin
{
    public class SaveFileDataWriter
    {
        public string SaveDataDirectoryPath = string.Empty;
        public string SaveDataFileName = string.Empty;

        public bool FileExists()
        {
            return File.Exists(Path.Combine(SaveDataDirectoryPath, SaveDataFileName));
        }

        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(SaveDataDirectoryPath, SaveDataFileName));
        }

        public void CreateNewSaveFile(CharacterSaveData characterSaveData)
        {
            string savePath = Path.Combine(SaveDataDirectoryPath, SaveDataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating save file at " + savePath);

                string saveData = JsonUtility.ToJson(characterSaveData, true);

                using (FileStream stream = new(savePath, FileMode.Create))
                {
                    using(StreamWriter fileWriter = new(stream))
                    {
                        fileWriter.Write(saveData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterSaveData = null;

            string loadPath = Path.Combine(SaveDataDirectoryPath, SaveDataFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string loadData = string.Empty;

                    using (FileStream stream = new(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new(stream))
                        {
                            loadData = reader.ReadToEnd();
                        }
                    }

                    characterSaveData = JsonUtility.FromJson<CharacterSaveData>(loadData);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }

            return characterSaveData;
        }
    }
}
