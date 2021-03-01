using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AlzPlayer : NetworkBehaviour
{
    private int playerId;

    private int[] playerIds = { 1, 2, 3, 4 };
    //public SyncList<int> activePlayers = new SyncList<int>();

    [SyncVar]
    public List<int> activePlayers = new List<int>();

    [SyncVar]
    [SerializeField] private int serverWhoseTurn = 0;

    //[SerializeField] NetworkIdentity deckIdentity;

    private NetworkIdentity deckIdentity;
    private Gamestate gamestate;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        print("OnstartServer player");
        playerId = playerIds[getNumberOfplayers()];
        Gamestate.serverTurnEnded += handleServerTurnEnded;
        deckIdentity = FindObjectOfType<Deck>().GetComponent<NetworkIdentity>();
        gamestate = FindObjectOfType<Gamestate>();

        AlzNetworkManager.ServerPlayerAdded += handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved += handleServerPlayerRemoved;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Gamestate.serverTurnEnded -= handleServerTurnEnded;

        AlzNetworkManager.ServerPlayerAdded -= handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved -= handleServerPlayerRemoved;
    }

    private void handleServerTurnEnded() { 
        
    }

    public int getPlayerId() {
        return playerId;
    }

    [Command]
    private void cmdChangeAuthority() {
        //print($"active player ID {activePlayers[serverWhoseTurn]}");
        print($"active player ID {gamestate.activePlayers[serverWhoseTurn]}");
        print($"Network server connections {NetworkServer.connections}");
        NetworkServer.connections.TryGetValue(gamestate.activePlayers[serverWhoseTurn], out NetworkConnectionToClient networkConnectionToClient);
        cmdUpdateTurnIndex();
        deckIdentity.RemoveClientAuthority();
        deckIdentity.AssignClientAuthority(networkConnectionToClient.identity.connectionToClient);
        
    }

    [Command]
    private void cmdUpdateTurnIndex() {
        serverWhoseTurn = (serverWhoseTurn + 1) % (gamestate.activePlayers.Count);
    }

    [Command(ignoreAuthority = true)]
    private void cmdServerPlayerAdded(int playerId) {
        activePlayers.Add(playerId);
    }

    [Command(ignoreAuthority = true)]
    private void cmdServerPlayerRemoved(int playerId)
    {
        activePlayers.Remove(playerId);
    }

    private void handleServerPlayerAdded(int playerId) {
        //cmdServerPlayerAdded(playerId); 
    }

    private void handleServerPlayerRemoved(int playerId)
    {
       //cmdServerPlayerRemoved(playerId);
    }

    #endregion

    #region Client

    public void pickCard() {
        // call command on deck.
        Deck deck = FindObjectOfType <Deck>();
        deck.cmdUpdateCardIndex(0);
        cmdChangeAuthority();
    }

    #endregion

    private int getNumberOfplayers() {
        NetworkManager nm = FindObjectOfType<NetworkManager>();
        return nm.numPlayers;
    }
}
