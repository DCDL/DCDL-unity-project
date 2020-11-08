using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public List<GameObject> GameCanvasObjects;
    public string PlayerId { get; set; }
    public string Password { get; set; }
    public string CurrentRoom { get; set; }

    Player_Selection MyPlayer_Selection;
    Room_Selector MyRoom_Selector;


    public enum GameCanvas
    {
        PLAYERSELECTION = 0,
        ROOMSELECTION = 1,
        ROOM = 2
    };

    // Start is called before the first frame update
    void Start()
    {
        //GameCanvasObjects.Add(GameObject.Find("canvas_player_selection"));
        MyPlayer_Selection = GameCanvasObjects[0].GetComponent<Player_Selection>();

        //GameCanvasObjects.Add(GameObject.Find("canvas_room_selection"));
        MyRoom_Selector = GameCanvasObjects[1].GetComponent<Room_Selector>();

        //GameCanvasObjects.Add(GameObject.Find("canvas_room"));
        SwitchCanvas(GameCanvas.PLAYERSELECTION);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchCanvas(GameCanvas canvas)
    {
        int index = (int)canvas;
        for (int i = 0; i < GameCanvasObjects.Count; i++)
        {
            try
            {
                if (i != index)
                    GameCanvasObjects[i].GetComponent<Canvas>().enabled = false;
                else
                    GameCanvasObjects[i].GetComponent<Canvas>().enabled = true;
            }
            catch (Exception e)
            {
                Debug.Log("Error : " + e);
            }
        }
    }

    public void SetPlayer(string playerId)
    {
        PlayerId = playerId;
    }

    public void StartRoom()
    {
        MyRoom_Selector.StartRoom();
    }
}
