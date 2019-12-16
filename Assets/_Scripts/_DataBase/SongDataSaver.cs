using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SongDataSaver
{
    public static void SaveData(TaikoGameInstance TS)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Song.taiko";

        FileStream file = new FileStream(path, FileMode.Create);

        SongData songData = new SongData(TS);

        formatter.Serialize(file, songData);

        file.Close();
    }

    public static SongData LoadData()
    {
        string path = Application.persistentDataPath + "/Song.taiko";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = new FileStream(path, FileMode.Open);

            SongData songData = formatter.Deserialize(file) as SongData;

            file.Close();

            return songData;
        }
        else
        {
            Debug.Log("file not exist");
            return null;
        }
    }

    public static bool DoesFilesExist()
    {
        string path = Application.persistentDataPath + "/Song.taiko";
        return File.Exists(path);
    }

    public static string GetDefaultTJAPath()
    {
        string tmpPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        tmpPath = tmpPath + "\\TaikoSim\\";
        if (!Directory.Exists(tmpPath))
        {
            //if it doesn't, create it

            Directory.CreateDirectory(tmpPath);
            if (!Directory.Exists(tmpPath + @"\Songs\"))
            {
                Directory.CreateDirectory(tmpPath + @"\Songs\");
                return tmpPath;
            }
            else
            {
                return tmpPath;
            }

        }
        else
        {
            return tmpPath;
        }
    }

}
