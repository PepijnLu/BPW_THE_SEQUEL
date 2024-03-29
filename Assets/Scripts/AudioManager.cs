using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource menuTheme, gameTheme, battleTheme, gameOverTheme, fireSFX, blockSFX, gameOverSFX, enemyDeathSFX, dodgeSFX, critSFX, exitSFX, chestSFX, pickupSFX, maxOrbSFX, hitSFX, doorUnlockSFX, pickCardSFX;
    public Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
    public Slider musicSlider, sfxSlider;

    void Awake()
    {
        instance = this;
        audioSources.Add("menuTheme", menuTheme);
        audioSources.Add("gameTheme", gameTheme);
        audioSources.Add("battleTheme", battleTheme);
        audioSources.Add("fireSFX", fireSFX);
        audioSources.Add("blockSFX", blockSFX);
        audioSources.Add("gameOverSFX", gameOverSFX);
        audioSources.Add("enemyDeathSFX", enemyDeathSFX);
        audioSources.Add("dodgeSFX", dodgeSFX);
        audioSources.Add("critSFX", critSFX);
        audioSources.Add("exitSFX", exitSFX);
        audioSources.Add("pickupSFX", pickupSFX);
        audioSources.Add("maxOrbSFX", maxOrbSFX);
        audioSources.Add("hitSFX", hitSFX);
        audioSources.Add("openRoomSFX", doorUnlockSFX);
        audioSources.Add("chestSFX", chestSFX);
        audioSources.Add("pickCardSFX", pickCardSFX);
        audioSources.Add("gameOverTheme", gameOverTheme);
    }

    void Start()
    {
        foreach (KeyValuePair<string, AudioSource> pair in audioSources)
        {
            AudioSource audio = pair.Value;
            string audioString = pair.Key;
            if (audio.isPlaying)
            {
                if (audioString.Contains("Theme"))
                {
                    audio.volume = GameData.musicVolume;
                }
                else if (audioString.Contains("SFX"))
                {
                    audio.volume = GameData.sfxVolume;
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            StartCoroutine(AudioFadeIn(audioSources["menuTheme"], 2f));
            PlaySound(audioSources["menuTheme"]);
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            StartCoroutine(AudioFadeIn(audioSources["gameTheme"], 2f));
            PlaySound(audioSources["gameTheme"]);
        }
        if (SceneManager.GetActiveScene().name == "GameOver")
        {
            StartCoroutine(AudioFadeIn(audioSources["gameOverTheme"], 2f));
            PlaySound(audioSources["gameOverTheme"]);
        }

        if ((musicSlider != null) && (sfxSlider != null))
        {
            musicSlider.value = GameData.musicVolume;
            sfxSlider.value = GameData.sfxVolume;
        }

    }

    public void PlaySound(AudioSource audio)
    {
        //audio.volume = (GameData.sfxVolume / 100);
        audio.Play();
    }

    public IEnumerator AudioFadeIn(AudioSource audio, float duration)
    {
        audio.UnPause();
        audio.volume = 0f;
        while (audio.volume < GameData.musicVolume)
        {
            audio.volume += Time.deltaTime / duration;
            yield return null;
        }
        audio.volume = GameData.musicVolume;
    }   

    public IEnumerator AudioFadeOut(AudioSource audio, float duration)
    {
        audio.volume = GameData.musicVolume;
        while (audio.volume > 0f)
        {
            audio.volume -= Time.deltaTime / duration;
            yield return null;
        }
        audio.volume = 0;
        audio.Pause();
    }   

    public void UpdateMusicVolume()
    {
        GameData.musicVolume = musicSlider.value;
        ChangeAudioLevels();
    }

    public void UpdateSFXVolume()
    {
        GameData.sfxVolume = sfxSlider.value;
        ChangeAudioLevels();
    }

    public void ChangeAudioLevels()
    {
        foreach (KeyValuePair<string, AudioSource> pair in audioSources)
        {
            AudioSource audio = pair.Value;
            string audioString = pair.Key;
            if (audio.isPlaying)
            {
                if (audioString.Contains("Theme"))
                {
                    audio.volume = GameData.musicVolume;
                }
                else if (audioString.Contains("SFX"))
                {
                    audio.volume = GameData.sfxVolume;
                }
            }
        }
    }
    
}
