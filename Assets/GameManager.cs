using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public Vector2 platFormingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] bench Bench;
    public static GameManager Instance { get; private set; }

    public GameObject Shade;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        Bench = FindObjectOfType<bench>();
    }
    public void RespawnPlayer()
    {
        if(Bench != null)
        {
            if (Bench.interacted)
            {
                respawnPoint = Bench.transform.position;
            }
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