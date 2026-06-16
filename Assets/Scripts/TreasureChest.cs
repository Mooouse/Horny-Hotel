using UnityEngine;
using System.Collections.Generic;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] private List<AbilityData> abilities = new List<AbilityData>();

    private void Start()
    {
        InitializeAbilities();
    }

    private void InitializeAbilities()
    {
        abilities.Clear();

        // Способность 1 - Усиление здоровья
        abilities.Add(new AbilityData
        {
            abilityName = "Зелье жизни",
            description = "+20 HP",
            healthBonus = 20,
            damageBonus = 0,
            defenseBonus = 0,
            attackSpeedBonus = 0,
            stunChanceBonus = 0
        });

        // Способность 2 - Усиление урона
        abilities.Add(new AbilityData
        {
            abilityName = "Боевой кристалл",
            description = "+5 Урон",
            healthBonus = 0,
            damageBonus = 5,
            defenseBonus = 0,
            attackSpeedBonus = 0,
            stunChanceBonus = 0
        });

        // Способность 3 - Усиление защиты
        abilities.Add(new AbilityData
        {
            abilityName = "Щит силы",
            description = "+3 Защита",
            healthBonus = 0,
            damageBonus = 0,
            defenseBonus = 3,
            attackSpeedBonus = 0,
            stunChanceBonus = 0
        });

        // Способность 4 - Усиление скорости атаки
        abilities.Add(new AbilityData
        {
            abilityName = "Ускоритель",
            description = "+0.3 Скорость атаки",
            healthBonus = 0,
            damageBonus = 0,
            defenseBonus = 0,
            attackSpeedBonus = 0.3f,
            stunChanceBonus = 0
        });

        // Способность 5 - Усиление оглушения
        abilities.Add(new AbilityData
        {
            abilityName = "Камень оглушения",
            description = "+10% Шанс оглушения",
            healthBonus = 0,
            damageBonus = 0,
            defenseBonus = 0,
            attackSpeedBonus = 0,
            stunChanceBonus = 10f
        });

        // Способность 6 - Сбалансированная способность
        abilities.Add(new AbilityData
        {
            abilityName = "Артефакт баланса",
            description = "+10 HP, +2 Урон, +1 Защита",
            healthBonus = 10,
            damageBonus = 2,
            defenseBonus = 1,
            attackSpeedBonus = 0,
            stunChanceBonus = 0
        });
    }

    public List<AbilityData> GetRandomAbilities(int count = 3)
    {
        List<AbilityData> selectedAbilities = new List<AbilityData>();
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < abilities.Count; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < count && availableIndices.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            selectedAbilities.Add(abilities[availableIndices[randomIndex]]);
            availableIndices.RemoveAt(randomIndex);
        }

        return selectedAbilities;
    }
}
