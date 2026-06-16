using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private TreasureChest treasureChest;
    [SerializeField] private DoorChoiceSystem doorChoiceSystem;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private Transform npcSpawnPoint;
    [SerializeField] private Transform chestSpawnPoint;
    [SerializeField] private Transform doorChoiceSpawnPoint;
    [SerializeField] private Transform bossSpawnPoint;

    private GameManager.EncounterType currentEncounterType;

    private void Start()
    {
        GameManager.Instance.OnEncounterStart += HandleEncounterStart;
        combatSystem.OnCombatEnd += HandleCombatEnd;
        combatSystem.OnCombatLog += uiManager.AddCombatLog;
        doorChoiceSystem.OnDoorsPresented += uiManager.ShowDoorSelection;
        doorChoiceSystem.OnDoorSelected += HandleDoorSelected;

        uiManager.UpdatePlayerStats();
    }

    private void Update()
    {
        // Обновляем бой если он активен
        if (combatSystem.IsCombatActive())
        {
            combatSystem.UpdateCombat(Time.deltaTime);
            uiManager.UpdateBossHealthBar(combatSystem.GetBossHealth(), GameManager.Instance.GetCurrentBoss().health);
            uiManager.UpdatePlayerStats();
        }
    }

    private void HandleEncounterStart(GameManager.EncounterType type, string description)
    {
        currentEncounterType = type;

        switch (type)
        {
            case GameManager.EncounterType.NPC:
                HandleNPCEncounter();
                break;
            case GameManager.EncounterType.Chest:
                HandleChestEncounter();
                break;
            case GameManager.EncounterType.DoorChoice:
                HandleDoorChoiceEncounter();
                break;
            case GameManager.EncounterType.Boss:
                HandleBossEncounter();
                break;
        }
    }

    private void HandleNPCEncounter()
    {
        // Персонаж идет к NPC
        playerController.MoveToPosition(npcSpawnPoint.position);
        Invoke("SkipNPCEncounter", 3f);
    }

    private void SkipNPCEncounter()
    {
        playerController.StopMoving();
        GameManager.Instance.SkipCurrentEncounter();
    }

    private void HandleChestEncounter()
    {
        // Персонаж идет к сундуку
        playerController.MoveToPosition(chestSpawnPoint.position);

        Invoke(() =>
        {
            playerController.StopMoving();
            List<AbilityData> abilities = treasureChest.GetRandomAbilities(3);
            uiManager.ShowAbilitySelection(abilities);
        }, 2f);
    }

    private void HandleDoorChoiceEncounter()
    {
        // Персонаж идет к дверям
        playerController.MoveToPosition(doorChoiceSpawnPoint.position);

        Invoke(() =>
        {
            playerController.StopMoving();
            doorChoiceSystem.PresentDoors();
        }, 2f);
    }

    private void HandleBossEncounter()
    {
        // Персонаж идет к боссу
        playerController.MoveToPosition(bossSpawnPoint.position);

        Invoke(() =>
        {
            playerController.StopMoving();
            BossData boss = GameManager.Instance.GetCurrentBoss();
            combatSystem.StartCombat(boss);
        }, 2f);
    }

    private void HandleDoorSelected(DoorChoiceSystem.DoorType doorType)
    {
        switch (doorType)
        {
            case DoorChoiceSystem.DoorType.EasyBoss:
            case DoorChoiceSystem.DoorType.HardBoss:
                GameManager.Instance.SkipCurrentEncounter();
                break;
            case DoorChoiceSystem.DoorType.GoodLoot:
                uiManager.AddCombatLog("Отличный лут!");
                List<AbilityData> goodAbilities = treasureChest.GetRandomAbilities(3);
                uiManager.ShowAbilitySelection(goodAbilities);
                break;
            case DoorChoiceSystem.DoorType.BadLoot:
                uiManager.AddCombatLog("Плохой лут...");
                GameManager.Instance.SkipCurrentEncounter();
                break;
            case DoorChoiceSystem.DoorType.TrapDoor:
                uiManager.AddCombatLog("ЛОВУШКА! Вы получили урон!");
                PlayerStats.Instance.currentStats.health -= 20;
                if (PlayerStats.Instance.currentStats.health <= 0)
                {
                    GameManager.Instance.EndCombat(false);
                }
                else
                {
                    GameManager.Instance.SkipCurrentEncounter();
                }
                break;
        }
    }

    private void HandleCombatEnd(bool playerWon)
    {
        if (playerWon)
        {
            uiManager.AddCombatLog("Босс разбит!");
        }
        else
        {
            uiManager.AddCombatLog("Вы были разбиты...");
        }

        Invoke(() =>
        {
            GameManager.Instance.EndCombat(playerWon);
        }, 2f);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEncounterStart -= HandleEncounterStart;
        }

        combatSystem.OnCombatEnd -= HandleCombatEnd;
        combatSystem.OnCombatLog -= uiManager.AddCombatLog;
        doorChoiceSystem.OnDoorsPresented -= uiManager.ShowDoorSelection;
        doorChoiceSystem.OnDoorSelected -= HandleDoorSelected;
    }
}
