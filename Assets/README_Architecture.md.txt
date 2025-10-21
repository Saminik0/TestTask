# Архитектура проекта (Задача №3)

## Структура
- `Scripts/Core` — базовые утилиты (Singleton, EventBus, Bootstrap).
- `Scripts/Systems` — менеджеры (Audio, Save, RandomEvents).
- `Scripts/Gameplay` — игровая логика (Repair, Interaction).
- `Scripts/UI` — интерфейс.
- `Resources/Boot` — автозагружаемые префабы.
- `Prefabs/Managers/Managers.prefab` — все менеджеры в одном месте.

## Инициализация
- В сцене есть объект `Bootstrap` — он подгружает `Resources/Boot/Managers.prefab`.
- Менеджеры помечены `DontDestroyOnLoad` и доступны как `.Instance`.

## Коммуникация
- Межсистемные события — через `EventBus` (publish/subscribe).
- Аудио — через `AudioManager.Instance`.
- Сохранения — через `SaveSystem.Instance`.
