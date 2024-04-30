using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseHero hero;

    public GameObject selector;
    public GameObject enemyToAttack;

    private bool actionStarted = false;
    public bool isDefending = false;

    private Vector3 startPosition;
    private float animSpeed = 6.5f;

    public Text playerUIText;

    public enum TurnState
    {
        Processing,
        AddToList,
        Waiting,
        Selecting,
        Action,
        Dead
    }

    public TurnState currentState;
    [SerializeField]
    private float curCooldown = 0f;
    [SerializeField]
    private float maxCooldown = 15f;
    private Image ProgressBar;

    private bool alive = true;
    //heroPanel
    
    public GameObject HeroPanelName;
    public GameObject HeroPanelStats;
    private HeroPanelName heroName;
    private HeroPanelStats heroStats;


    void Start ()
    {
        hero.curHP = hero.maxHP;
        hero.curMP = hero.maxMP;
        //create panel and fill in info
        CreateHeroPanel();

        startPosition = transform.position;
        curCooldown = Random.Range(0f, 0.5f);
        selector.SetActive(false);
        BSM = GameObject.Find("GameManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.Processing;
        ;
    }
	
	
	void Update ()
    {
        //Debug.Log(currentState)
        switch (currentState)
        {
            
            case (TurnState.Processing):
                UpdateProgressBar();
                break;

            case (TurnState.AddToList):
                BSM.HeroesToManage.Add(this.gameObject);
                currentState = TurnState.Waiting;
                break;

            case (TurnState.Waiting):
                
                break;

            case (TurnState.Action):
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):
                if(!alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";
                    //not attackable
                    BSM.HeroInGame.Remove(this.gameObject);
                    //cant manage hero
                    BSM.HeroesToManage.Remove(this.gameObject);
                    //deactivate selector
                    selector.SetActive(false);
                    //reset gui
                    BSM.attackPanel.SetActive(false);
                    BSM.enemySelectPanel.SetActive(false);
                    if(BSM.HeroInGame.Count > 0)
                    {
                        //remove item from perform list
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if(i != 0)
                            {
                                if (BSM.PerformList[i].attackGameObject == this.gameObject)
                                {
                                    BSM.PerformList.Remove(BSM.PerformList[i]);
                                }
                                else if (BSM.PerformList[i].attackTarget == this.gameObject)
                                {
                                    BSM.PerformList[i].attackTarget = BSM.HeroInGame[Random.Range(0, BSM.HeroInGame.Count)];
                                }
                            }
                            
                        }
                    }
                    
                    //Changes color and rotates character
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    this.gameObject.transform.Rotate(90, 0, 180);
                    //Checks if player is alive
                    BSM.battleState = BattleStateMachine.PerformAction.CheckAlive;

                    alive = false;
                }
                break;
                
        }
    }

    void UpdateProgressBar()
    {
        curCooldown = (curCooldown + (hero.speed / 100f)) + Time.deltaTime;
        float calcCooldown = curCooldown / maxCooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);

        if (curCooldown >= maxCooldown)
        {
            currentState = TurnState.AddToList;
        }
    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        //animate enemy to hero attack (subtract 1.5f so the sprites don't overlap one another)
        Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x - 1.5f, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition))
        {
            yield return null;
        }

        //yield return new WaitForSeconds(0.5f);
        float damage = 0;
        //do damage
        if (!isDefending)
        {
            damage = DoDamage();
            BSM.attackUIText.text = heroName.heroName.text + " has chosen " + BSM.PerformList[0].chosenAttack.attackName.ToString() + " and does " + damage + " damage.";
        }
        else
        {
            BSM.attackUIText.text = heroName.heroName.text + " is defending.";
        }
        //yield return new WaitForSeconds(0.5f);


        Vector3 firstPosition = startPosition;
        while (MoveTowardsEnemy(startPosition))
        {
            yield return null;
        }

        BSM.PerformList.RemoveAt(0);
        if(BSM.battleState != BattleStateMachine.PerformAction.Win && BSM.battleState != BattleStateMachine.PerformAction.Lose)
        {
            BSM.battleState = BattleStateMachine.PerformAction.Wait;
            curCooldown = 0f;
            currentState = TurnState.Processing;
        }
        else
        {
            currentState = TurnState.Waiting;
        }
        
        //end Coroutine
        actionStarted = false;
        //reset EnemyState
        
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    
    public void TakeDamage(float getDamageAmount)
    {
        //TODO Defense check here instead of ESM
        hero.curHP -= getDamageAmount;
        if (hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.Dead;
        }

        isDefending = false;
        UpdateHeroPanel();
    }
    float DoDamage()
    {
        //HandleTurns heroAttack = new HandleTurns();
        float calc_damage;

        //Do magic attack
        if (BSM.PerformList[0].chosenAttack.attackCost > 0)
        {
            if(BSM.PerformList[0].chosenAttack.effectChance > 0)
            {
                //do chance calc
                Chance(BSM.PerformList[0].chosenAttack.effectChance);
                //if hit, do damage over time
                calc_damage = 1f;
            }
            else
            {
                if (hero.curMP >= BSM.PerformList[0].chosenAttack.attackCost)
                {
                    hero.curMP = hero.curMP - BSM.PerformList[0].chosenAttack.attackCost;
                    calc_damage = hero.intellect + BSM.PerformList[0].chosenAttack.attackDamage;
                    //print(BSM.PerformList[0].chosenAttack.attackName);
                    //print("Intellect: " + hero.intellect + " " + BSM.PerformList[0].chosenAttack.attackDamage + " = " + calc_damage);
                }
                else
                {
                    calc_damage = 0;
                }
            }
         
        }
        //Do physical attack
        else
        {
            calc_damage = hero.strength + BSM.PerformList[0].chosenAttack.attackDamage;
            //print(BSM.PerformList[0].chosenAttack.attackName);
            //print("Strength: " + hero.strength + " " + BSM.PerformList[0].chosenAttack.attackDamage + " = " + calc_damage);
        }

        if(enemyToAttack != this.gameObject)
        {
            enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
        }
        UpdateHeroPanel();
        return(calc_damage);
    }

    private bool Chance(float critChance)
    {
        bool isEffected = false;
        float val = Random.value;
        print(val);
        if(val < critChance)
        {
            print("Is poisoned");
            isEffected = true;
        }
        else
        {
            isEffected = false;
        }
        return isEffected;

    }


    void CreateHeroPanel()
    {
        heroName = HeroPanelName.GetComponent<HeroPanelName>();
        heroStats = HeroPanelStats.GetComponent<HeroPanelStats>();
        
        heroName.heroName.text = hero.name;
        heroStats.heroHP.text = hero.curHP + "/" + hero.maxHP;
        heroStats.heroMP.text = hero.curMP + "/" + hero.maxMP;
        ProgressBar = heroStats.progressBar;
    }

    void UpdateHeroPanel()
    {
        heroStats.heroHP.text = hero.curHP + "/" + hero.maxHP;
        heroStats.heroMP.text = hero.curMP + "/" + hero.maxMP;
    }
}

