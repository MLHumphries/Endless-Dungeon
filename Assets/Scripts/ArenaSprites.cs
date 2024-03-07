using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSprites : MonoBehaviour
{
    public Material[] matChoice;
    public GameObject[] objectMat;

    // Use this for initialization
    void Start()
    {
        foreach (GameObject mat in objectMat)
        {
            //mat.renderer.material = matChoice[Random.Range(0, matChoice.Length)];
            mat.GetComponent<Renderer>().material = matChoice[Random.Range(0, matChoice.Length)];
        }

    }

}
