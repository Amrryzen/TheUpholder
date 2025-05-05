using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider volumeSlider;

    private float lastVolume = 1f; // Simpan volume sebelum mute

    void Start()
    {
        volumeSlider.value = musicSource.volume;
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        lastVolume = musicSource.volume;
    }

    public void ToggleMute()
    {
        if (!musicSource.mute)
        {
            // Simpan volume saat ini sebelum mute
            lastVolume = musicSource.volume;
            musicSource.mute = true;
            volumeSlider.value = 0f; // Slider ikut ke 0
        }
        else
        {
            musicSource.mute = false;
            musicSource.volume = lastVolume; // Balikin volume sebelumnya
            volumeSlider.value = lastVolume;
        }

        Debug.Log("Mute status: " + musicSource.mute);
    }

    void ChangeVolume(float value)
    {
        musicSource.volume = value;

        // Kalau volume > 0, unmute otomatis
        if (musicSource.mute && value > 0f)
        {
            musicSource.mute = false;
        }

        // Update lastVolume biar selalu terbaru kalau belum mute
        if (!musicSource.mute)
        {
            lastVolume = value;
        }
    }
}
