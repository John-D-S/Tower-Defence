using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    [Header("-- Song Settings --")]
    [SerializeField, Tooltip("A list of songs that will play")]
    private List<AudioClip> songs;
    //the song that is currently playing
    private AudioClip currentSong;
    //the audioSource that plays the music.
    private AudioSource MusicPlayer;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        MusicPlayer = GetComponent<AudioSource>();
        StartCoroutine(StartMusic());
    }

    /// <summary>
    /// start playing songs
    /// </summary>
    private IEnumerator StartMusic()
    {
        //play a random song until it stops, then play another
        while (true)
        {
            MusicPlayer.Stop();
            MusicPlayer.clip = songs[Random.Range(0, songs.Count)];
            currentSong = MusicPlayer.clip;
            MusicPlayer.Play();
            yield return new WaitForSeconds(currentSong.length + 5);
        }
    }
}
