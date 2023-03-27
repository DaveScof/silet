using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scaffold;
using UnityEngine.UI;
using AppAdvisory.MathGame;
using TMPro;

public class SettingManager : Manager<SettingManager>
{
    public Slider musicSlider;
    public Slider soundEffectSlider;
    public TMP_Dropdown languageDropDown;

    [Space()]

    public Tongues.Language english;
    public Tongues.Language amharic;

    [Space()]
    public GameManager gameManager;

    private void Start()
    {
        english = Tongues.LanguageControl.ActiveLanguage;
        
    }

    public void MusicSlider()
    {
        gameManager.ChangeMusicAudio(musicSlider.value);
    }

    public void SoundEffectVolume()
    {
        gameManager.ChangeSoundEffectAudio(soundEffectSlider.value);
    }

    public void ChangeToEnglish()
    {
        Tongues.LanguageControl.SetLanguage(english);
    }

    public void ChangeToAmharic()
    {
        Tongues.LanguageControl.SetLanguage(amharic);
    }

    public void LogoutClicked()
    {
        MenuBarouch.MenuManager.Instance.Logout();
    }

    public void OnLanguageChanged()
    {
        int val = languageDropDown.value;
        if (val == 0)
        {
            ChangeToEnglish();
        }
        else
        {
            ChangeToAmharic();
        }
    }
    

}
