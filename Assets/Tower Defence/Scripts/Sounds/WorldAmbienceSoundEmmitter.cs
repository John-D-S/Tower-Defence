using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static HelperClasses.HelperFunctions;

public class WorldAmbienceSoundEmmitter : MonoBehaviour
{
    [Header("-- Ambience Settings --")]
    [SerializeField, Tooltip("The audio source that plays the low altitude ambience sound")]
    private AudioSource LowAltitudeAmbience;
    [SerializeField, Tooltip("below this height, the low altitude ambience volume will be at its max and the high altitude ambience will be at its min")]
    private float minBlendRange;
    [SerializeField, Tooltip("The audio source that plays the high altitude ambience sound")]
    private AudioSource HighAltitudeAmbience;
    [SerializeField, Tooltip("Above this height, the heigh altitude ambience volume will be at its max and the low altitude ambience will be at its min")]
    private float maxBlendRange;
    [SerializeField, Tooltip("How fast the Ambient volume lerps")]
    private float blendLerpSpeed = 1;

    //the maximum volume for each respective audio source
    private float lowAltitudeAmbienceMaxVolume;
    private float highAltitudeAmbienceMaxVolume;

    private void Start()
    {
        //set the max volume for each audio source to be the volume it starts at
        lowAltitudeAmbienceMaxVolume = LowAltitudeAmbience.volume;
        highAltitudeAmbienceMaxVolume = HighAltitudeAmbience.volume;
    }

    private void FixedUpdate()
    {
        //gets the current camera position
        Vector3 cameraPosition = Camera.main.transform.position;
        //the terrain height at the current camera position
        float terrainHeight = TargetHeight(ConvertToVector2(cameraPosition), 200f);
        //set the position of this transform to be the point on the terrain directly below the main camera
        transform.position = new Vector3(cameraPosition.x, terrainHeight, cameraPosition.z);
        //set the volume of the low and high altitude ambient volumes.
        float unscaledHighAltitudeAmbienceVolume = Mathf.Clamp01((terrainHeight - minBlendRange) / (maxBlendRange - minBlendRange));
        float unscaledlowAltitudeAmbienceVolume = Mathf.Abs(unscaledHighAltitudeAmbienceVolume - 1);
        LowAltitudeAmbience.volume = Mathf.Lerp(LowAltitudeAmbience.volume, unscaledlowAltitudeAmbienceVolume * lowAltitudeAmbienceMaxVolume, Time.fixedDeltaTime * blendLerpSpeed);
        HighAltitudeAmbience.volume = Mathf.Lerp(HighAltitudeAmbience.volume, unscaledHighAltitudeAmbienceVolume * highAltitudeAmbienceMaxVolume, Time.fixedDeltaTime * blendLerpSpeed);
    }
}
