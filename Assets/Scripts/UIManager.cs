using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text damageText;
    [SerializeField] private Text defenseText;
    [SerializeField] private Text attackSpeedText;
    [SerializeField] private Text stunChanceText;
    [SerializeField] private Text raidLevelText;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image bossHealthBar;

    [SerializeField] private Text combatLogText;
    [SerializeField] private GameObject abilityButtonPrefab;
    [SerializeField] private Transform abilityButtonContainer;

    [SerializeField] private GameObject doorButtonPrefab;
    [SerializeField] private Transform doorButtonContainer;

    private List<string> combatLog = new List<string>();
    private const int maxLogLines = 10;

    private void Start()
    {
        GameManager.Instance.OnGameMessage += OnGameMessage;
        GameManager.Instance.OnEncounterStart += OnEncounterStart;
    }

    public void UpdatePlayerStats()
    {
        PlayerStats.Stats stats = PlayerStats.Instance.currentStats;

        healthText.text = $"HP: {stats.health}/{stats.maxHealth}";
        damageText.text = $"Урон: {stats.damage}";
        defenseText.text = $"Защита: {stats.defense}";
        attackSpeedText.text = $"Скорость атаки: {stats.attackSpeed:F1}";
        stunChanceText.text = $"Оглушение: {stats.stunChance:F0}%";

        raidLevelText.text = $"Рейд: {BossDatabase.Instance.GetRaidLevel() + 1}";

        // Обновляем полосу здоровья
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)stats.health / stats.maxHealth;
        }
    }

    public void UpdateBossHealthBar(int currentHealth, int maxHealth)
    {
        if (bossHealthBar != null && maxHealth > 0)
        {
            bossHealthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void ShowAbilitySelection(List<AbilityData> abilities)
    {
        // Очищаем старые кнопки
        foreach (Transform child in abilityButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаём кнопки для каждой способности
        for (int i = 0; i < abilities.Count; i++)
        {
            GameObject buttonGO = Instantiate(abilityButtonPrefab, abilityButtonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Text buttonText = buttonGO.GetComponentInChildren<Text>();

            AbilityData ability = abilities[i];
            buttonText.text = $"{ability.abilityName}\n{ability.description}";

            button.onClick.AddListener(() => OnAbilitySelected(ability));
        }
    }

    public void ShowDoorSelection(List<DoorChoiceSystem.DoorOption> doors)
    {
        // Очищаем старые кнопки
        foreach (Transform child in doorButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаём кнопки для каждой двери
        for (int i = 0; i < doors.Count; i++)
        {
            GameObject buttonGO = Instantiate(doorButtonPrefab, doorButtonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Text buttonText = buttonGO.GetComponentInChildren<Text>();

            DoorChoiceSystem.DoorOption door = doors[i];
            buttonText.text = $"{door.doorName}\n{door.description}";

            int doorIndex = i;
            button.onClick.AddListener(() => GameManager.Instance.SelectDoor(doorIndex));
        }
    }

    private void OnAbilitySelected(AbilityData ability)
    {
        PlayerStats.Instance.ApplyAbility(ability);
        UpdatePlayerStats();
        AddCombatLog($"Выбрана способность: {ability.abilityName}");
        GameManager.Instance.SkipCurrentEncounter();
    }

    private void OnGameMessage(string message)
    {
        AddCombatLog(message);
    }

    private void OnEncounterStart(GameManager.EncounterType type, string description)
    {
        AddCombatLog($"--- {description} ---");
    }

    public void AddCombatLog(string message)
    {
        combatLog.Add(message);

        if (combatLog.Count > maxLogLines)
        {
            combatLog.RemoveAt(0);
        }

        UpdateCombatLog();
    }

    private void UpdateCombatLog()
    {
        combatLogText.text = string.Join("\n", combatLog);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameMessage -= OnGameMessage;
            GameManager.Instance.OnEncounterStart -= OnEncounterStart;
        }
    }
}
