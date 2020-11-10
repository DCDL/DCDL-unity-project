//This was taken from https://github.com/Rocher0724/socket.io-unity
using Socket.Quobject.SocketIoClientDotNet.Client;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using SocketIOClient;
//using System.Diagnostics;

[Serializable]
public class JsonMessage
{
    public string playerId;
    public string message;
}

public class DCDL_API_handler : MonoBehaviour
{

    public GameMode MyGameMode;


    //This bools are set to true by callbacks so that the main thread knows it needs to do something.
    private bool IsAllowedToConnect = false;
    private bool IsGameAvailable = false;
    private bool IsInNeedOfReconnexion = false;
    private bool IsAbleToReconnect = false;

    private string Endpoint;
    private SocketIO Socket;
    private bool IsLocal = true;
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("Starting the DCDL API handler...");
        if (!IsLocal)
            Endpoint = "https://dcdlbackend.azurewebsites.net";
        else
            Endpoint = "http://localhost:8080";

        await StartSocket();
    }

    async Task StartSocket()
    {

        Socket = new SocketIO(Endpoint);
        Socket.On("dclc", response =>
        {
            string text = response.GetValue<string>();
            Debug.Log("New WS system said : " + text);
        });

        Socket.On("connection", response =>
        {
            string message = response.GetValue<string>();
            Debug.Log("Connection status received from the websocket : " + message);
            if (message == "connected")
            {
                Debug.Log("The backend has connected this player. Starting the room...");
                IsAllowedToConnect = true;
            }
        });

        Socket.On("clientchat", data =>
        {
            string str = data.GetValue<string>();
            Debug.Log("chat message received from the websocket : " + str);
            var json = JsonUtility.FromJson<JsonMessage>(str);
            string playerId = json.playerId;
            string message = json.message;
            MyGameMode.MyRoom.DisplayChatMessage(playerId, message);
        });

        Socket.On("GameAvailable", data =>
        {
            Debug.Log("Game received from the websocket : " + data.GetValue<string>());
        });

        Socket.On("stop", data =>
        {
            string str = data.GetValue<string>();
            Debug.Log("STOP received from the websocket : " + str);
        });

        Socket.OnConnected += async (sender, e) =>
        {
            await Socket.EmitAsync("hi", ".net core");
            Debug.Log("New WS system connected");
        };
        await Socket.ConnectAsync();

        // Debug.Log("Starting socket in room " + MyGameMode.CurrentRoom);
        // var query = new Dictionary<string, string>();
        // query.Add("room", MyGameMode.CurrentRoom);
        // var options = new IO.Options();
        // options.Query = query;

        // Socket = IO.Socket(Endpoint, options);
        // Socket.On(QSocket.EVENT_CONNECT, () =>
        // {
        //     Debug.Log("WebSocket connected succesfully");
        // });

        // Socket.On(QSocket.EVENT_DISCONNECT, () =>
        //{
        //    Debug.Log("WebSocket was disconnected noooooo !!!");
        //    IsInNeedOfReconnexion = true;
        //});

        // Socket.On(QSocket.EVENT_MESSAGE, (data) =>
        // {
        //     Debug.Log("Some weird received : " + data);
        // });


        // Socket.On("dclc", data =>
        //{
        //    string message = data.ToString();
        //    Debug.Log("Debug received from the websocket : " + message);
        //    if (IsInNeedOfReconnexion)
        //        IsAbleToReconnect = true;
        //});

        // Socket.On("connection", data =>
        // {
        //     string message = data.ToString();
        //     Debug.Log("Connection status received from the websocket : " + message);
        //     if (message == "connected")
        //     {
        //         Debug.Log("The backend has connected this player. Starting the room...");
        //         IsAllowedToConnect = true;
        //     }
        // });

        // Socket.On("clientchat", data =>
        // {
        //     string str = data.ToString();
        //     Debug.Log("chat message received from the websocket : " + str);
        //     var json = JsonUtility.FromJson<JsonMessage>(str);
        //     string playerId = json.playerId;
        //     string message = json.message;
        //     MyGameMode.MyRoom.DisplayChatMessage(playerId, message);
        // });

        // Socket.On("gameavailable", data =>
        // {
        //     Debug.Log("Game received from the websocket : " + data.ToString());
        // });

        // Socket.On("stop", data =>
        // {
        //     string str = data.ToString();
        //     Debug.Log("STOP received from the websocket : " + str);
        // });
    }

    // Update is called once per frame
    async void Update()
    {
        if (IsAllowedToConnect)
        {
            IsAllowedToConnect = false;
            MyGameMode.StartRoom();
        }

        if (IsGameAvailable)
        {
            IsGameAvailable = true;
            MyGameMode.NewGame();
        }

        if (IsInNeedOfReconnexion && IsAbleToReconnect)
        {
            IsInNeedOfReconnexion = false;
            IsAbleToReconnect = false;
            await ConnectPlayerToRoom(MyGameMode.PlayerId, MyGameMode.Password, MyGameMode.CurrentRoom);
            Debug.Log("I tried to reconnect.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroying the socket.");
        //Socket.Disconnect();
    }

    public async Task<bool> ConnectPlayerToRoom(string playerId, string passwword, string roomId)
    {
        //await StartSocket();

        string response = await GetRequest("/rooms/" + roomId);

        if (response == "")
        {
            Debug.Log("This room could not be found.");
            return false;
        }

        string socketMessage = playerId + ";" + passwword + ";" + roomId;
        await Socket.EmitAsync("playerconnection", socketMessage);
        return true;
    }

    public async Task<bool> ConnectPlayer(string playerId, string password)
    {
        string body = "{\"playerId\" : \"" + playerId + "\",\"password\" : \"" + password + "\"}";
        string response = await PostRequest("/players", body);

        if (response == "")
            return false;

        else
            return true;
    }

    async public Task<string> CreateRoom()
    {
        return await PostRequest("/rooms", "");
    }

    public async Task<string> PostRequest(string uri, string body)
    {
        //Debug.Log("Sending POST request " + body + " to " + Endpoint + uri);
        var request = new UnityWebRequest(Endpoint + uri, "POST");

        if (body != "")
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(body);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        //UnityWebRequest www = UnityWebRequest.Post(Endpoint + uri, formData);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error :  + " + request.error);
            return "";
        }
        else
        {
            return request.downloadHandler.text;
        }
    }

    public async Task<string> GetRequest(string uri)
    {
        //Debug.Log("Sending GET request to " + Endpoint + uri);

        var request = new UnityWebRequest(Endpoint + uri, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        //UnityWebRequest www = UnityWebRequest.Post(Endpoint + uri, formData);
        await request.Send();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error :  + " + request.error);
            return "";
        }
        else
        {
            return request.downloadHandler.text;
        }
    }

    public async void SendMessageToSocket(string title, string message)
    {
        await Socket.EmitAsync(title, message);
    }
}

