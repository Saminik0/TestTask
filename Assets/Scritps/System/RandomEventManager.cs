using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RandomEventManager : MonoBehaviour
{
    [System.Serializable]
    public class TimedEvent
    {
        public string name;
        public float minDelay = 10f;
        public float maxDelay = 25f;
        public UnityEvent onTrigger;
    }

    public TimedEvent[] events;
    public bool randomizeOrder = true;
    public float initialDelayMin = 3f;
    public float initialDelayMax = 10f;

    void Start()
    {
        if (events.Length > 0)
            StartCoroutine(RunRandomEvents());
    }

    IEnumerator RunRandomEvents()
    {
        yield return new WaitForSeconds(Random.Range(initialDelayMin, initialDelayMax)); // ⏳ ждём немного перед первым событием

        while (true)
        {
            int index = randomizeOrder ? Random.Range(0, events.Length) : 0;
            var e = events[index];
            yield return new WaitForSeconds(Random.Range(e.minDelay, e.maxDelay));
            e.onTrigger.Invoke();
        }
    }
}
