using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundTrigger : MonoBehaviour
{
    public AudioClip[] sounds;
    public float volume = 0.7f;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false; // ✅ гарантировано выключаем автовоспроизведение
        source.loop = false;
    }

    public void PlayRandomSound()
    {
        if (sounds == null || sounds.Length == 0) return;
        var clip = sounds[Random.Range(0, sounds.Length)];
        source.PlayOneShot(clip, volume);
    }
}
