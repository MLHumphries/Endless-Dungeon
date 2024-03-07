using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{   
    public enum Type
    {
        Grass,
        Fire,
        Water,
        Electric
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        VeryRare
    }

    public Type EnemyType;
    public Rarity rarity;

    
}
