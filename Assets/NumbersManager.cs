using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NumbersManager : MonoBehaviour
{

    public static NumbersManager numbersManager;

    public Text fNumber;
    public Text sNumber;

    int fCon;
    int sCon;

    private void Awake()
    {
        numbersManager = this;
        fCon = 0;
        sCon = 0;
    }

    public void UpdateTurn()
    {
        if (ServerManager.serverManager.getFirstList.Count > 0 || ServerManager.serverManager.getSecondList.Count > 0)
        {
            if (ServerManager.serverManager.firstToilet && ServerManager.serverManager.getFirstList.Count > 0)
            {
                fNumber.text = ServerManager.serverManager.getFirstList.First().Value;
                fCon = ServerManager.serverManager.getFirstList.First().Key;
                ServerManager.serverManager.firstToilet = false;
            }
            else if (ServerManager.serverManager.secondToilet && ServerManager.serverManager.getSecondList.Count > 0)
            {
                sNumber.text = ServerManager.serverManager.getSecondList.First().Value;
                sCon = ServerManager.serverManager.getSecondList.First().Key;
                ServerManager.serverManager.secondToilet = false;
            }
            else
            {
                if (ServerManager.serverManager.getFirstList.Count > 0)
                {
                    fCon = 0;
                    fNumber.text = null;
                }
                else if(ServerManager.serverManager.getSecondList.Count > 0)
                {
                    sCon = 0;
                    sNumber.text = null;
                }
            }

            return;
        }

        fCon = 0;
        fNumber.text = null;
        sCon = 0;
        sNumber.text = null;
    }

    public void CheckDisconnect(int connId)
    {
        if (fCon == connId)
        {
            fCon = 0;
            fNumber.text = null;
            ServerManager.serverManager.firstToilet = true;
        }
        else if (sCon == connId)
        {
            sCon = 0;
            sNumber.text = null;
            ServerManager.serverManager.secondToilet = true;
        }

        UpdateTurn();
    }
}
