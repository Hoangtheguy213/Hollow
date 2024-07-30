using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heartController : MonoBehaviour
{
    // Start is called before the first frame update
    Player player;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsparent;
    public GameObject heartContainerPrefabs;
    void Start()
    {
        player = Player.Instance;
        heartContainers = new GameObject[Player.Instance.maxTotalHealth];
        heartFills = new Image[Player.Instance.maxTotalHealth];

        Player.Instance.onHealthChangedCallBack += UpdateHeartHUD;
        InstantiateHeartContainers();
        UpdateHeartHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetHeartContainer()
    {
        for(int i = 0; i < heartContainers.Length; i++)
        {
            if (i < Player.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < Player.Instance.health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }
    void InstantiateHeartContainers()
    {
        for( int i = 0; i<Player.Instance.maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefabs);
            temp.transform.SetParent(heartsparent, false);
            heartContainers[i] = temp;
            heartFills[i] =temp.transform.Find("heart").GetComponent<Image>();
        } 
    }
    void UpdateHeartHUD()
    {
        SetFilledHearts();
        SetHeartContainer();
    }
}
