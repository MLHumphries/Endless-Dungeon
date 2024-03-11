using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurns
{
    public string attackerName;
    public string type;
    public GameObject attackGameObject;
    public GameObject attackerTarget;

    //which attack is performed
    public BaseAttack chosenAttack;
	
}
