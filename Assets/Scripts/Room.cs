using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameMode MyGameMode;

    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string content);

    public DCDL_API_handler MyDCDL_API_Handler;

    public Button ButtonReconnect;

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

        NumbersObjective.SetActive(false);
        ContainerSetParts.SetActive(false);
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
                if(list[0] == "Bertrand Renard")
                    playerMessage.fontStyle = FontStyle.Bold;
            }
            PendingMessages.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContainerMessages.GetComponent<RectTransform>());
        }

        if(Input.GetKey(KeyCode.Return))
        {
            if (InputMessageToSend.text != "")
                SendChatMessage();
        }
    }

    public async void NewGame(string nextSetId)
    {
        //First, get this new game.
        Debug.LogError("Setting up a new game.");
        MyGameMode.CurrentSet = nextSetId;
        Set set = await MyDCDL_API_Handler.GetSet(nextSetId);
        Debug.LogError("The backend gave us the game " + JsonConvert.SerializeObject(set));

        //setup the numbers or the letters into the parts
        List<string> options = set.question.Split(',').ToList();

        bool NeedToShow4MoreParts = true;
        NumbersObjective.SetActive(false);

        if (set.mode == "numbers")
        {
            NeedToShow4MoreParts = false;
            NumbersObjective.SetActive(true);
            Text objective = NumbersObjective.transform.GetChild(0).GetComponent<Text>();
            objective.text = options[0];
            options.RemoveAt(0);
        }

        //Write the right value into the parts
        for (int i = 0; i < options.Count; i++)
        {
            SetParts[i].transform.GetChild(0).GetComponent<Text>().text = options[i];
        }
        
        //hide or show the parts.
        for (int i = SetParts.Count - 4; i < SetParts.Count; i++)
        {
            SetParts[i].SetActive(NeedToShow4MoreParts);
        }

        //show the whole thing
        ContainerSetParts.SetActive(true);
    }

    public async void ConcludeSet()
    {
        ContainerSetParts.SetActive(false);
        
        Dictionary<string, string> playerScores = new Dictionary<string, string>();
        List<string> winners = new List<string>();

        Set set = await MyDCDL_API_Handler.GetSet(MyGameMode.CurrentSet);
        Debug.LogError(JsonConvert.SerializeObject(set));
        List<PlayerInSet> players = set.playersInSet;
        if (players == null)
        {
            Debug.LogError("No one played this game...");
            return;
        }
        foreach(PlayerInSet player in players)
        {
            playerScores[player.playerId] = player.score;
            if (player.victory == "true")
            {
                winners.Add(player.playerId);
            }

            string message = "";
            if (winners.Count == 1)
                message += "Le gagnant est " + winners[0]  + ", avec un score de " + playerScores[winners[0]] + ". ";
            else if (winners.Count > 1)
                message += "Les gagnants sont " + string.Join(", ", winners) + ", avec un score de " + playerScores[winners[0]] + ". ";
            else if (winners.Count == 0)
                message += "Personne n'a gagné cette manche ! ";

            if(playerScores.Count > 1)
            {
                message += "Les scores sont de ";
                foreach(KeyValuePair<string, string> entry in playerScores)
                {
                    message += entry.Value + " pour " + entry.Key + ", ";
                }
                message += "c'est du beau travail !";
            }

            DisplayChatMessage("Bertrand Renard", message);

            //GameObject gameObjectMessage = Instantiate(PrefabMessage, ContainerMessages.transform);
            //Text bertrandName = gameObjectMessage.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            //Text bertrandMessage = gameObjectMessage.transform.GetChild(1).GetChild(1).GetComponent<Text>();
            //bertrandMessage.fontStyle = FontStyle.Bold;
            //bertrandName.text = "Bertrand Renard";
            //bertrandMessage.text = message;

        }
    }
    public async void ReconnectRoom()
    {
        bool success = await MyDCDL_API_Handler.ConnectPlayerToRoom(MyGameMode.PlayerId, MyGameMode.Password, MyGameMode.CurrentRoom);
        if (success)
            ButtonReconnect.interactable = false;
        DisplayChatMessage("Bertrand Renard", "C'est reparti !");
    }

    public void ShareRoom()
    {
        DisplayChatMessage("Bertrand Renard", "Le code de cette salle est " + MyGameMode.CurrentRoom);
        CopyToClipboard(MyGameMode.CurrentRoom);
    }

    public void Stop()
    {
        Debug.Log("Stopping the runner for this room.");
        ButtonReconnect.interactable = true;
        DisplayChatMessage("Bertrand Renard", "Cette partie a l'air inactive. Quand vous voudrez jouer à nouveau, cliquez dans le coin en haut à gauche.");
        return;
    }

    public async void SendChatMessage()
    {
        string playerId = MyGameMode.PlayerId;
        string message = InputMessageToSend.text;
        InputMessageToSend.text = "";
        string data = "{ \"playerId\" : \"" + playerId + "\",\"message\" : \"" + message + "\",\"roomId\" : \"" + MyGameMode.CurrentRoom + "\"}";
        //Debug.LogError("Sending message : " + data);
        MyDCDL_API_Handler.SendMessageToSocket("serverchat", data);
        await MyDCDL_API_Handler.SendAction(MyGameMode.CurrentSet, playerId, message);

    }

    public void DisplayChatMessage(string player, string message)
    {
        //Debug.LogError("New chat message : " + player + " said " + message);
        List<string> fullmessage = new List<string>();
        fullmessage.Add(player);
        fullmessage.Add(message);
        PendingMessages.Add(fullmessage);
    }
}
