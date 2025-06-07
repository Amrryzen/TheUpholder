using UnityEngine;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    public AudioSource musicSource;

    // Fungsi untuk dipanggil dari Slider
    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Fungsi untuk mute musik
    public void MuteMusic()
    {
        musicSource.mute = true;
    }

    // Fungsi untuk unmute musik
    public void UnmuteMusic()
    {
        musicSource.mute = false;
    }

    // Atau toggle mute (jika pakai satu tombol saja)
    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
    }
}
