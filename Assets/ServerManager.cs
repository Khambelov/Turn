using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkManager
{

    public static ServerManager serverManager;

    Dictionary<int, string> firstQueue;
    public Dictionary<int, string> getFirstList { get { return firstQueue; } }
    Dictionary<int, string> secondQueue;
    public Dictionary<int, string> getSecondList { get { return secondQueue; } }

    bool firstIsFree;
    public bool firstToilet { get { return firstIsFree; } set { firstIsFree = value; } }
    bool secondIsFree;
    public bool secondToilet { get { return secondIsFree; } set { secondIsFree = value; } }

    int fCounter;
    int sCounter;

    private void Awake()
    {
        serverManager = this;
        firstQueue = new Dictionary<int, string>();
        secondQueue = new Dictionary<int, string>();

        firstIsFree = true;
        secondIsFree = true;

        fCounter = 1;
        sCounter = 1;
    }

    private void Start()
    {
        StartServer();
        NetworkServer.RegisterHandler(200, TakeNumber);
        NetworkServer.RegisterHandler(404, DeleteNumber);
    }

    public override void OnStartServer()
    {
        Debug.Log("Server is started!");

        StartCoroutine(ClearCounters());

        base.OnStartServer();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Client with conId: " + conn.connectionId + "is connected!");
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client with conId: " + conn.connectionId + "is disconnected!");

        if (firstQueue.ContainsKey(conn.connectionId))
            firstQueue.Remove(conn.connectionId);
        else if (secondQueue.ContainsKey(conn.connectionId))
            secondQueue.Remove(conn.connectionId);

        NumbersManager.numbersManager.CheckDisconnect(conn.connectionId);

        base.OnServerDisconnect(conn);
    }

    void TakeNumber(NetworkMessage msg)
    {
        string code;

        if (msg.ReadMessage<Message>().message != "get")
        {
            Message error = new Message();
            error.message = "Ошибка при получении номера!";
            NetworkServer.SendToClient(msg.conn.connectionId, 100, error);
            return;
        }

        if (firstIsFree)
        {
            code = string.Concat("A ", GenerateNumber(fCounter));
            fCounter += 1;
            firstQueue.Add(msg.conn.connectionId, code);

        }
        else if (secondIsFree)
        {
            code = string.Concat("B ", GenerateNumber(sCounter));
            sCounter += 1;
            secondQueue.Add(msg.conn.connectionId, code);

        }
        else
        {
            if (firstQueue.Count <= secondQueue.Count)
            {
                code = string.Concat("A ", GenerateNumber(fCounter));
                fCounter += 1;
                firstQueue.Add(msg.conn.connectionId, code);
            }
            else
            {
                code = string.Concat("B ", GenerateNumber(sCounter));
                sCounter += 1;
                secondQueue.Add(msg.conn.connectionId, code);
            }
        }

        Message result = new Message();
        result.message = code;

        NetworkServer.SendToClient(msg.conn.connectionId, 200, result);

        NumbersManager.numbersManager.UpdateTurn();
    }

    void DeleteNumber(NetworkMessage msg)
    {
        if (msg.ReadMessage<Message>().message != "clear")
        {
            Message error = new Message();
            error.message = "Ошибка при выходе из очереди!";
            NetworkServer.SendToClient(msg.conn.connectionId, 100, error);
            return;
        }

        if (firstQueue.ContainsKey(msg.conn.connectionId))
        {
            firstQueue.Remove(msg.conn.connectionId);

            firstIsFree = true;
        }
        else if (secondQueue.ContainsKey(msg.conn.connectionId))
        {
            secondQueue.Remove(msg.conn.connectionId);

            secondIsFree = true;
        }

        Message result = new Message();
        result.message = "200";

        NetworkServer.SendToClient(msg.conn.connectionId, 404, result);

        NumbersManager.numbersManager.UpdateTurn();
    }

    string GenerateNumber(int count)
    {
        return string.Concat(count < 10 ? "00" : count < 100 ? "0" : "", count);
    }

    IEnumerator ClearCounters()
    {
        float time = 1080f;

        while (time > 0)
        {
            if (fCounter >= 999 || sCounter >= 999)
                break;

            time -= 1;
            yield return new WaitForSeconds(1f);
        }

        fCounter = 1;
        sCounter = 1;

        StartCoroutine(ClearCounters());

        yield break;
    }
}
