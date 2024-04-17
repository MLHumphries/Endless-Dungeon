using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSprites : MonoBehaviour
{
    public Material[] floorMatChoice;
    public Material[] wallMatChoice;
    public GameObject floor;
    public GameObject wall;

    // Use this for initialization
    void Start()
    {       
            floor.GetComponent<Renderer>().material = floorMatChoice[Random.Range(0, floorMatChoice.Length)];
            wall.GetComponent<Renderer>().material = wallMatChoice[Random.Range(0, wallMatChoice.Length)];
            //mat.renderer.material = matChoice[Random.Range(0, matChoice.Length)];
    }

}
