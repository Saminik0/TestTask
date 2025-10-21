using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float flickerDuration = 1.5f;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1.5f;
    public AudioSource audioSource;
    public AudioClip flickerSound;

    private Light lamp;
    private bool isFlickering;

    void Awake()
    {
        lamp = GetComponent<Light>();
    }

    public void TriggerFlicker()
    {
        if (!isFlickering)
            StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        isFlickering = true;
        if (audioSource && flickerSound)
            audioSource.PlayOneShot(flickerSound);

        float t = 0f;
        while (t < flickerDuration)
        {
            lamp.intensity = Random.Range(minIntensity, maxIntensity);
            lamp.enabled = Random.value > 0.1f;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            t += Time.deltaTime;
        }

        lamp.enabled = true;
        lamp.intensity = maxIntensity;
        isFlickering = false;
    }
}
