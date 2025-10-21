using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    const string ManagersPath = "Boot/Managers"; // Resources/Boot/Managers.prefab
    static bool spawned;

    void Awake()
    {
        if (spawned) return;
        var prefab = Resources.Load<GameObject>(ManagersPath);
        if (prefab) { Instantiate(prefab); spawned = true; }
        else Debug.LogWarning("[Bootstrap] Managers prefab not found at Resources/Boot/Managers");
    }
}
