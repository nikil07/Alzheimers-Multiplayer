using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class AlzNetworkManager : NetworkManager
{
    [SerializeField] GameObject handPrefab;
    [SerializeField] GameObject deckPrefab;
    [SerializeField] Transform deckSpawnPoint;
    GameObject deckInstance = null;

    public static event Action<int> ServerPlayerAdded;
    public static event Action<int> ServerPlayerRemoved;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        ServerPlayerAdded?.Invoke(conn.connectionId);
        GameObject handInstance = Instantiate(handPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(handInstance, conn);


        if (numPlayers == 1)
        {
            deckInstance = Instantiate(deckPrefab, deckSpawnPoint.position, deckSpawnPoint.rotation);
            NetworkServer.Spawn(deckInstance, conn);
            //NetworkServer.Spawn(deckInstance);
        }
        
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        ServerPlayerRemoved?.Invoke(conn.connectionId);
    }

}
