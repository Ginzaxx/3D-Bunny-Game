using UnityEngine;
using System.Collections;

public enum WeatherType
{
    DayDry,        // Siang Kering - Normal
    AfternoonDry,  // Sore Kering  - Rubah muncul sering
    Snow           // Salju        - Speed turun, wortel salju sering
}

/// <summary>
/// WeatherManager - Mengatur cuaca dan efek visualnya
/// Attach ke GameObject WeatherManager
/// </summary>
public class WeatherManager : MonoBehaviour
{
    [Header("Current Weather")]
    public WeatherType CurrentWeather = WeatherType.DayDry;

    [Header("Weather Objects")]
    public GameObject snowParticleSystem;
    public GameObject sunEffect;           // cahaya terang siang
    public GameObject afternoonLightEffect; // cahaya sore / oranye

    [Header("Skybox Materials")]
    public Material daySkybox;
    public Material afternoonSkybox;
    public Material snowSkybox;

    [Header("Lighting")]
    public Light directionalLight;
    public Color dayLightColor = new Color(1f, 0.95f, 0.8f);
    public Color afternoonLightColor = new Color(1f, 0.6f, 0.2f);
    public Color snowLightColor = new Color(0.8f, 0.9f, 1f);

    [Header("Auto Cycle (opsional)")]
    public bool autoCycle = false;
    public float cycleDuration = 60f; // detik per cuaca

    private float cycleTimer = 0f;
    private int weatherIndex = 0;

    void Start()
    {
        ApplyWeather(CurrentWeather);

        if (autoCycle)
            StartCoroutine(WeatherCycleLoop());
    }

    IEnumerator WeatherCycleLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleDuration);
            weatherIndex = (weatherIndex + 1) % 3;
            ChangeWeather((WeatherType)weatherIndex);
        }
    }

    public void ChangeWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;
        ApplyWeather(newWeather);
        Debug.Log($"[WeatherManager] Cuaca berubah: {newWeather}");
    }

    void ApplyWeather(WeatherType weather)
    {
        // Reset semua
        if (snowParticleSystem != null) snowParticleSystem.SetActive(false);
        if (sunEffect != null) sunEffect.SetActive(false);
        if (afternoonLightEffect != null) afternoonLightEffect.SetActive(false);

        switch (weather)
        {
            case WeatherType.DayDry:
                ApplyDayWeather();
                break;
            case WeatherType.AfternoonDry:
                ApplyAfternoonWeather();
                break;
            case WeatherType.Snow:
                ApplySnowWeather();
                break;
        }
    }

    void ApplyDayWeather()
    {
        if (sunEffect != null) sunEffect.SetActive(true);
        if (daySkybox != null) RenderSettings.skybox = daySkybox;
        if (directionalLight != null)
        {
            directionalLight.color = dayLightColor;
            directionalLight.intensity = 1.2f;
        }
    }

    void ApplyAfternoonWeather()
    {
        if (afternoonLightEffect != null) afternoonLightEffect.SetActive(true);
        if (afternoonSkybox != null) RenderSettings.skybox = afternoonSkybox;
        if (directionalLight != null)
        {
            directionalLight.color = afternoonLightColor;
            directionalLight.intensity = 0.9f;
        }
    }

    void ApplySnowWeather()
    {
        if (snowParticleSystem != null) snowParticleSystem.SetActive(true);
        if (snowSkybox != null) RenderSettings.skybox = snowSkybox;
        if (directionalLight != null)
        {
            directionalLight.color = snowLightColor;
            directionalLight.intensity = 0.7f;
        }
    }

    // Dipanggil dari UI button / GameManager
    public void SetDayDry()      => ChangeWeather(WeatherType.DayDry);
    public void SetAfternoonDry() => ChangeWeather(WeatherType.AfternoonDry);
    public void SetSnow()        => ChangeWeather(WeatherType.Snow);
}
