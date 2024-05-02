﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpell : BaseAttack
{
    public PoisonSpell()
    {
        attackName = "Poison";
        //attackDescription = "Basic spell spell.";
        attackDamage = 10f;
        attackCost = 4f;
        effectChance = Mathf.Clamp(effectChance, 0.0f, 1.0f);
        effectDuration = 8f;
    }

}
