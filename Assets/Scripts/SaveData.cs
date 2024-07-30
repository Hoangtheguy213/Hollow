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
    public int playerHeartShards;
    public float playerMana;
    public bool playerHalfMana;
    public int playerManaOrbs;
    public int playerOrbShard;
    public float playerOrb0Fill, playerOrb1Fill, playerOrb2Fill;
    public Vector2 playerPos;
    public string lastScene;

    public bool playerUnlockWallJump;
    public bool playerUnlockDash;
    public bool playerUnlockVarJump;

    public bool playerUnlockSideSpell, playerUnlockUpSpell, playerUnlockDownSpell;

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
            playerHeartShards = Player.Instance.heartShards;
            writer.Write(playerHeartShards);
            playerHalfMana = Player.Instance.halfMana;
            writer.Write(playerHalfMana);
            playerMana = Player.Instance.Mana;
            writer.Write(playerMana);
            playerManaOrbs = Player.Instance.manaOrbs;
            writer.Write(playerManaOrbs);
            playerOrbShard = Player.Instance.orbShard;
            writer.Write(playerOrbShard);
            playerOrb0Fill = Player.Instance.manaOrbsHandler.orbFills[0].fillAmount;
            writer.Write(playerOrb0Fill);
            playerOrb1Fill = Player.Instance.manaOrbsHandler.orbFills[1].fillAmount;
            writer.Write(playerOrb1Fill);
            playerOrb2Fill = Player.Instance.manaOrbsHandler.orbFills[2].fillAmount;
            writer.Write(playerOrb2Fill);


            playerUnlockWallJump = Player.Instance.unlockWallJump;
            writer.Write(playerUnlockWallJump);

            playerUnlockDash = Player.Instance.unlockDash;
            writer.Write(playerUnlockDash);

            playerUnlockVarJump = Player.Instance.unlockVarJump;
            writer.Write(playerUnlockVarJump);

            playerUnlockSideSpell = Player.Instance.unlockSideSpell;
            writer.Write(playerUnlockSideSpell);
            playerUnlockUpSpell = Player.Instance.unlockUpSpell;
            writer.Write(playerUnlockUpSpell);
            playerUnlockDownSpell = Player.Instance.unlockDownSpell;
            writer.Write(playerUnlockDownSpell);

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
                playerHeartShards = reader.ReadInt32();
                playerHalfMana = reader.ReadBoolean();
                playerMana = reader.ReadSingle();
                playerManaOrbs = reader.ReadInt32();
                playerOrbShard = reader.ReadInt32();
                playerOrb0Fill = reader.ReadSingle();
                playerOrb1Fill = reader.ReadSingle();
                playerOrb2Fill = reader.ReadSingle();

                playerUnlockWallJump = reader.ReadBoolean();
                playerUnlockDash= reader.ReadBoolean();
                playerUnlockVarJump= reader.ReadBoolean();

                playerUnlockSideSpell = reader.ReadBoolean();
                playerUnlockUpSpell= reader.ReadBoolean();
                playerUnlockDownSpell= reader.ReadBoolean();

                playerPos.x = reader.ReadSingle();
                playerPos.y = reader.ReadSingle();
                lastScene = reader.ReadString();
                
                
                Player.Instance.Health = playerHealth;
                Player.Instance.heartShards = playerHeartShards;
                Player.Instance.halfMana = playerHalfMana;
                Player.Instance.Mana = playerMana;
                Player.Instance.manaOrbs = playerManaOrbs;
                Player.Instance.orbShard = playerOrbShard;
                Player.Instance.manaOrbsHandler.orbFills[0].fillAmount = playerOrb0Fill;
                Player.Instance.manaOrbsHandler.orbFills[1].fillAmount = playerOrb1Fill;
                Player.Instance.manaOrbsHandler.orbFills[2].fillAmount = playerOrb2Fill;


                Player.Instance.unlockWallJump = playerUnlockWallJump;
                Player.Instance.unlockDash = playerUnlockDash;
                Player.Instance.unlockVarJump = playerUnlockVarJump;

                Player.Instance.unlockSideSpell = playerUnlockSideSpell;
                Player.Instance.unlockUpSpell = playerUnlockUpSpell;
                Player.Instance.unlockDownSpell = playerUnlockDownSpell;

                Player.Instance.transform.position = playerPos;
                SceneManager.LoadScene(lastScene);
            }
        }
        else
        {
            Debug.Log("file doesn't exist");
            
            Player.Instance.Health = Player.Instance.maxHealth;
            Player.Instance.heartShards = 0;
            Player.Instance.halfMana = false;
            Player.Instance.Mana = 0.5f;
            

            Player.Instance.unlockWallJump = false;
            Player.Instance.unlockDash = false;
            Player.Instance.unlockVarJump= false;

            Player.Instance.unlockSideSpell = false;
            Player.Instance.unlockUpSpell = false;
            Player.Instance.unlockDownSpell = false;
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
