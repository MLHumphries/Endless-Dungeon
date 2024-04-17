﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{

    public int stamina;
    public int intellect;
    public int dexterity;
    public int luck;

    //public List<BaseAttack> physicalAttacks = new List<BaseAttack>();
    public List<BaseAttack> magicAttacks = new List<BaseAttack>();
    public BaseAttack defense;


}
