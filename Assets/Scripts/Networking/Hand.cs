using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hand : NetworkBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform[] cardPositions = new Transform[0];

    #region Server

    public override void OnStartServer()
    {
        foreach (Transform cardPosition in cardPositions) {
            GameObject cardPrefabInstance = Instantiate(cardPrefab, cardPosition.position, cardPosition.rotation);
            NetworkServer.Spawn(cardPrefabInstance, connectionToClient);
        }
    }

    #endregion

    #region Client

    #endregion
}
