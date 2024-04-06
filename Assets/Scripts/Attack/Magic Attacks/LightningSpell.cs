using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSpell : BaseAttack
{
    public LightningSpell()
    {
        attackName = "Lightning";
        //attackDescription = "Basic spell spell.";
        attackDamage = 16f;
        attackCost = 6f;
    }

}