using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioClip musicLoop;

    [SerializeField] public float sfxVolume;
    [SerializeField] public float musicVolume;

    public float currentMusicVolume;

    private void Awake()
    {
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
        LoopMusic(musicLoop, true);
    }

    private void Update()
    {
        sfxSource.volume = sfxVolume;
        musicSource.volume = currentMusicVolume;
    }

    public void PlaySFXOneShot(AudioClip clip)
    {
        sfxSource.volume = sfxVolume;
        sfxSource.pitch = 1f + Random.Range(-0.15f, 0.15f);
        sfxSource.PlayOneShot(clip);
    }

    public void LoopMusic(AudioClip music, bool loop = true)
    {
        musicSource.volume = musicVolume;
        musicSource.clip = music;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    public IEnumerator FadeMusicIn()
    {
        float t = 0f;
        while (t < musicVolume)
        {
            t += Time.deltaTime / musicVolume;
            currentMusicVolume = t;
            yield return null;
        }
    }

    public IEnumerator FadeMusicOut()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / musicVolume;
            currentMusicVolume = t;
            yield return null;
        }
    }
}
