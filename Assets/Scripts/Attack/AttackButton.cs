using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack magicAttackToPerform;
    public BaseAttack physicalAttackToPeform;

    public void CastMagicAttack()
    {
        GameObject.Find("GameManager").GetComponent<BattleStateMachine>().SelectMagicAttack(magicAttackToPerform);
    }

    public void CastPhysicalAttack()
    {
        GameObject.Find("GameManager").GetComponent<BattleStateMachine>().SelectPhysicalAttack(physicalAttackToPeform);
    }

}
