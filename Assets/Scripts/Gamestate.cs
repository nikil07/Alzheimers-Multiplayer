using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Gamestate : MonoBehaviour
{
    private List<int> activePlayers = new List<int>();

    private int whoseTurn = -1;

    // Start is called before the first frame update
    void Start()
    {
        AlzNetworkManager.ServerPlayerAdded += handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved += handleServerPlayerRemoved;
    }

    private void OnDestroy()
    {
        AlzNetworkManager.ServerPlayerAdded -= handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved -= handleServerPlayerRemoved;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int returnTurnState() {
        print(activePlayers.ToString());
        whoseTurn = (whoseTurn + 1) % activePlayers.Count;
        return activePlayers[whoseTurn];
    }

    public void clearPlayersList() {
        activePlayers.Clear();
    }

    private void handleServerPlayerAdded(int playerId) {
        activePlayers.Add(playerId);
        print(activePlayers.Count + "," +  activePlayers.ToString());
    }

    private void handleServerPlayerRemoved(int playerId)
    {
        activePlayers.Remove(playerId);
        print(activePlayers.Count + "," + activePlayers.ToString());
    }


}
