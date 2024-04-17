using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        Wait,
        TakeAction,
        PerformAction
    }

    public PerformAction battleState;

    public List<HandleTurns> PerformList = new List<HandleTurns>();

    public List<GameObject> HeroInGame = new List<GameObject>();
    public List<GameObject> EnemyInGame = new List<GameObject>();

    public enum HeroGUI
    {
        Activate,
        Waiting,
        Input1,
        Input2,
        Done
    }
    public HeroGUI heroInput;

    public GameObject enemyButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject physicalAttackPanel;
    public GameObject enemySelectPanel;
    public GameObject magicPanel;
   
    //Magic Attacks
    public Transform actionSpacer;
    public Transform magicSpacer;
    public GameObject actionButton;
    public GameObject magicButton;
    private List<GameObject> atkBtns = new List<GameObject>();

    public List<GameObject> HeroesToManage = new List<GameObject>();
    private HandleTurns heroChoice;



	void Start ()
    {
        battleState = PerformAction.Wait;

        EnemyInGame.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroInGame.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        EnemyStateMachine esm1 = GameObject.Find("Enemy").GetComponent<EnemyStateMachine>();
        EnemyStateMachine esm2 = GameObject.Find("Enemy 2").GetComponent<EnemyStateMachine>();

        if (esm1 != null)
        {
            Debug.Log("Enemy 1 state active");
        }
        if (esm2 != null)
        {
            Debug.Log("Enemy 2 state active");
        }

        heroInput = HeroGUI.Activate;

        attackPanel.SetActive (false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);

        EnemyButtons();
	}
	
	
	void Update ()
    {
        switch (battleState)
        {
            case (PerformAction.Wait):
                if(PerformList.Count > 0)
                {
                    battleState = PerformAction.TakeAction;
                }
                break;
            case (PerformAction.TakeAction):
                GameObject performer = GameObject.Find(PerformList[0].attackerName);

                if(PerformList[0].type == "Enemy")
                {
                    Debug.Log(PerformList[0].attackGameObject);
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.HeroToAttack = PerformList[0].attackerTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.Action;
                    for (int i = 0; i < HeroInGame.Count; i++)
                    {
                        if (PerformList[0].attackerTarget == HeroInGame[i])
                        {
                            ESM.HeroToAttack = PerformList[0].attackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.Action;

                        }
                        else
                        {
                            PerformList[0].attackerTarget = HeroInGame[Random.Range(0, HeroInGame.Count)];
                            ESM.HeroToAttack = PerformList[0].attackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.Action;
                        }
                    }

                }
                if (PerformList[0].type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.enemyToAttack = PerformList[0].attackerTarget;
                    HSM.currentState = HeroStateMachine.TurnState.Action;
                }
                battleState = PerformAction.PerformAction;
                break;
            case (PerformAction.PerformAction):

                break;
        }
        switch (heroInput)
        {
            //Pulls from available heroes, activates cube selector icon/prefab
            case (HeroGUI.Activate): 
                if(HeroesToManage.Count > 0)
                {
                    HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroChoice = new HandleTurns();

                    attackPanel.SetActive(true);
                    //populate Attack panels
                    CreateAttackButtons();
                    heroInput = HeroGUI.Waiting;
                }

                break;
            //Idle state
            case (HeroGUI.Waiting): 

                break;
            case (HeroGUI.Done):
                HeroInputDone();
                break;
        }
	}

    public void CollectActions(HandleTurns input)
    {
        PerformList.Add(input);
    }

    void EnemyButtons()
    {
        foreach(GameObject enemy in EnemyInGame)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = cur_enemy.enemy.name;

            button.enemyPrefab = enemy;

            newButton.transform.SetParent(spacer);
        }
    }

    void HeroInputDone()
    {
        PerformList.Add(heroChoice);
        enemySelectPanel.SetActive(false);
        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        //atkBtns.Clear();

        HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HeroesToManage.RemoveAt(0);
        heroInput = HeroGUI.Activate;
    }

    void CreateAttackButtons()
    {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        attackButtonText.text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(() => OpenPhysicalAttacksMenu());
        attackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(attackButton);

        GameObject magicAttackButton = Instantiate(actionButton) as GameObject;
        Text magicAttackButtonText = magicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        magicAttackButtonText.text = "Magic";
        magicAttackButton.GetComponent<Button>().onClick.AddListener(() => OpenMagicAttacksMenu());
        magicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(magicAttackButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.physicalAttacks.Count > 0)
        {
            foreach (BaseAttack physicalAttack in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.physicalAttacks)
            {
                GameObject physicalButton = Instantiate(magicButton) as GameObject;
                Text magicButtonText = magicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                magicButtonText.text = physicalAttack.attackName;
                AttackButton ATB = physicalButton.GetComponent<AttackButton>();
                ATB.magicAttackToPerform = physicalAttack;
                physicalButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(physicalButton);
            }
        }
        else
        {
            attackButton.GetComponent<Button>().interactable = false;
        }

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
        {
            foreach (BaseAttack magicAttack in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                Text magicButtonText = magicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                magicButtonText.text = magicAttack.attackName;
                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.magicAttackToPerform = magicAttack;
                MagicButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(MagicButton);
            }
        }
        else
        {
            magicAttackButton.GetComponent<Button>().interactable = false;
        }
    }

    //Attack button
    public void Input1() 
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";
        heroChoice.chosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void SelectPhysicalAttack(BaseAttack attack)
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";

        heroChoice.chosenAttack = attack;
        physicalAttackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
    //Enemy selector
    public void SelectEnemy(GameObject enemy) 
    {
        heroChoice.attackerTarget = enemy;
        heroInput = HeroGUI.Done;
    }
    
    //Choosing magic attack if possible
    public void SelectMagicAttack(BaseAttack magicAttack)
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";

        heroChoice.chosenAttack = magicAttack;
        magicPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
    //Switching to magic attack menu
    public void OpenMagicAttacksMenu()
    {
        attackPanel.SetActive(false);
        magicPanel.SetActive(true);
    }
    
    //Switch to physical attack menu
    public void OpenPhysicalAttacksMenu()
    {
        attackPanel.SetActive(false);
        physicalAttackPanel.SetActive(true);
    }

}
