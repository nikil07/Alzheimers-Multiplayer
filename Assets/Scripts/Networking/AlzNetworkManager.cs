using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AlzNetworkManager : NetworkManager
{
    [SerializeField] GameObject handPrefab;
    [SerializeField] GameObject deckPrefab;
    [SerializeField] Transform deckSpawnPoint;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject handInstance = Instantiate(handPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(handInstance, conn);

        if (numPlayers == 1) {
            GameObject deckInstance = Instantiate(deckPrefab, deckSpawnPoint.position, deckSpawnPoint.rotation);
            NetworkServer.Spawn(deckInstance, conn);
        }
    }

}
