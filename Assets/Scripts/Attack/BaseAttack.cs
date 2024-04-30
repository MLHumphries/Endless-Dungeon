using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName; // name of attack ex. Slash
    public float attackDamage; // base damage
    public float attackCost; //applies to magic attacks using magic
    public float effectChance; //chance certain magic can last for more than one turn (poison, burn, etc.)
    public float effectDuration; //duration that effect stays on character
}
