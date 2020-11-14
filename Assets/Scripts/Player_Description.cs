using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Description : MonoBehaviour
{
    public GameMode MyGameMode;
    public DCDL_API_handler MyDCDL_API_Handler;

    public Dropdown Dropdown;
    public List<PlayerAPI> Players;

    public Text EloText;
    public Text VictoriesText;
    public Text GameCountText;

    // Start is called before the first frame update
    void Start()
    {
        Players = new List<PlayerAPI>();
        Dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChangedHandler(Dropdown);
        });
    }

    // Update is called once per frame

    public async void DisplayPlayerDescription()
    {
        Players.Clear();
        gameObject.GetComponent<Canvas>().enabled = true;
        RoomAPI room = await MyDCDL_API_Handler.GetRoom(MyGameMode.CurrentRoom);
        foreach (string playerId in room.playerIds)
        {
            PlayerAPI player = await MyDCDL_API_Handler.GetPlayer(playerId);
            Players.Add(player);
            Dropdown.AddOptions(new List<string> { playerId });
        }
    }

    public void HidePlayerDescription()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    private void DropdownValueChangedHandler(Dropdown target)
    {
        Debug.Log("selected: " + Dropdown.options[Dropdown.value].text);
        foreach(PlayerAPI player in Players)
        {
            if(player.playerId == Dropdown.options[Dropdown.value].text)
            {
                EloText.text = player.ELO;
                VictoriesText.text = player.victoriesCount;
                GameCountText.text = player.gameCount;
                break;
            }
        }
    }

    private void OnDestroy()
    {
        Dropdown.onValueChanged.RemoveAllListeners();
    }
}
