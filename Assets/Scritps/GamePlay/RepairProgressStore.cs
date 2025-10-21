using UnityEngine;

public static class RepairProgressStore
{
    public static void SaveProgress(string id, float progress01)
    {
        PlayerPrefs.SetFloat($"repair_{id}", Mathf.Clamp01(progress01));
        PlayerPrefs.Save();
    }

    public static float LoadProgress(string id)
    {
        return PlayerPrefs.GetFloat($"repair_{id}", 0f);
    }
}
