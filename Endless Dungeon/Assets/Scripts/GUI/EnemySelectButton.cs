using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;
    

    public void SelectEnemy()
    {
        //input of enemy prefab
        GameObject.Find("GameManager").GetComponent<BattleStateMachine>().Input2(enemyPrefab);
    }

    public void HideSelector()
    {
         enemyPrefab.transform.Find("Selector").gameObject.SetActive(false);
    }
    public void ShowSelector()
    {
        enemyPrefab.transform.Find("Selector").gameObject.SetActive(true);
    }
	
}
