using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddManaOrb : MonoBehaviour
{
    bool used;
    [SerializeField] GameObject particals;
    [SerializeField] GameObject canvasUI;

    [SerializeField] OrbShard orbShard;
    // Start is called before the first frame update
    void Start()
    {
        if (Player.Instance.manaOrbs >=3)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") & !used)
        {
            used = true;
            GameObject _particals = Instantiate(particals, transform.position, Quaternion.identity);
            Destroy(_particals, 0.5f);
            StartCoroutine(ShowUI());

        }
    }
    IEnumerator ShowUI()
    {
        GameObject _particals = Instantiate(particals, transform.position, Quaternion.identity);
        Destroy(_particals, 0.5f);
        yield return new WaitForSeconds(0.5f);
       
        canvasUI.SetActive(true);
        orbShard.initialFillAmount = Player.Instance.orbShard * 0.34f;
        Player.Instance.orbShard++;
        orbShard.targetFillAmount = Player.Instance.orbShard * 0.34f;

        StartCoroutine(orbShard.LerpFill());

        yield return new WaitForSeconds(2.5f);
        
        canvasUI.SetActive(false);
        SaveData.Instance.SavePlayerData();
        
        Destroy(gameObject);
    }
}
