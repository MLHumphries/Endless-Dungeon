﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        Wait,
        TakeAction,
        PerformAction,
        CheckAlive,
        Win,
        Lose
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
        Defending,
        Done
    }
    public HeroGUI heroInput;

    public GameObject enemyButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject physicalAttackPanel;
    public GameObject enemySelectPanel;
    public GameObject magicPanel;
   
    //Attacks
    public Transform actionSpacer;
    public Transform magicSpacer;
    public Transform attackSpacer;
    public GameObject actionButton;
    public GameObject physicalAttackButton;
    public GameObject magicButton;
    private List<GameObject> atkBtns = new List<GameObject>();
    private List<GameObject> enemyButtons = new List<GameObject>();

    public List<GameObject> HeroesToManage = new List<GameObject>();
    private HandleTurns heroChoice;

    public Text playerUIText;
    public Text enemyUIText;
    private bool winGame;



	void Start ()
    {
        battleState = PerformAction.Wait;

        EnemyInGame.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroInGame.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        EnemyStateMachine esm1 = GameObject.Find("Enemy").GetComponent<EnemyStateMachine>();
        EnemyStateMachine esm2 = GameObject.Find("Enemy 2").GetComponent<EnemyStateMachine>();

        //if (esm1 != null)
        //{
        //    Debug.Log("Enemy 1 state active");
        //}
        //if (esm2 != null)
        //{
        //    Debug.Log("Enemy 2 state active");
        //}

        heroInput = HeroGUI.Activate;

        attackPanel.SetActive (false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);
        ClearPlayerUIText();
        enemyUIText.text = " ";

        winGame = false;
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
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.HeroToAttack = PerformList[0].attackTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.Action;
                    for (int i = 0; i < HeroInGame.Count; i++)
                    {
                        if (PerformList[0].attackTarget == HeroInGame[i])
                        {
                            ESM.HeroToAttack = PerformList[0].attackTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.Action;
                            //break;
                        }
                        else
                        {
                            PerformList[0].attackTarget = HeroInGame[Random.Range(0, HeroInGame.Count)];
                            ESM.HeroToAttack = PerformList[0].attackTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.Action;
                        }
                        
                    }

                }
                if (PerformList[0].type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.enemyToAttack = PerformList[0].attackTarget;
                    HSM.currentState = HeroStateMachine.TurnState.Action;
                }
                battleState = PerformAction.PerformAction;
                break;

            case (PerformAction.PerformAction):
                //idle state
                break;

            case (PerformAction.CheckAlive):
                if(HeroInGame.Count < 1)
                {
                    battleState = PerformAction.Lose;
                }
                else if(EnemyInGame.Count < 1)
                {
                    battleState = PerformAction.Win;
                }
                else
                {
                    ClearAttackPanel();
                    heroInput = HeroGUI.Activate;
                }
                break;

            case (PerformAction.Win):
                winGame = true;
                for(int i = 0; i < HeroInGame.Count; i++)
                {
                    HeroInGame[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.Waiting;
                }
                StartCoroutine(EndGame());
                break;
            case (PerformAction.Lose):
                winGame = false;
                StartCoroutine(EndGame());
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
            case (HeroGUI.Defending):
                StartCoroutine(Delay());
                heroInput = HeroGUI.Done;
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

    public void EnemyButtons()
    {
        //Clears buttons when enemy dies
        foreach(GameObject button in enemyButtons)
        {
            Destroy(button);
        }
        enemyButtons.Clear();
        //Create new buttons
        foreach(GameObject enemy in EnemyInGame)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = cur_enemy.enemy.name;

            button.enemyPrefab = enemy;

            newButton.transform.SetParent(spacer, false);
            enemyButtons.Add(newButton);
        }
    }

    void HeroInputDone()
    {
        PerformList.Add(heroChoice);
        ClearAttackPanel();
        
        HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HeroesToManage.RemoveAt(0);
        heroInput = HeroGUI.Activate;
        //playerUIText.text = " ";
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

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks.Count > 0)
        {
            foreach (BaseAttack physicalAttack in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks)
            {
                GameObject physicalButton = Instantiate(physicalAttackButton) as GameObject;
                Text physicalAttackButtonText = physicalAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
                physicalAttackButtonText.text = physicalAttack.attackName;
                AttackButton ATB = physicalButton.GetComponent<AttackButton>();
                ATB.physicalAttackToPeform = physicalAttack;
                physicalButton.transform.SetParent(attackSpacer, false);
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

    void ClearAttackPanel()
    {
        enemySelectPanel.SetActive(false);
        attackPanel.SetActive(false);
        magicPanel.SetActive(false);

        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }

    //Attack button for testing
    public void Input1() 
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";
        heroChoice.chosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    //Choosing physical attack if possible
    public void SelectPhysicalAttack(BaseAttack attack)
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";

        heroChoice.chosenAttack = attack;
        playerUIText.text = heroChoice.attackerName + " will use " + heroChoice.chosenAttack.attackName + " and do " + heroChoice.chosenAttack.attackDamage + " damage.";
            //heroName.heroName.text + " has chosen " + BSM.PerformList[0].chosenAttack.attackName.ToString() + " and does " + calc_damage + " damage";
        physicalAttackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
    //Enemy selector
    public void SelectEnemy(GameObject enemy) 
    {
        heroChoice.attackTarget = enemy;
        heroInput = HeroGUI.Done;
    }
    
    //Choosing magic attack if possible
    public void SelectMagicAttack(BaseAttack magicAttack)
    {
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";

        heroChoice.chosenAttack = magicAttack;
        playerUIText.text = heroChoice.attackerName + " will use " + heroChoice.chosenAttack.attackName + " and do " + heroChoice.chosenAttack.attackDamage + " damage.";
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

    public void Defend(BaseAttack defend)
    {
        bool isDefending = true;
        heroInput = HeroGUI.Defending;
        heroChoice.attackerName = HeroesToManage[0].name;
        heroChoice.attackGameObject = HeroesToManage[0];
        heroChoice.type = "Hero";
        heroChoice.chosenAttack = defend;
        heroChoice.attackTarget = HeroesToManage[0];

        HeroesToManage[0].GetComponent<HeroStateMachine>().isDefending = isDefending;
        attackPanel.SetActive(false);
    }


    public void SetPlayerUIText(string msg)
    {
        playerUIText.text = msg;
    }
    public void ClearPlayerUIText()
    {
        playerUIText.text = " ";
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(4.0f);
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2);
        if(winGame)
        {
            SceneManager.LoadScene("Win Scene");
        }
        else
        {
            SceneManager.LoadScene("Game Over");
        }
    }

}
