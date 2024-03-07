using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float turnSpeed;
    private Rigidbody rbdy;

    public int combatChance;

	// Use this for initialization
	void Start ()
    {
        rbdy = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate (new Vector3(0, 0, moveVertical));

        float turn = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        transform.Rotate (new Vector3 (0, turn, 0));

        if(turn < .1)
        {
            rbdy.angularVelocity = Vector3.zero;
        }

        
    }
    //Triggers combat
    private void OnTriggerEnter(Collider other)
    {
        combatChance = Random.Range(0, 2);

        if (combatChance > 0)
        {
            SceneManager.LoadScene("Combat");
        }
        else
        {
            Debug.Log("No Combat");
        }

    }


}
