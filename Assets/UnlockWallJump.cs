using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class UnlockWallJump : MonoBehaviour
{
    bool used;
    [SerializeField] GameObject particals;
    [SerializeField] GameObject canvasUI;
    // Start is called before the first frame update
    void Start()
    {
        if (Player.Instance.unlockWallJump)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if(_collision.CompareTag("Player") & !used)
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

        yield return new WaitForSeconds(4f);
        Player.Instance.unlockWallJump = true;
        SaveData.Instance.SavePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
