using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct SaveData 
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;

    public void Initialize()
    {
        if(sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
}
