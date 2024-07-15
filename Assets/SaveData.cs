using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.Mathematics;


[System.Serializable]
public struct SaveData 
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;

    //bench stuff
    public string benchSceneName;
    public Vector2 benchPos;

    //player stuff
    public int playerHealth;
    public float playerMana;
    public bool playerHalfMana;
    public Vector2 playerPos;
    public string lastScene;

    //enemy stuff
    //Shade stuff
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.bench.data"))//if file doesnt exist , create new file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }
        if(!File.Exists(Application.persistentDataPath + "/save.player.data"))//if file doesnt exist , create new file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if(!File.Exists(Application.persistentDataPath + "/save.shade.data"))//if file doesnt exist , create new file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.shade.data"));
        }

        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveBench()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }

    public void LoadBench()
    {
        if(File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {

            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
    }
    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = Player.Instance.Health; 
            writer.Write(playerHealth);

            playerMana = Player.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = Player.Instance.halfMana;
            writer.Write(playerHalfMana);   

            playerPos = Player.Instance.transform.position;
            writer.Write(playerPos.x);
            writer.Write(playerPos.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);    
        }
    }
    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerHalfMana = reader.ReadBoolean();
                playerMana = reader.ReadSingle();
                playerPos.x = reader.ReadSingle();
                playerPos.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                Player.Instance.transform.position = playerPos;
                Player.Instance.Health = playerHealth;
                Player.Instance.halfMana = playerHalfMana;
                Player.Instance.Mana = playerMana;
                

            }
        }
        else
        {
            Debug.Log("file doesn't exist");
            Player.Instance.halfMana = false;
            Player.Instance.Health = Player.Instance.maxHealth;
            Player.Instance.Mana = 0.5f;
        }
    }

    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);

            writer.Write(shadePos.x);
            writer.Write(shadePos.y);

            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }
    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.data")))
            {
                sceneWithShade= reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle(); 
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                shadeRot = new Quaternion( rotationX, rotationY, rotationZ,rotationW);

            }
        }
        else
        {
            Debug.Log("shade doesnt exist");
        }
    }
    public void DeletePlayerData()
    {
        string path = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Player save data deleted.");
        }
    }

    public void DeleteShadeData()
    {
        string path = Application.persistentDataPath + "/save.shade.data";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Shade save data deleted.");
        }
    }

    public void DeleteAllSaveData()
    {
        //DeleteBenchData();
        DeletePlayerData();
        DeleteShadeData();
    }
}
