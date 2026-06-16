using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    [System.Serializable]
    public class Combatant
    {
        public string name;
        public int health;
        public int maxHealth;
        public int damage;
        public int defense;
        public float attackSpeed;
        public float stunChance;
        public bool isStunned;
        public float stunDuration;
    }

    public Combatant playerCombatant;
    public Combatant bossCombatant;

    private float playerAttackTimer;
    private float bossAttackTimer;
    private bool combatActive;

    public System.Action<string> OnCombatLog; // для логирования боя
    public System.Action<bool> OnCombatEnd; // true - победа, false - поражение

    public void StartCombat(BossData boss)
    {
        PlayerStats playerStats = PlayerStats.Instance;

        playerCombatant = new Combatant
        {
            name = "Игрок",
            health = playerStats.currentStats.health,
            maxHealth = playerStats.currentStats.maxHealth,
            damage = playerStats.currentStats.damage,
            defense = playerStats.currentStats.defense,
            attackSpeed = playerStats.currentStats.attackSpeed,
            stunChance = playerStats.currentStats.stunChance,
            isStunned = false,
            stunDuration = 0
        };

        bossCombatant = new Combatant
        {
            name = boss.bossName,
            health = boss.health,
            maxHealth = boss.health,
            damage = boss.damage,
            defense = boss.defense,
            attackSpeed = boss.attackSpeed,
            stunChance = 0,
            isStunned = false,
            stunDuration = 0
        };

        playerAttackTimer = 0;
        bossAttackTimer = 0;
        combatActive = true;

        OnCombatLog?.Invoke($"Бой начинается! Противник: {boss.bossName} (HP: {boss.health})");
    }

    public void UpdateCombat(float deltaTime)
    {
        if (!combatActive) return;

        // Обновляем оглушение
        UpdateStun(playerCombatant, deltaTime);
        UpdateStun(bossCombatant, deltaTime);

        // Атаки
        if (!playerCombatant.isStunned)
        {
            playerAttackTimer += deltaTime;
            float playerAttackInterval = 1f / playerCombatant.attackSpeed;
            if (playerAttackTimer >= playerAttackInterval)
            {
                AttackTarget(playerCombatant, bossCombatant, "Игрок");
                playerAttackTimer = 0;
            }
        }

        if (!bossCombatant.isStunned)
        {
            bossAttackTimer += deltaTime;
            float bossAttackInterval = 1f / bossCombatant.attackSpeed;
            if (bossAttackTimer >= bossAttackInterval)
            {
                AttackTarget(bossCombatant, playerCombatant, bossCombatant.name);
                bossAttackTimer = 0;
            }
        }

        // Проверка конца боя
        if (playerCombatant.health <= 0)
        {
            EndCombat(false);
        }
        else if (bossCombatant.health <= 0)
        {
            EndCombat(true);
        }
    }

    private void AttackTarget(Combatant attacker, Combatant target, string attackerName)
    {
        int damage = Mathf.Max(1, attacker.damage - target.defense);
        target.health -= damage;

        // Проверка оглушения
        bool stunned = false;
        if (Random.value * 100 < attacker.stunChance)
        {
            target.isStunned = true;
            target.stunDuration = 2f; // 2 секунды оглушения
            stunned = true;
        }

        string logMessage = $"{attackerName} атакует! Урон: {damage}";
        if (stunned)
        {
            logMessage += " [ОГЛУШЕН]";
        }
        OnCombatLog?.Invoke(logMessage);
    }

    private void UpdateStun(Combatant combatant, float deltaTime)
    {
        if (combatant.isStunned)
        {
            combatant.stunDuration -= deltaTime;
            if (combatant.stunDuration <= 0)
            {
                combatant.isStunned = false;
            }
        }
    }

    private void EndCombat(bool playerWon)
    {
        combatActive = false;
        if (playerWon)
        {
            OnCombatLog?.Invoke($"Победа! Вы разбили {bossCombatant.name}!");
            OnCombatEnd?.Invoke(true);
        }
        else
        {
            OnCombatLog?.Invoke("Поражение! Вы были разбиты...");
            OnCombatEnd?.Invoke(false);
        }
    }

    public bool IsCombatActive()
    {
        return combatActive;
    }

    public int GetPlayerHealth()
    {
        return playerCombatant?.health ?? 0;
    }

    public int GetBossHealth()
    {
        return bossCombatant?.health ?? 0;
    }
}
