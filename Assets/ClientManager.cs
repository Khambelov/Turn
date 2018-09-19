using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientManager : NetworkManager
{
    public Image loadScreen;
    public Image turnScren;

    public Text number;

    private void Awake()
    {
        loadScreen.gameObject.SetActive(true);
    }

    // Use this for initialization
    void Start()
    {
        StartClient();
        client.RegisterHandler(100, Error);
        client.RegisterHandler(200, CheckAnswer);
        client.RegisterHandler(404, ReturnInMain);
    }

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Client is started!");
        base.OnStartClient(client);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client is Connected!");
        loadScreen.gameObject.SetActive(false);
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
        StopClient();
    }

    public void GetNumber()
    {
        Message msg = new Message();
        msg.message = "get";

        client.Send(200, msg);

        loadScreen.gameObject.SetActive(true);
    }

    public void ClearTurn()
    {
        Message msg = new Message();
        msg.message = "clear";

        client.Send(404, msg);

        loadScreen.gameObject.SetActive(true);
    }

    void CheckAnswer(NetworkMessage msg)
    {
        string code = msg.ReadMessage<Message>().message;

        turnScren.gameObject.SetActive(true);
        number.text = code;
        loadScreen.gameObject.SetActive(false);
    }

    void ReturnInMain(NetworkMessage msg)
    {
        int result = int.Parse(msg.ReadMessage<Message>().message);

        if (result == 200)
        {
            number.text = null;
            turnScren.gameObject.SetActive(false);
            loadScreen.gameObject.SetActive(false);
        }
    }

    void Error(NetworkMessage msg)
    {
        Debug.Log(msg.ReadMessage<Message>().message);
        loadScreen.gameObject.SetActive(false);
    }
}

public class Message : MessageBase
{
    public string message;
}
