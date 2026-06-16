using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BossData
{
    public string bossName;
    public Sprite bossSprite;
    public int health;
    public int damage;
    public int defense;
    public float attackSpeed;
}

public class BossDatabase : MonoBehaviour
{
    [SerializeField] private List<BossData> bosses = new List<BossData>();
    private int raidLevel = 0;

    private static BossDatabase instance;

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

        InitializeBosses();
    }

    private void InitializeBosses()
    {
        bosses.Clear();

        // Босс 1
        bosses.Add(new BossData
        {
            bossName = "Скелет",
            health = 80,
            damage = 12,
            defense = 2,
            attackSpeed = 0.8f
        });

        // Босс 2
        bosses.Add(new BossData
        {
            bossName = "Огр",
            health = 120,
            damage = 18,
            defense = 4,
            attackSpeed = 0.6f
        });

        // Босс 3
        bosses.Add(new BossData
        {
            bossName = "Демон",
            health = 150,
            damage = 22,
            defense = 6,
            attackSpeed = 1.0f
        });
    }

    public static BossDatabase Instance => instance;

    public BossData GetRandomBoss()
    {
        BossData baseBoss = bosses[Random.Range(0, bosses.Count)];
        BossData scaledBoss = new BossData
        {
            bossName = baseBoss.bossName,
            bossSprite = baseBoss.bossSprite,
            health = Mathf.RoundToInt(baseBoss.health * (1 + raidLevel * 0.1f)),
            damage = Mathf.RoundToInt(baseBoss.damage * (1 + raidLevel * 0.1f)),
            defense = Mathf.RoundToInt(baseBoss.defense * (1 + raidLevel * 0.05f)),
            attackSpeed = baseBoss.attackSpeed
        };

        return scaledBoss;
    }

    public void IncrementRaidLevel()
    {
        raidLevel++;
    }

    public int GetRaidLevel()
    {
        return raidLevel;
    }

    public void ResetRaidLevel()
    {
        raidLevel = 0;
    }
}
