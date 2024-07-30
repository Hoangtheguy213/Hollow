using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour
{
    bool used;
    [SerializeField] GameObject particals;
    [SerializeField] GameObject canvasUI;

    [SerializeField] HeartShard heartShard;
    // Start is called before the first frame update
    void Start()
    {
        if (Player.Instance.maxHealth >= Player.Instance.maxTotalHealth)
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
        heartShard.initialFillAmount = Player.Instance.heartShards * 0.25f;
        Player.Instance.heartShards++;
        heartShard.targetFillAmount = Player.Instance.heartShards * 0.25f;

        StartCoroutine(heartShard.LerpFill());

        yield return new WaitForSeconds(2.5f);
        canvasUI.SetActive(false);
        SaveData.Instance.SavePlayerData();
        Destroy(gameObject);
    }
}
