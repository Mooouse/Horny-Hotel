using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public int health = 100;
        public int maxHealth = 100;
        public int damage = 15;
        public int defense = 5;
        public float attackSpeed = 1.0f; // атак в секунду
        public float stunChance = 0f; // шанс оглушения в процентах
    }

    public Stats currentStats;
    public Stats baseStats;

    private static PlayerStats instance;

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

        InitializeStats();
    }

    private void InitializeStats()
    {
        baseStats = new Stats
        {
            health = 100,
            maxHealth = 100,
            damage = 15,
            defense = 5,
            attackSpeed = 1.0f,
            stunChance = 0f
        };

        currentStats = new Stats
        {
            health = baseStats.health,
            maxHealth = baseStats.maxHealth,
            damage = baseStats.damage,
            defense = baseStats.defense,
            attackSpeed = baseStats.attackSpeed,
            stunChance = baseStats.stunChance
        };
    }

    public static PlayerStats Instance => instance;

    public void ResetStats()
    {
        currentStats.health = baseStats.health;
        currentStats.maxHealth = baseStats.maxHealth;
        currentStats.damage = baseStats.damage;
        currentStats.defense = baseStats.defense;
        currentStats.attackSpeed = baseStats.attackSpeed;
        currentStats.stunChance = baseStats.stunChance;
    }

    public void ApplyAbility(AbilityData ability)
    {
        currentStats.health += ability.healthBonus;
        if (currentStats.health > currentStats.maxHealth)
            currentStats.health = currentStats.maxHealth;

        currentStats.damage += ability.damageBonus;
        currentStats.defense += ability.defenseBonus;
        currentStats.attackSpeed += ability.attackSpeedBonus;
        currentStats.stunChance += ability.stunChanceBonus;

        // Сохраняем улучшения в базовые статы для следующего рейда
        baseStats.health += ability.healthBonus;
        baseStats.maxHealth += ability.healthBonus;
        baseStats.damage += ability.damageBonus;
        baseStats.defense += ability.defenseBonus;
        baseStats.attackSpeed += ability.attackSpeedBonus;
        baseStats.stunChance += ability.stunChanceBonus;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - currentStats.defense);
        currentStats.health -= actualDamage;
    }

    public bool IsAlive()
    {
        return currentStats.health > 0;
    }

    public void Die()
    {
        ResetStats();
    }
}
