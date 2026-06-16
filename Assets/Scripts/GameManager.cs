using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum EncounterType
    {
        NPC,
        Chest,
        DoorChoice,
        Boss
    }

    [System.Serializable]
    public class Encounter
    {
        public EncounterType type;
        public string description;
    }

    [SerializeField] private List<Encounter> encounters = new List<Encounter>();
    private int currentEncounterIndex = 0;
    private bool inCombat = false;
    private BossData currentBoss;
    private DoorChoiceSystem doorChoiceSystem;

    public System.Action<EncounterType, string> OnEncounterStart;
    public System.Action<string> OnGameMessage;

    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        doorChoiceSystem = FindObjectOfType<DoorChoiceSystem>();
        InitializeEncounters();
        StartNextEncounter();
    }

    private void InitializeEncounters()
    {
        // Последовательность встреч для первого рейда
        encounters.Clear();

        encounters.Add(new Encounter { type = EncounterType.NPC, description = "Встреча с торговцем" });
        encounters.Add(new Encounter { type = EncounterType.Chest, description = "Сундук с сокровищами" });
        encounters.Add(new Encounter { type = EncounterType.DoorChoice, description = "Три двери на выбор" });
        encounters.Add(new Encounter { type = EncounterType.Boss, description = "Финальный босс!" });
    }

    public static GameManager Instance => instance;

    public void StartNextEncounter()
    {
        if (currentEncounterIndex >= encounters.Count)
        {
            OnGameMessage?.Invoke("Рейд завершён! Все враги разбиты!");
            CompleteRaid();
            return;
        }

        Encounter encounter = encounters[currentEncounterIndex];
        OnEncounterStart?.Invoke(encounter.type, encounter.description);

        switch (encounter.type)
        {
            case EncounterType.NPC:
                HandleNPCEncounter();
                break;
            case EncounterType.Chest:
                HandleChestEncounter();
                break;
            case EncounterType.DoorChoice:
                HandleDoorChoice();
                break;
            case EncounterType.Boss:
                HandleBossEncounter();
                break;
        }

        currentEncounterIndex++;
    }

    private void HandleNPCEncounter()
    {
        OnGameMessage?.Invoke("Торговец: Привет, путник! Вот мой товар...");
        Invoke("SkipCurrentEncounter", 2f);
    }

    private void HandleChestEncounter()
    {
        OnGameMessage?.Invoke("Вы открыли сундук! Выберите способность...");
    }

    private void HandleDoorChoice()
    {
        OnGameMessage?.Invoke("Перед вами три двери...");
    }

    private void HandleBossEncounter()
    {
        currentBoss = BossDatabase.Instance.GetRandomBoss();
        inCombat = true;
        OnGameMessage?.Invoke($"Появился босс: {currentBoss.bossName}!");
    }

    public void SelectDoor(int doorIndex)
    {
        if (doorChoiceSystem != null)
        {
            doorChoiceSystem.SelectDoor(doorIndex);
        }
    }

    public void SkipCurrentEncounter()
    {
        StartNextEncounter();
    }

    private void CompleteRaid()
    {
        BossDatabase.Instance.IncrementRaidLevel();
        currentEncounterIndex = 0;
        InitializeEncounters();
        OnGameMessage?.Invoke($"Начинается рейд #{BossDatabase.Instance.GetRaidLevel() + 1}");
        Invoke("StartNextEncounter", 2f);
    }

    public bool IsInCombat()
    {
        return inCombat;
    }

    public void EndCombat(bool playerWon)
    {
        inCombat = false;
        if (playerWon)
        {
            OnGameMessage?.Invoke("Босс разбит! Идём дальше...");
        }
        else
        {
            OnGameMessage?.Invoke("Вы погибли... Характеристики восстановлены.");
            PlayerStats.Instance.Die();
            BossDatabase.Instance.ResetRaidLevel();
        }
        Invoke("StartNextEncounter", 2f);
    }

    public BossData GetCurrentBoss()
    {
        return currentBoss;
    }
}
