using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : BaseAttack
{
    public IceSpell()
    {
        attackName = "Ice";
        //attackDescription = "Basic fire spell which burns nothing.";
        attackDamage = 15f;
        attackCost = 6f;
    }

}
