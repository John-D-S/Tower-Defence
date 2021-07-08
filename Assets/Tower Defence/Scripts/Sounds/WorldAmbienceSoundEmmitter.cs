using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static HelperClasses.HelperFunctions;

public class WorldAmbienceSoundEmmitter : MonoBehaviour
{
    [SerializeField]
    private AudioSource LowAltitudeAmbience;
    [SerializeField]
    private float minBlendRange;
    [SerializeField]
    private AudioSource HighAltitudeAmbience;
    [SerializeField]
    private float maxBlendRange;

    private float lowAltitudeAmbienceMaxVolume;
    private float highAltitudeAmbienceMaxVolume;

    private void Start()
    {
        lowAltitudeAmbienceMaxVolume = LowAltitudeAmbience.volume;
        highAltitudeAmbienceMaxVolume = HighAltitudeAmbience.volume;
    }

    private void FixedUpdate()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float terrainHeight = TargetHeight(ConvertToVector2(cameraPosition), 200f);
        transform.position = new Vector3(cameraPosition.x, terrainHeight, cameraPosition.z);
        float unscaledHighAltitudeAmbienceVolume = Mathf.Clamp01((terrainHeight - minBlendRange) / (maxBlendRange - minBlendRange));
        float unscaledlowAltitudeAmbienceVolume = Mathf.Abs(unscaledHighAltitudeAmbienceVolume - 1);
        LowAltitudeAmbience.volume = unscaledlowAltitudeAmbienceVolume * lowAltitudeAmbienceMaxVolume;
        HighAltitudeAmbience.volume = unscaledHighAltitudeAmbienceVolume * highAltitudeAmbienceMaxVolume;
    }
}
