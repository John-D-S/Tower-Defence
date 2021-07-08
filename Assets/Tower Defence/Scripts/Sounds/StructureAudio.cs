using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
using UnityEngine.Audio;

public class StructureAudio : MonoBehaviour
{
    [SerializeField]
    private Structure structure;
    [SerializeField]
    private AudioSource structureAudio;
    [SerializeField]
    private float volumeLerpSpeed = 1;

    private float originalVolume;
    private void Start()
    {
        originalVolume = structureAudio.volume;
        structureAudio.volume = 0;
    }

    private void FixedUpdate()
    {
        if (structure.CanFunction)
            if (structureAudio.volume < originalVolume - 0.001f)
            {
                structureAudio.volume = Mathf.Lerp(structureAudio.volume, originalVolume, Time.fixedDeltaTime * volumeLerpSpeed);
            }
        else
            structureAudio.volume = 0;
    }
}
