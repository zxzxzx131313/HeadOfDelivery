using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataFolderHelper : MonoBehaviour
{
    // TODO: to change
    //public NoteData data;


    public static string GetRootPath()
    {
        return Application.persistentDataPath;
    }

    public string GetOrCreateDirFullPath(string DataDirName)
    {
        if (!Directory.Exists(Path.Join(GetRootPath(), DataDirName)))
        {
            Directory.CreateDirectory(Path.Join(GetRootPath(), DataDirName));
        }

        return Path.Join(GetRootPath(), DataDirName);
    }

}
