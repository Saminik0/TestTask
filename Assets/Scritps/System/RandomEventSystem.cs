using UnityEngine;

public class RandomEventSystem : Singleton<RandomEventSystem>
{
    [Header("Intervals (sec)")]
    public float minInterval = 60f;
    public float maxInterval = 180f;

    // тут позже можно повесить список событий и логику запуска
}
