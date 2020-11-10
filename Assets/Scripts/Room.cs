using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameMode MyGameMode;
    public DCDL_API_handler MyDCDL_API_Handler;

    public InputField InputMessageToSend;
    public GameObject PrefabMessage;
    public GameObject ContainerMessages;
    List<List<string>> PendingMessages;

    // Start is called before the first frame update
    void Start()
    {
        PendingMessages = new List<List<string>>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PendingMessages.Count != 0)
        {
            foreach(List<string> list in PendingMessages)
            {
                GameObject gameObjectMessage = Instantiate(PrefabMessage, ContainerMessages.transform);
                Text playerName = gameObjectMessage.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                Text playerMessage = gameObjectMessage.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                playerName.text = list[0];
                playerMessage.text = list[1];
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
            PendingMessages.Clear();
        }
    }

    internal void NewGame()
    {
        throw new NotImplementedException();
    }

    internal void Setup()
    {
//
    }

    public void SendChatMessage()
    {
        string playerId = MyGameMode.PlayerId;
        string message = InputMessageToSend.text;
        InputMessageToSend.text = "";
        string data = "{ \"playerId\" : \"" + playerId + "\",\"message\" : \"" + message + "\",\"roomId\" : \"" + MyGameMode.CurrentRoom + "\"}";
        //Debug.Log("Sending message : " + data);
        MyDCDL_API_Handler.SendMessageToSocket("serverchat", data);

    }

    public void DisplayChatMessage(string player, string message)
    {
        //Debug.Log("New chat message : " + player + " said " + message);
        List<string> fullmessage = new List<string>();
        fullmessage.Add(player);
        fullmessage.Add(message);
        PendingMessages.Add(fullmessage);
    }
}
