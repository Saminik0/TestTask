// Assets/Scripts/RepairSystem/RepairMinigameController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RepairMinigameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panel;      // панель отключена по умолчанию (inactive)
    [SerializeField] Slider progressBar;
    [SerializeField] Text titleText;        // обычный Text (НЕ TMP)
    [SerializeField] Text hintText;         // обычный Text (НЕ TMP)

    [Header("Gameplay")]
    public KeyCode holdKey = KeyCode.Space;
    public float fillPerSecond = 0.5f;      // сколько прогресса в секунду
    public bool allowExitKeys = true;
    public KeyCode[] exitKeys = new[] { KeyCode.Escape, KeyCode.E };

    [Header("Optional")]
    public Behaviour disableWhileOpen;      // например, скрипт движения игрока/камеры

    [Header("Debug")]
    public bool debugLogs = true;

    public bool IsOpen { get; private set; }
    private Repairable current;
    private bool prevSendNav;

    void Awake()
    {
        // Подстрахуем null-ссылки сразу, чтобы понять, чего не хватает
        if (!panel) Debug.LogError("[Minigame] panel НЕ назначен в инспекторе", this);
        if (!progressBar) Debug.LogError("[Minigame] progressBar НЕ назначен в инспекторе", this);
        // titleText/hintText не критичны — можно оставить пустыми

        // Если кто-то очистил массив выходных клавиш
        if (exitKeys == null || exitKeys.Length == 0)
            exitKeys = new[] { KeyCode.Escape, KeyCode.E };
    }

    public void Open(Repairable repair)
    {
        current = repair;
        if (!current)
        {
            if (debugLogs) Debug.LogWarning("[Minigame] Open() вызван с null Repairable.");
            return;
        }

        IsOpen = true;

        // Чтобы Space не «кликал» UI-кнопки: снимаем выделение и навигацию
        if (EventSystem.current)
        {
            prevSendNav = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (disableWhileOpen) disableWhileOpen.enabled = false;

        if (panel) panel.SetActive(true);
        if (progressBar) progressBar.value = Mathf.Clamp01(current.LoadProgress());
        if (titleText) titleText.text = "Ремонт оборудования";
        if (hintText) hintText.text = $"Удерживайте [{holdKey}] — ремонт  •  Esc/E — выйти";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (debugLogs) Debug.Log("[Minigame] OPEN");

        // подцепим время ремонта из баланса, если он есть
        try
        {
            if (BalanceLoader.Data != null)
            {
                var t = BalanceLoader.Data.boilerRepairTime;
                if (t > 0.05f) fillPerSecond = 1f / t; // 2.0 cек → 0.5/сек
            }
        }
        catch { /* BalanceLoader может отсутствовать — это ок */ }
    }

    public void Close()
    {
        if (panel) panel.SetActive(false);

        if (EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.sendNavigationEvents = prevSendNav;
        }

        if (disableWhileOpen) disableWhileOpen.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        IsOpen = false;
        current = null;

        if (debugLogs) Debug.Log("[Minigame] CLOSE");
    }

    void Update()
    {
        // Локальная копия — если объект внезапно уничтожат, не словим NRE между проверками
        var c = current;
        if (!IsOpen || c == null) return;

        // выход
        if (allowExitKeys && exitKeys != null)
        {
            for (int i = 0; i < exitKeys.Length; i++)
            {
                if (Input.GetKeyDown(exitKeys[i])) { Close(); return; }
            }
        }

        // Без слайдера нет смысла продолжать — не падаем, просто выходим
        if (!progressBar) return;

        // ремонт
        if (Input.GetKey(holdKey))
        {
            float delta = fillPerSecond * Time.unscaledDeltaTime;

            // Подстрахуемся, что цель ещё жива
            if (c != null)
            {
                c.AddProgress(delta);
                progressBar.value = c.LoadProgress();
            }
            else
            {
                if (debugLogs) Debug.LogWarning("[Minigame] Target потерян во время ремонта. Закрываю UI.");
                Close();
            }
        }
    }
}
