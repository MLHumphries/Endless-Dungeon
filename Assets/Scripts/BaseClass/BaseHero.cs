using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
    public List<BaseAttack> magicAttacks = new List<BaseAttack>();
    public BaseAttack defend;
}
