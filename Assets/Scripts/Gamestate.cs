using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Gamestate : NetworkBehaviour
{
    //[SyncVar]
    //private List<int> activePlayers = new List<int>();

    public static event Action serverTurnEnded;

    public SyncList<int> activePlayers = new SyncList<int>();

    [SyncVar]
    [SerializeField]private int serverWhoseTurn = 0;

    // Start is called before the first frame update
    void Start()
    {
        print("Strt");
        AlzNetworkManager.ServerPlayerAdded += handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved += handleServerPlayerRemoved;
    }

    private void OnDestroy()
    {
        AlzNetworkManager.ServerPlayerAdded -= handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved -= handleServerPlayerRemoved;
    }

    [Command(ignoreAuthority = true)]
    private void cmdAddPlayer(int playerId) {
        activePlayers.Add(playerId);
        printList();
    }

    [Command(ignoreAuthority = true)]
    private void cmdRemovePlayer(int playerId)
    {
        activePlayers.Remove(playerId);
        printList();
    }

    [Command(ignoreAuthority = true)]
    private void cmdUpdateWhoseTurn()
    {
        //print("active player list count " + activePlayers.Count);
        serverWhoseTurn = (serverWhoseTurn + 1) % (activePlayers.Count);
        print($"Server whose turn {serverWhoseTurn}");
    }

    public bool returnTurnState(int whoseTurn) {
        //print("active player count "+ activePlayers.Count);
        print("whose turn " + whoseTurn);
        print("Server whose turn " +  serverWhoseTurn);
        //whoseTurn = (whoseTurn + 1) % activePlayers.Count;
        return serverWhoseTurn == whoseTurn;
    }

    void OnMouseOver()
    {
        // Needs collider on object
        //print("OnMouseOver");
        if (Input.GetMouseButtonDown(0))
        {
            print("END TURN");
            serverTurnEnded?.Invoke();
            cmdUpdateWhoseTurn();

        }
    }

    public void clearPlayersList() {
        activePlayers.Clear();
    }

    private void handleServerPlayerAdded(int playerId) {
        cmdAddPlayer(playerId);
    }

    private void handleServerPlayerRemoved(int playerId)
    {
        cmdRemovePlayer(playerId);
    }

    private void printList() {
        string listString = "";
        foreach (int playerId in activePlayers) {
            listString += playerId + " , ";
        }
        print("Player list " + listString);
    }

}
