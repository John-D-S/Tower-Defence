using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> songs;
    private AudioClip currentSong;
    private AudioSource MusicPlayer;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        MusicPlayer = GetComponent<AudioSource>();
        StartCoroutine(StartMusic());
    }

    private IEnumerator StartMusic()
    {
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
