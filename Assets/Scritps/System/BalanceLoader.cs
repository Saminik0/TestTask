using UnityEngine;

[System.Serializable]
public class GameplayConfig
{
    public float boilerRepairTime = 2f;
    public float lampFlickerMin = 8f;
    public float lampFlickerMax = 15f;
}

public class BalanceLoader : MonoBehaviour
{
    [Header("JSON из сабмодул€ (TextAsset)")]
    public TextAsset gameplayJson;         // перетащи Assets/BalanceData/Data/gameplay.json
    public bool useBalance = false;        // по умолчанию ¬џ Ћ Ч сцена не зависит от данных

    public static GameplayConfig Data { get; private set; }

    void Awake()
    {
        // дефолты, если данных нет/выключено
        Data = new GameplayConfig();

        if (!useBalance) return;

        if (gameplayJson == null || string.IsNullOrEmpty(gameplayJson.text))
        {
            Debug.LogWarning("[Balance] JSON not assigned or empty. Using defaults.");
            return;
        }

        try
        {
            var parsed = JsonUtility.FromJson<GameplayConfig>(gameplayJson.text);
            if (parsed != null)
            {
                if (parsed.boilerRepairTime <= 0f) parsed.boilerRepairTime = 2f;
                if (parsed.lampFlickerMin < 0f) parsed.lampFlickerMin = 8f;
                if (parsed.lampFlickerMax <= parsed.lampFlickerMin)
                    parsed.lampFlickerMax = parsed.lampFlickerMin + 5f;

                Data = parsed;
                Debug.Log("[Balance] Config loaded.");
            }
        }
        catch
        {
            Debug.LogWarning("[Balance] Parse error. Using defaults.");
        }
    }
}
