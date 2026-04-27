using UnityEngine;
using System.Collections;

public enum WeatherType
{
    DayDry,        // Siang Kering - Normal
    AfternoonDry,  // Sore Kering  - Rubah muncul sering
    Snow           // Salju        - Speed turun, wortel salju sering
}

/// <summary>
/// WeatherManager - Mengatur cuaca dan efek visualnya.
/// Cuaca sekarang berubah berdasarkan score (lihat ScoreManager.CheckWeatherThreshold).
/// Score >= 61  -> AfternoonDry
/// Score >= 121 -> Snow
/// Attach ke GameObject WeatherManager
/// </summary>
public class WeatherManager : MonoBehaviour
{
    [Header("Current Weather")]
    public WeatherType CurrentWeather = WeatherType.DayDry;

    [Header("Weather Objects")]
    public GameObject snowParticleSystem;
    public GameObject sunEffect;
    public GameObject afternoonLightEffect;

    [Header("Skybox Materials")]
    public Material daySkybox;
    public Material afternoonSkybox;
    public Material snowSkybox;

    [Header("Lighting")]
    public Light directionalLight;
    public Color dayLightColor       = new Color(1f, 0.95f, 0.8f);
    public Color afternoonLightColor = new Color(1f, 0.6f,  0.2f);
    public Color snowLightColor      = new Color(0.8f, 0.9f, 1f);

    void Start()
    {
        ApplyWeather(CurrentWeather);
    }

    public void ChangeWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;
        ApplyWeather(newWeather);
        AudioManager.Instance?.PlaySeasonChange();
        Debug.Log($"[WeatherManager] Cuaca berubah ke: {newWeather}");
    }

    void ApplyWeather(WeatherType weather)
    {
        // Reset semua efek dulu
        if (snowParticleSystem  != null) snowParticleSystem.SetActive(false);
        if (sunEffect           != null) sunEffect.SetActive(false);
        if (afternoonLightEffect != null) afternoonLightEffect.SetActive(false);

        AudioManager.Instance?.PlayMusicForWeather(weather);

        switch (weather)
        {
            case WeatherType.DayDry:      ApplyDayWeather();       break;
            case WeatherType.AfternoonDry: ApplyAfternoonWeather(); break;
            case WeatherType.Snow:         ApplySnowWeather();      break;
        }
    }

    void ApplyDayWeather()
    {
        if (sunEffect   != null) sunEffect.SetActive(true);
        if (daySkybox   != null) RenderSettings.skybox = daySkybox;
        if (directionalLight != null)
        {
            directionalLight.color     = dayLightColor;
            directionalLight.intensity = 1.2f;
        }
    }

    void ApplyAfternoonWeather()
    {
        if (afternoonLightEffect != null) afternoonLightEffect.SetActive(true);
        if (afternoonSkybox      != null) RenderSettings.skybox = afternoonSkybox;
        if (directionalLight != null)
        {
            directionalLight.color     = afternoonLightColor;
            directionalLight.intensity = 0.9f;
        }
    }

    void ApplySnowWeather()
    {
        if (snowParticleSystem != null) snowParticleSystem.SetActive(true);
        if (snowSkybox         != null) RenderSettings.skybox = snowSkybox;
        if (directionalLight != null)
        {
            directionalLight.color     = snowLightColor;
            directionalLight.intensity = 0.7f;
        }
    }

    // Bisa tetap dipanggil dari UI button atau GameManager jika perlu override manual
    public void SetDayDry()       => ChangeWeather(WeatherType.DayDry);
    public void SetAfternoonDry() => ChangeWeather(WeatherType.AfternoonDry);
    public void SetSnow()         => ChangeWeather(WeatherType.Snow);
}