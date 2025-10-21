// Assets/Scripts/Interaction/PlayerInteractor.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("Оставь пустым, если используешь тег MainCamera")]
    public Camera overrideCamera;             // <- ЯВНО можно назначить камеру в инспекторе

    [Header("Raycast")]
    public float distance = 100f;             // дальность щедрая
    public float sphereRadius = 0.05f;        // SphereCast, чтобы «толще» луч
    public LayerMask interactMask = ~0;       // ВСЕ слои
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Collide; // ловим даже триггеры

    [Header("UI")]
    public Text promptText;                   // обычный Text ИЛИ...
    public TMP_Text promptTmpText;            // ...TMP_Text (любой один)
    [TextArea] public string defaultPrompt = "";

    [Header("Minigame")]
    public RepairMinigameController minigame;

    [Header("Debug")]
    public bool debugOverlay = true;
    public bool debugLogs = true;
    public Color rayColor = Color.cyan;

    Camera _cam;
    IInteractable _current;
    string _overlay = "no hit";

    void Awake()
    {
        _cam = overrideCamera != null ? overrideCamera : Camera.main;
        if (debugLogs)
        {
            if (_cam) Debug.Log($"[Interactor] Camera = '{_cam.name}' (tag={_cam.tag})");
            else Debug.LogWarning("[Interactor] Camera is NULL. Назначь overrideCamera или поставь тег MainCamera.");
        }
        SetPrompt(defaultPrompt);
    }

    void Update()
    {
        // если мини-игра открыта — не сканируем мир
        if (minigame && minigame.IsOpen)
        {
            SetPrompt("");
            _overlay = "minigame open";
            return;
        }

        if (_cam == null) _cam = overrideCamera != null ? overrideCamera : Camera.main;
        if (_cam == null)
        {
            _overlay = "no camera";
            return;
        }

        var origin = _cam.transform.position;
        var dir = _cam.transform.forward;

#if UNITY_EDITOR
        Debug.DrawRay(origin, dir * distance, rayColor, 0f, false);
#endif

        // 1) Пытаемся SphereCast (толстый луч)
        bool hitSomething = Physics.SphereCast(origin, sphereRadius, dir, out RaycastHit hit, distance, interactMask, triggerMode);

        // 2) Если не попали, пробуем обычный Raycast (на всякий)
        if (!hitSomething)
            hitSomething = Physics.Raycast(origin, dir, out hit, distance, interactMask, triggerMode);

        if (hitSomething)
        {
            var go = hit.collider.gameObject;
            string layerName = LayerMask.LayerToName(go.layer);
            var interactable = hit.collider.GetComponentInParent<IInteractable>();

            _overlay = $"HIT: {go.name} | Layer: {layerName}({go.layer}) | has IInteractable: {(interactable != null)}";
            _current = interactable;

            if (_current != null)
            {
                var prompt = _current.GetPrompt() ?? "";
                SetPrompt(prompt);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (debugLogs) Debug.Log($"[Interactor] Interact E on {go.name} ({_current})");
                    _current.Interact(gameObject);
                }
            }
            else
            {
                SetPrompt(defaultPrompt);
            }
        }
        else
        {
            _current = null;
            SetPrompt(defaultPrompt);
            _overlay = $"no hit | cam='{_cam.name}' pos={_cam.transform.position:F2} fwd={_cam.transform.forward:F2}";
        }
    }

    void SetPrompt(string msg)
    {
        if (promptText) promptText.text = msg;
        if (promptTmpText) promptTmpText.text = msg;
    }

    void OnGUI()
    {
        if (!debugOverlay) return;

        GUI.Label(new Rect(10, 10, 1200, 22), $"Raycast: {_overlay}");
        if (_cam)
        {
            GUI.Label(new Rect(10, 32, 1200, 22), $"Camera: '{_cam.name}' pos={_cam.transform.position:F2} fwd={_cam.transform.forward:F2}");
        }
    }
}
