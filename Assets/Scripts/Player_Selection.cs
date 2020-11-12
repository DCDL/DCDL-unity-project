using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Player_Selection : MonoBehaviour
{
    public GameMode MyGameMode;
    public DCDL_API_handler MyDCDL_API_Handler;
    public InputField PlayerIdText;
    public InputField PasswordText;
    private string PlayerId;

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

        bool connected = await MyDCDL_API_Handler.ConnectPlayer(playerId, password);

        if (connected)
        {
            MyGameMode.PlayerId = playerId;
            MyGameMode.Password = password;
            Debug.LogError("Player selected : " + playerId + ". Switching planel.");
            MyGameMode.SwitchCanvas(GameMode.GameCanvas.ROOMSELECTION);
        }

        else
        {
            Debug.LogError("Unable to connect.");
        }
    }
}
