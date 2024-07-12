using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public SceneFader sceneFader;
    public static UIManager Instance;

    [SerializeField] GameObject deathSceen;

    [SerializeField] GameObject halfMana, fullMana;

    public enum ManaState
    {
        FullMana,
        HalfMana
    }
    public ManaState manaState;
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
        sceneFader = GetComponentInChildren<SceneFader>(); 
    }

    public void SwitchMana(ManaState _manaState)
    {
        switch(_manaState)
        {
            case ManaState.FullMana:
                halfMana.SetActive(false);
                fullMana.SetActive(true);
                break;
            case ManaState.HalfMana:
                halfMana.SetActive(true);
                fullMana.SetActive(false);
                break;
        }
        manaState = _manaState;
    }
    public IEnumerator ActivateDeathSceen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
        yield return new WaitForSeconds(0.8f);
        deathSceen.SetActive(true);
    }
    public IEnumerator DeactiveDeathSceen()
    {
        yield return new WaitForSeconds(0.5f);
        deathSceen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
}
