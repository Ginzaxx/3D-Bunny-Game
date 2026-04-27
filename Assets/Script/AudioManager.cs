using UnityEngine;

/// <summary>
/// AudioManager - Singleton untuk semua suara game
/// Attach ke GameObject AudioManager
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    public AudioClip bgmDay;
    public AudioClip bgmAfternoon;
    public AudioClip bgmSnow;
    public AudioClip bgmMenu;

    [Header("SFX")]
    public AudioClip sfxCatchCarrotNormal;
    public AudioClip sfxCatchCarrotFrozen;
    public AudioClip sfxFoxAppear;
    public AudioClip sfxFoxClick;
    public AudioClip sfxFoxMissed;
    public AudioClip sfxTimerLow;
    public AudioClip sfxGameOver;
    public AudioClip sfxButtonClick;
    public AudioClip sfxSeasonChange;
    public AudioClip sfxGameWin;
    public AudioClip sfxGameLose;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ============================
    //   Music
    // ============================

    public void PlayMenuMusic()   => PlayMusic(bgmMenu);
    public void PlayDayMusic()    => PlayMusic(bgmDay);
    public void PlayAfternoonMusic() => PlayMusic(bgmAfternoon);
    public void PlaySnowMusic()   => PlayMusic(bgmSnow);

    public void PlayMusicForWeather(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.DayDry:        PlayDayMusic(); break;
            case WeatherType.AfternoonDry:  PlayAfternoonMusic(); break;
            case WeatherType.Snow:          PlaySnowMusic(); break;
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = 0.8f; // Set default music volume to 0.8
        musicSource.Play();
    }

    // ============================
    //   SFX
    // ============================

    public void PlayCatchNormal()    => PlaySFX(sfxCatchCarrotNormal);
    public void PlayCatchFrozen()    => PlaySFX(sfxCatchCarrotFrozen);
    public void PlayFoxAppear()      => PlaySFX(sfxFoxAppear, 2.0f); // Volume 2x
    public void PlayFoxClick()       => PlaySFX(sfxFoxClick, 3.0f);  // Volume 2x
    public void PlayFoxMissed()      => PlaySFX(sfxFoxMissed, 3.0f); // Volume 2x
    public void PlayTimerLow()       => PlaySFX(sfxTimerLow);
    public void PlayGameOver()       => PlaySFX(sfxGameOver);
    public void PlayButtonClick()    => PlaySFX(sfxButtonClick);
    public void PlaySeasonChange()   => PlaySFX(sfxSeasonChange);
    public void PlayWin()            => PlaySFX(sfxGameWin);
    public void PlayLose()           => PlaySFX(sfxGameLose);

    void PlaySFX(AudioClip clip, float volumeMultiplier = 1.0f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }

    public void SetMusicVolume(float vol)
    {
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(vol);
    }

    public void SetSFXVolume(float vol)
    {
        if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(vol);
    }
}
