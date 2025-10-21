using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{
    public void SaveString(string key, string value) => PlayerPrefs.SetString(key, value);
    public string LoadString(string key, string def = "") => PlayerPrefs.GetString(key, def);
    public void Flush() => PlayerPrefs.Save();
}
