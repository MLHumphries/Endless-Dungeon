﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChanger : MonoBehaviour
{
    //This script changes between game menues
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //checks for any key down and advances the menu to the game scene
		if(Input.anyKeyDown == true)
        {
            SceneManager.LoadScene("Game");
        }
	}
}
