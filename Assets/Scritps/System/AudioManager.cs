using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("One-shot source")]
    public AudioSource oneShotSource;

    public void PlayOneShot(AudioClip clip)
    {
        if (clip && oneShotSource) oneShotSource.PlayOneShot(clip);
    }
}
