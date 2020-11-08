using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Room_Selection : MonoBehaviour
{
    public Text PrivateRoomId;

    public GameMode MyGameMode;
    public DCDL_API_handler MyDCDL_API_Handler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    async public void CreatePrivateRoom()
    {
        string roomId = await MyDCDL_API_Handler.CreateRoom();
        if (roomId == "")
        {
            Debug.Log("The room was not created");
        }
        else
        {
            MyGameMode.CurrentRoom = roomId;
            JoinPrivateRoom(roomId);
        }
    }

    public async void JoinPrivateRoom(string roomId = "")
    {
        if (roomId == "")
        {
            roomId = PrivateRoomId.text;
            MyGameMode.CurrentRoom = roomId;
        }

        Debug.Log("Trying to join room " + roomId);
        bool success = await MyDCDL_API_Handler.ConnectPlayerToRoom(MyGameMode.PlayerId, MyGameMode.Password, roomId);
    }

    public void JoinWorldRoom()
    {
        
    }
}
