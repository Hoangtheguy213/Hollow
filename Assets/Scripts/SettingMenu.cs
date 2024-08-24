using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SettingMenu : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    // Start is called before the first frame update

    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void SetFullSceen(bool _isFullSceen)
    {
        Screen.fullScreen = _isFullSceen;
    }
    public void Quit()
    {
        Application.Quit();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
