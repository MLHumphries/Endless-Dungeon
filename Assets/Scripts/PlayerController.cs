using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float turnSpeed;
    private Rigidbody rbdy;

    public GameObject flashScreen;

    public int combatChance;
    private bool isFroze = false;
	// Use this for initialization
	void Start ()
    {
        rbdy = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        

        float turn = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        
        if (isFroze)
        {
            moveVertical = 0;
            turn = 0;
        }
        transform.Translate(new Vector3(0, 0, moveVertical));
        transform.Rotate(new Vector3(0, turn, 0));

        if (turn < .1)
        {
            rbdy.angularVelocity = Vector3.zero;
        }
        if(flashScreen != null)
        {
            if(flashScreen.GetComponent<Image>().color.a > 0)
            {
                var color = flashScreen.GetComponent<Image>().color;
                color.a -= 0.01f;
                flashScreen.GetComponent<Image>().color = color;
            }
        }
        
    }
    //Triggers combat
    private void OnTriggerEnter(Collider other)
    {
        combatChance = Random.Range(0, 2);

        if (combatChance > 0)
        {
            isFroze = true;
            ChangeAlphaColor();
            Invoke("LoadCombat", 3.0f);
        }
        else
        {
            Debug.Log("No Combat");
        }

    }

    private void ChangeAlphaColor()
    {
        var color = flashScreen.GetComponent<Image>().color;
        color.a = 0.8f;
        flashScreen.GetComponent<Image>().color = color;
        //SceneManager.LoadScene("Combat");
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.5f);
    }

    private void LoadCombat()
    {
        SceneManager.LoadScene("Combat");
    }


}
