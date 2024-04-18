using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;
    public GameObject selector;
    public Text enemyAttackText;
    public Text enemyHealthUI;

    public enum TurnState
    {
        Processing,
        ChooseAction,
        Waiting,
        Action,
        Dead
    }

    public TurnState currentState;

    private float cur_Cooldown = 0f;
    public float max_Cooldown = 10f;

    private Vector3 startPosition;

    private bool actionStarted = false;
    public GameObject HeroToAttack;

    private float animSpeed = 10f;
    

    void Start ()
    {
        //enemyHealthUI.text = "HP: " + enemy.curHP.ToString();
        currentState = TurnState.Processing;
        selector.SetActive(false);
        BSM = GameObject.Find("GameManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }
	
	
	void Update ()
    {
        //Debug.Log(currentState);
        switch (currentState)
        {

            case (TurnState.Processing):
                UpdateProgressBar();
                break;

            case (TurnState.ChooseAction):
                ChooseAction();
                currentState = TurnState.Waiting;
                break;

            case (TurnState.Waiting):
                break;


            case (TurnState.Action):
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):
                //change tag
                this.gameObject.tag = "DeadEnemy";
                //not attackable
                BSM.EnemyInGame.Remove(this.gameObject);

                //remove item from perform list
                for (int i = 0; i < BSM.PerformList.Count; i++)
                {
                    if (BSM.PerformList[i].attackGameObject == this.gameObject)
                    {
                        BSM.PerformList.Remove(BSM.PerformList[i]);
                    }
                }

                this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                
                //StartCoroutine(WinGame());
                break;

        }
    }
    private IEnumerator WinGame()
    {
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void UpdateProgressBar()
    {
        cur_Cooldown = cur_Cooldown + Time.deltaTime;
        
        if (cur_Cooldown >= max_Cooldown)
        {
            currentState = TurnState.ChooseAction;
        }
    }
    void ChooseAction()
    {
        HandleTurns myAttack = new HandleTurns();
        myAttack.attackerName = enemy.name;
        myAttack.type = "Enemy";
        myAttack.attackGameObject = this.gameObject;
        myAttack.attackerTarget = BSM.HeroInGame[Random.Range(0, BSM.HeroInGame.Count)];

        int num = Random.Range(0, enemy.attacks.Count);
        
        myAttack.chosenAttack = enemy.attacks[num];
      
        BSM.CollectActions(myAttack);

    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        //animate enemy to hero attack
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x + 1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while (MoveTowardsEnemy(heroPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        //doDamage
        DoDamage();
        //animate back to start pos
        Vector3 firstPosition = startPosition;
        while (MoveTowardsEnemy(startPosition))
        {
            yield return null;
        }

        enemyAttackText.text = "";

        BSM.PerformList.RemoveAt(0);

        BSM.battleState = BattleStateMachine.PerformAction.Wait;
        //end Coroutine
        actionStarted = false;
        //reset EnemyState
        cur_Cooldown = 0f;
        currentState = TurnState.Processing;
    }
    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowards(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        float calcDamage;
        if(HeroToAttack.GetComponent<HeroStateMachine>().isDefending)
        {
            calcDamage = 0;
        }
        else
        {
            calcDamage = enemy.curATK + BSM.PerformList[0].chosenAttack.attackDamage;
        }
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calcDamage);
        enemyAttackText.text = enemy.name + " has chosen " + BSM.PerformList[0].chosenAttack.attackName.ToString() + " and does " + calcDamage + " damage";
        TextDelay();
    }
    public void TakeDamage(float getDamageAmount)
    {
        enemy.curHP -= getDamageAmount;

        if(enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currentState = TurnState.Dead;
        }
        //UpdateEnemyHealth();
    }

    private IEnumerator TextDelay()
    {
        yield return new WaitForSeconds(0.75f);
    }

    //void UpdateEnemyHealth()
    //{
    //    enemyHealthUI.text = "HP: " + enemy.curHP;
    //}
}
