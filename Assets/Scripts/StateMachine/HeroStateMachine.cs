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

    private Vector3 startPosition;
    private float animSpeed = 10f;

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

    private float cur_Cooldown = 0f;
    private float max_Cooldown = 4f;
    private Image ProgressBar;

    private bool alive = true;
    //heroPanel
    
    public GameObject HeroPanelName;
    public GameObject HeroPanelStats;
    private HeroPanelName heroName;
    private HeroPanelStats heroStats;


    void Start ()
    {
        //create panel and fill in info
        CreateHeroPanel();

        startPosition = transform.position;
        cur_Cooldown = Random.Range(0f, 2.5f);
        selector.SetActive(false);
        BSM = GameObject.Find("GameManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.Processing;
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
                    //remove item from perform list
                    for(int i = 0; i < BSM.PerformList.Count; i ++)
                    {
                        if (BSM.PerformList[i].attackGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    //change color
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset hero input
                    BSM.heroInput = BattleStateMachine.HeroGUI.Activate;

                    alive = false;

                    SceneManager.LoadScene("Game Over");
                }
                break;
                
        }
    }

    void UpdateProgressBar()
    {
        cur_Cooldown = cur_Cooldown + Time.deltaTime;
        float calc_Cooldown = cur_Cooldown / max_Cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_Cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);

        if (cur_Cooldown >= max_Cooldown)
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

        //animate enemy to hero attack
        Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x - 1.5f, enemyToAttack.transform.position.y);
        while (MoveTowardsEnemy(enemyPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //do damage
        DoDamage();

        Vector3 firstPosition = startPosition;
        while (MoveTowardsEnemy(startPosition))
        {
            yield return null;
        }

        playerUIText.text = "";

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

    public void TakeDamage(float getDamageAmount)
    {
        hero.curHP -= getDamageAmount;
        if(hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.Dead;
        }
        UpdateHeroPanel();
    }
    void DoDamage()
    {
        HandleTurns heroAttack = new HandleTurns();
        float calc_damage = hero.curATK + BSM.PerformList[0].chosenAttack.attackDamage;
        enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
        playerUIText.text = heroName.heroName.text + " has chosen " + BSM.PerformList[0].chosenAttack.attackName.ToString() + " and does " + calc_damage + " damage";
        //Debug.Log(this.gameObject.name + " does " + calc_damage + " damage");
        //Debug.Log(this.gameObject.name + " has choosen " + .choosenAttack.attackName + " and does " + myAttack.choosenAttack.attackDamage + " dammage");
    }

    void CreateHeroPanel()
    {
        //HeroPanelName = GameObject.FindGameObjectWithTag("Name");
        //HeroPanelStats = GameObject.FindGameObjectWithTag("Stats");

        heroName = HeroPanelName.GetComponent<HeroPanelName>();
        heroStats = HeroPanelStats.GetComponent<HeroPanelStats>();
        

        heroName.heroName.text = hero.name;
        heroStats.heroHP.text = hero.curHP + "/" + hero.baseHP;
        heroStats.heroMP.text = hero.curMP + "/" + hero.baseMP;
        ProgressBar = heroStats.progressBar;
    }

    void UpdateHeroPanel()
    {
        heroStats.heroHP.text = hero.curHP + "/" + hero.baseHP;
        heroStats.heroMP.text = hero.curMP + "/" + hero.baseMP;
    }
}

