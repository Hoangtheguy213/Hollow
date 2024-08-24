using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public Vector2 platFormingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Bench bench;
    public static GameManager Instance { get; private set; }

    public GameObject shade;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;
    private void Awake()
    {
        SaveData.Instance.Initialize();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (Player.Instance != null)
        {
            if (Player.Instance.halfMana)
            {
                SaveData.Instance.LoadShadeData();
                if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                {
                    Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                }
            }
        }
        SaveScene();

        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();

        /* The line below is used for del all the temp save*/
        SaveData.Instance.DeleteAllSaveData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveData.Instance.SavePlayerData();
            Debug.Log("Player data saved");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }
    public void RespawnPlayer()
    {

        SaveData.Instance.LoadBench();

        if (SaveData.Instance.benchSceneName != null)// load the bench's scene if exist
        {
            //transitionedFromScene = SceneManager.GetActiveScene().name;
            transitionedFromScene = SaveData.Instance.benchSceneName;
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }
        if (SaveData.Instance.benchPos != null)// set the respawn to the bench's pos
        {
            respawnPoint = SaveData.Instance.benchPos;
        }
        else
        {
            respawnPoint = platFormingRespawnPoint;
        }
        Player.Instance.transform.position = respawnPoint;

        StartCoroutine(UIManager.Instance.DeactiveDeathSceen());
        Player.Instance.Respawned();

    }
}