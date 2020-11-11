using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameMode MyGameMode;
    public DCDL_API_handler MyDCDL_API_Handler;

    //chat
    public InputField InputMessageToSend;
    public GameObject PrefabMessage;
    public GameObject ContainerMessages;
    List<List<string>> PendingMessages;

    //gameavailable
    public GameObject ContainerSetParts;
    private List<GameObject> SetParts;   //10 for letters, 6 for numbers (so 4 will be hidden)
    public GameObject NumbersObjective;

    // Start is called before the first frame update
    void Start()
    {
        PendingMessages = new List<List<string>>();
        SetParts = new List<GameObject>();
        for (int i = 0; i < ContainerSetParts.transform.childCount; i++)
            SetParts.Add(ContainerSetParts.transform.GetChild(i).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (PendingMessages.Count != 0)
        {
            foreach (List<string> list in PendingMessages)
            {
                GameObject gameObjectMessage = Instantiate(PrefabMessage, ContainerMessages.transform);
                Text playerName = gameObjectMessage.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                Text playerMessage = gameObjectMessage.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                playerName.text = list[0];
                playerMessage.text = list[1];
            }
            PendingMessages.Clear();
        }
    }

    public async void NewGame(string nextSetId)
    {
        //First, get this new game.
        Debug.Log("Setting up a new game.");
        Set set = await MyDCDL_API_Handler.GetSet(nextSetId);
        Debug.Log("The backend gave us the game " + JsonConvert.SerializeObject(set));

        //setup the numbers or the letters into the parts
        string[] options = set.question.Split(',');
        for (int i = 0; i < options.Length; i++)
        {
            int j = (set.mode == "numbers") ? i+1 : i;
            SetParts[i].transform.GetChild(0).GetComponent<Text>().text = options[i];
        }

        bool NeedToShow4MoreParts = true;
        NumbersObjective.SetActive(false);

        if (set.mode == "numbers")
        {
            NeedToShow4MoreParts = false;
            NumbersObjective.SetActive(true);
            Text objective = NumbersObjective.transform.GetChild(0).GetComponent<Text>();
            objective.text = options[0];
        }

        for (int i = SetParts.Count - 4; i < SetParts.Count; i++)
        {
            SetParts[i].SetActive(NeedToShow4MoreParts);
        }
    }

    internal void Setup()
    {
        //
    }

    public async void SendChatMessage()
    {
        string playerId = MyGameMode.PlayerId;
        string message = InputMessageToSend.text;
        InputMessageToSend.text = "";
        string data = "{ \"playerId\" : \"" + playerId + "\",\"message\" : \"" + message + "\",\"roomId\" : \"" + MyGameMode.CurrentRoom + "\"}";
        //Debug.Log("Sending message : " + data);
        MyDCDL_API_Handler.SendMessageToSocket("serverchat", data);
        await MyDCDL_API_Handler.SendAction(playerId, MyGameMode.CurrentRoom, data);

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
