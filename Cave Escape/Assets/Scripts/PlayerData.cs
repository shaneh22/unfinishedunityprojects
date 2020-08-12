using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int food;
    public bool isEquipped;
    public int wallDamage;

    public PlayerData(GameManager manager)
    {
        level = manager.level-1;
        food = manager.playerFoodPoints;
        isEquipped = manager.swordEquipped;
        wallDamage = manager.playerWallDamage;
    }
}
