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
        SaveScene();

        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }
    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        if(SaveData.Instance.benchSceneName != null)// load the bench's scene if exist
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }
        if(SaveData.Instance.benchPos != null)// set the respawn to the bench's pos
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