using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
using UnityEngine.Audio;

public class StructureAudio : MonoBehaviour
{
    [Header("-- Structure Audio Settings --")]
    [SerializeField, Tooltip("The Structure script of the associated structure")]
    private Structure structure;
    [SerializeField, Tooltip("The audiosource that plays the structure's sound")]
    private AudioSource structureAudio;
    [SerializeField, Tooltip("The speed at which the volume lerps up and down when the structure is activated or deactivated")]
    private float volumeLerpSpeed = 1;

    //the starting volume of the structure
    private float originalVolume;
    private bool currentlyFullVolume = false;
    private bool currentlyNoVolume = false;

    float distanceFromPlayer;
    /// <summary>
    /// updates whether the audio is audible based on its distance from the main camera.
    /// </summary>
    IEnumerator UpdateAudioAudibility()
    {
        while (gameObject)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
            if (distanceFromPlayer < structureAudio.maxDistance + 10)
            {
                ToggleAudioSource(true);
            }
            else
            {
                ToggleAudioSource(false);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// pauses or unpauses the audio according to isAudible
    /// </summary>
    void ToggleAudioSource(bool isAudible)
    {
        if (!isAudible && structureAudio.isPlaying)
        {
            structureAudio.Pause();
        }
        else if (isAudible && !structureAudio.isPlaying)
        {
            structureAudio.Play();
        }
    }

    private void Start()
    {
        //initializing variables and starting the updateAudioAudibility coroutine
        originalVolume = structureAudio.volume;
        structureAudio.volume = 0;
        StartCoroutine(UpdateAudioAudibility());
    }

    private void FixedUpdate()
    {
        //if the structure can function, raise the volume of the structure and if it can't, lower it.
        if (structure.CanFunction)
        {
            if (!currentlyFullVolume)
            {
                if (structureAudio.volume < originalVolume - 0.001f)
                {
                    structureAudio.volume = Mathf.Lerp(structureAudio.volume, originalVolume, Time.fixedDeltaTime * volumeLerpSpeed);
                    currentlyNoVolume = false;
                }
                else
                {
                    structureAudio.volume = originalVolume;
                    currentlyFullVolume = true;
                }
            }
        }
        else if (!currentlyNoVolume)
        {
            if (structureAudio.volume > 0.001f)
            {
                structureAudio.volume = Mathf.Lerp(structureAudio.volume, 0, Time.fixedDeltaTime * volumeLerpSpeed);
                currentlyFullVolume = false;
            }
            else
            {
                structureAudio.volume = 0;
                currentlyNoVolume = true;
            }
        }
    }
}
