using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using System.Threading.Tasks;

//This was taken from https://github.com/Rocher0724/socket.io-unity
using Socket.Quobject.SocketIoClientDotNet.Client;

public class DCDL_API_handler : MonoBehaviour
{

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Socket.Disconnect();
    }

    async public void ConnectPlayer(string playerId, string password)
    {
        string body = "{\"playerId\" : \"" + playerId + "\",\"password\" : \"" + password + "\"}";
        string response = await PostRequest(Endpoint + "/players", body);
    }

    public async Task<string> PostRequest(string uri, string body)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(body));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post(Endpoint + uri, formData);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return www.error;
        }
        else
        {
            Debug.Log("Form upload complete : " + www.downloadHandler.text);
            return www.downloadHandler.text;
        }
    }
}

