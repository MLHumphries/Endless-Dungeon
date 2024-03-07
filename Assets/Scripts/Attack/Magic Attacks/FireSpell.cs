using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : BaseAttack
{
    public FireSpell()
    {
        attackName = "Fire";
        //attackDescription = "Basic fire spell which burns nothing.";
        attackDamage = 10f;
        attackCost = 4f;
    }

}
