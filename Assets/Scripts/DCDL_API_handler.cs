//This was taken from https://github.com/Rocher0724/socket.io-unity
using Socket.Quobject.SocketIoClientDotNet.Client;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
//using System.Diagnostics;

public class DCDL_API_handler : MonoBehaviour
{

    public GameMode MyGameMode;

    private bool IsOnline;
    private string Endpoint;
    private QSocket Socket;
    private bool IsLocal = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting the DCDL API handler...");
        if (!IsLocal)
            Endpoint = "https://dcdlbackend.azurewebsites.net";
        else
            Endpoint = "http://localhost:8080";

        Socket = IO.Socket(Endpoint);
        Socket.On(QSocket.EVENT_CONNECT, () =>
        {
            Debug.Log("WebSocket connected succesfully");
        });

        Socket.On(QSocket.EVENT_MESSAGE, (data) =>
        {
            Debug.Log("Message received : " + data);
        });


        Socket.On("dclc", data =>
        {
            string message = data.ToString();
            Debug.Log("Debug received from the websocket : " + message);
        });

        Socket.On("connection", data =>
        {
            string message = data.ToString();
            Debug.Log("Connection status received from the websocket : " + message);
            if (message == "connected")
            {
                Debug.Log("The backend has connected this player. Starting the room...");
                MyGameMode.StartRoom();
            }
        });



    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Socket.Disconnect();
    }

    public async Task<bool> ConnectPlayerToRoom(string playerId, string passwword, string roomId)
    {
        string response = await GetRequest("/rooms/" + roomId);

        if (response == "")
        {
            Debug.Log("This room could not be found.");
            return false;
        }

        string socketMessage = playerId + ";" + passwword + ";" + roomId;
        Socket.Emit("playerconnection", socketMessage);
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

    public void SendMessageToSocket(string title, string message)
    {
        Socket.Emit(title, message);
    }
}

