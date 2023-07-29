using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataFolder : MonoBehaviour
{
    // TODO: to change
    public NoteData data;

    private void Awake()
    {
        if (!Directory.Exists(Path.Join(GetRootPath(), data.SaveDataDirname)))
        {
            Directory.CreateDirectory(Path.Join(GetRootPath(), data.SaveDataDirname));
        }
    }

    public static string GetRootPath()
    {
        return Application.persistentDataPath;
    }

    public string GetDataPath()
    {
        return Path.Join(GetRootPath(), data.SaveDataDirname);
    }

    public string GetDataFileFullPath(string filename)
    {
        return Path.Join(GetDataPath(), filename);
    }
}
