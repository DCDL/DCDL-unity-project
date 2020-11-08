using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Player_Selection : MonoBehaviour
{
    public DCDL_API_handler dCDL_API_Handler;
    public Text PlayerIdText;
    public Text PasswordText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public void ConnectPlayer()
    {
        string playerId = PlayerIdText.text;
        string password = PasswordText.text;

        string body = "{\"playerId\" : \"" + playerId + "\",\"password\" : \"" + password + "\"}";
        string response = await dCDL_API_Handler.PostRequest("/players", body);
        Debug.Log("Reponse from the API : " + response);
    }
}
