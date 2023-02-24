using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scaffold;
using UnityEngine.UI;
using AppAdvisory.MathGame;

public class SettingManager : Manager<SettingManager>
{
    public Slider musicSlider;
    public Slider soundEffectSlider;

    [Space()]
    public GameManager gameManager;


    public void MusicSlider()
    {
        gameManager.ChangeMusicAudio(musicSlider.value);
    }

    public void SoundEffectVolume()
    {
        gameManager.ChangeSoundEffectAudio(soundEffectSlider.value);
    }
}
