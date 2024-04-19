using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string name;

    public float maxHP;
    public float curHP;

    public float maxMP;
    public float curMP;

    public int strength;
    public int defense;
    public int speed;
    public int intellect;
    //public int dexterity;
    public int luck;

    public List<BaseAttack> attacks = new List<BaseAttack>();
    

}
