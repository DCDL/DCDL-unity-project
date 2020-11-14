using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Description : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void DisplayPlayerDescription()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
    }

    public void HidePlayerDescription()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
