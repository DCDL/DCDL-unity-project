﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public List<GameObject> GameCanvasObjects;
    public string PlayerId { get; set; }
    public string Password { get; set; }
    public string CurrentRoom { get; set; }
    public string CurrentSet { get; set; }

    public Player_Selection MyPlayer_Selection { get; private set; }
    public Room_Selection MyRoom_Selection { get; private set; }
    public Room MyRoom { get; private set; }

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
        MyRoom_Selection = GameCanvasObjects[1].GetComponent<Room_Selection>();

        //GameCanvasObjects.Add(GameObject.Find("canvas_room"));
        this.MyRoom = GameCanvasObjects[2].GetComponent<Room>();

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
                Debug.LogError("Error : " + e);
            }
        }
    }

    void DisplayRoomSelection()
    {
        SwitchCanvas(GameMode.GameCanvas.ROOMSELECTION);
        CurrentRoom = "";
        CurrentSet = "";
    }

    void DisplayPlayerSelection()
    {
        SwitchCanvas(GameMode.GameCanvas.PLAYERSELECTION);
        CurrentRoom = "";
        CurrentSet = "";
        PlayerId = "";
        Password = "";
    }
}
