using UnityEngine;
using System.Collections.Generic;

public class DoorChoiceSystem : MonoBehaviour
{
    [System.Serializable]
    public class DoorOption
    {
        public string doorName;
        public string description;
        public DoorType doorType;
        public float difficulty; // 0-1, где 1 - самый сложный
    }

    public enum DoorType
    {
        EasyBoss,      // Лёгкий босс
        HardBoss,      // Сложный босс
        GoodLoot,      // Хороший лут (способность +)
        BadLoot,       // Плохой лут (способность -)
        TrapDoor       // Ловушка (урон)
    }

    private List<DoorOption> currentDoors = new List<DoorOption>();

    public System.Action<List<DoorOption>> OnDoorsPresented;
    public System.Action<DoorType> OnDoorSelected;

    public void PresentDoors()
    {
        currentDoors.Clear();

        // Генерируем 3 случайные двери
        List<DoorType> doorTypes = new List<DoorType>
        {
            DoorType.EasyBoss,
            DoorType.HardBoss,
            DoorType.GoodLoot
        };

        // Перемешиваем для разнообразия
        for (int i = 0; i < doorTypes.Count; i++)
        {
            int randomIndex = Random.Range(i, doorTypes.Count);
            (doorTypes[i], doorTypes[randomIndex]) = (doorTypes[randomIndex], doorTypes[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            currentDoors.Add(CreateDoorOption(doorTypes[i], i + 1));
        }

        OnDoorsPresented?.Invoke(currentDoors);
    }

    private DoorOption CreateDoorOption(DoorType type, int doorNumber)
    {
        DoorOption door = new DoorOption { doorType = type, doorName = $"Дверь {doorNumber}" };

        switch (type)
        {
            case DoorType.EasyBoss:
                door.description = "Лёгкий босс";
                door.difficulty = 0.7f;
                break;
            case DoorType.HardBoss:
                door.description = "Сложный босс";
                door.difficulty = 1.2f;
                break;
            case DoorType.GoodLoot:
                door.description = "Хороший лут (+20%)";
                door.difficulty = 0.5f;
                break;
            case DoorType.BadLoot:
                door.description = "Плохой лут (-10%)";
                door.difficulty = 0.3f;
                break;
            case DoorType.TrapDoor:
                door.description = "Ловушка!";
                door.difficulty = 0f;
                break;
        }

        return door;
    }

    public void SelectDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < currentDoors.Count)
        {
            OnDoorSelected?.Invoke(currentDoors[doorIndex].doorType);
        }
    }

    public List<DoorOption> GetCurrentDoors()
    {
        return currentDoors;
    }
}
