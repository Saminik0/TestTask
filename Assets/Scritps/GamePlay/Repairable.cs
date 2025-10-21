// Assets/Scripts/RepairSystem/Repairable.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Repairable : MonoBehaviour, IInteractable
{
    [Header("IDs")]
    public string repairId = "boiler_01";

    [Header("Links")]
    public ParticleSystem sparksVfx;
    public AudioSource audioSource;
    public AudioClip startClip;
    public AudioClip successClip;
    public RepairMinigameController minigame;

    [Header("State")]
    [SerializeField] bool _isBroken = true;
    [Range(0f, 1f)][SerializeField] float _progress = 0f;

    [Header("Prompts")]
    [Tooltip("���������, ����� ������ ������")]
    public string promptWhenBroken = "������� [E], ����� ��������";
    [Tooltip("���������, ����� ������ ��������")]
    public string promptWhenOk = "������������ �������� (R � �������)";

    [Header("Debug hotkeys (��� ������)")]
    public bool debugHotkeys = true;
    public KeyCode breakKey = KeyCode.R;
    public KeyCode repairKey = KeyCode.T;

    [Header("Debug")]
    public bool debugLogs = false;

    // ===== IInteractable =====
    public string GetPrompt()
    {
        return _isBroken ? promptWhenBroken : promptWhenOk;
    }

    public void Interact(GameObject who)
    {
        if (!_isBroken)
        {
            if (debugLogs) Debug.Log($"[Repairable:{repairId}] Already repaired.");
            return;
        }
        if (!minigame)
        {
            if (debugLogs) Debug.LogWarning($"[Repairable:{repairId}] Minigame not assigned.");
            return;
        }

        if (audioSource && startClip) audioSource.PlayOneShot(startClip);
        if (sparksVfx) sparksVfx.Play();

        minigame.Open(this);
        if (debugLogs) Debug.Log($"[Repairable:{repairId}] Open minigame.");
    }

    // ===== API ��� ����-���� =====
    public float LoadProgress() => _progress;

    public void AddProgress(float delta01)
    {
        if (!_isBroken) return;
        float old = _progress;
        _progress = Mathf.Clamp01(_progress + Mathf.Max(0f, delta01));
        if (debugLogs && Mathf.Abs(_progress - old) > 0.0001f)
            Debug.Log($"[Repairable:{repairId}] progress {old:0.00} -> {_progress:0.00}");
        if (_progress >= 1f) CompleteRepair();
    }

    public void BreakDown()
    {
        _isBroken = true;
        _progress = 0f;
        if (sparksVfx) sparksVfx.Play();
        if (debugLogs) Debug.Log($"[Repairable:{repairId}] BREAK.");
    }

    public void CompleteRepair()
    {
        _isBroken = false;
        _progress = 1f;
        if (sparksVfx) sparksVfx.Stop();
        if (audioSource && successClip) audioSource.PlayOneShot(successClip);
        if (minigame && minigame.IsOpen) minigame.Close();
        if (debugLogs) Debug.Log($"[Repairable:{repairId}] REPAIRED.");
    }

    // ===== ������ ���� ������ =====
    void Update()
    {
        if (!debugHotkeys) return;
        if (!Application.isEditor) return;              // ����� �� ������ � �����

        if (Input.GetKeyDown(breakKey)) BreakDown();
        if (Input.GetKeyDown(repairKey)) CompleteRepair();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = false; // ������ �� �������
    }
#endif
}
