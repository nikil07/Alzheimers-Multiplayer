using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Gamestate : NetworkBehaviour
{
    //[SyncVar]
    //private List<int> activePlayers = new List<int>();

    [SerializeField]SyncList<int> activePlayers = new SyncList<int>();

    [SyncVar]
    private int whoseTurn = 0;

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
    private void cmdUpdateWhoseTurn(int whoseTurn)
    {
        this.whoseTurn = whoseTurn;
    }

    public int returnTurnState() {
        //print("active player count "+ activePlayers.Count);
        print("whose turn " + whoseTurn);
        //whoseTurn = (whoseTurn + 1) % activePlayers.Count;
        cmdUpdateWhoseTurn((whoseTurn + 1) % activePlayers.Count);
        return activePlayers[whoseTurn];
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
