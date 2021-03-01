using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] Sprite[] cardImages = new Sprite[0];

    SyncList<int> activePlayers = new SyncList<int>();


    [SyncVar(hook = nameof(ClientHandleCardIndexUpdated))]
    private int openCardIndex;

    [SyncVar]
    private int whoseTurn;

    private SpriteRenderer spriteRenderer;

    public static event Action clientTakecardButtonClicked;
    public static event Action clientDiscardButtonClicked;
    public static event Action<int> ClientNewCardOpened;

    private Gamestate gamestate;
    private AlzPlayer player;
    private bool firstTurn = false;

    public int getOpenCardIndex() {
        return openCardIndex;
    }

    private void Start()
    {
        Card.ServerCardSwappedWithDeck += handleServerCardSwappedWithDeck;
        AlzNetworkManager.ServerPlayerAdded += handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved += handleServerPlayerRemoved;
        Gamestate.serverTurnEnded += handleServerTurnEnded;
        //player = NetworkClient.connection.identity.GetComponent<AlzPlayer>();]
    }

    private void OnDestroy()
    {
        AlzNetworkManager.ServerPlayerAdded -= handleServerPlayerAdded;
        AlzNetworkManager.ServerPlayerRemoved -= handleServerPlayerRemoved;
        Card.ServerCardSwappedWithDeck -= handleServerCardSwappedWithDeck;
        Gamestate.serverTurnEnded -= handleServerTurnEnded;
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        openCardIndex = UnityEngine.Random.Range(0, cardImages.Length);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
    }

    [Command]
    public void cmdUpdateCardIndex(int newIndex)
    {
        //print("cmdUpdateCardIndex , new index : " + newIndex);
        newIndex = UnityEngine.Random.Range(0, cardImages.Length);
        openCardIndex = newIndex;
    }

    [Command(ignoreAuthority = true)]
    public void cmdWhoseTurn()
    {
        
        whoseTurn = (whoseTurn + 1) % (activePlayers.Count);
        print("cmd whose turn in deck , : " + whoseTurn);
    }


    #endregion

    #region Client

    private void Update()
    {
        if(player == null)
            player = NetworkClient.connection.identity.GetComponent<AlzPlayer>();
    }

    private void handleServerTurnEnded() {
        cmdWhoseTurn();
    }

    private void handleServerPlayerAdded(int playerId)
    {
        activePlayers.Add(playerId);
    }

    private void handleServerPlayerRemoved(int playerId)
    {
        activePlayers.Remove(playerId);
    }


    private void handleServerCardSwappedWithDeck(int newIndex) {
        //print("handleServerCardSwappedWithDeck , new Index : " + newIndex);
        cmdUpdateCardIndex(newIndex);
    }

    private void ClientHandleCardIndexUpdated(int oldIndex, int newIndex)
    {
        init();
        spriteRenderer.sprite = cardImages[newIndex];
        ClientNewCardOpened?.Invoke(newIndex);
    }

    private void updateCard()
    {
        cmdUpdateCardIndex(UnityEngine.Random.Range(0, cardImages.Length));
        
    }

    void OnMouseOver()
    {
        // Needs collider on object
        //print("OnMouseOver");
        if (Input.GetMouseButtonDown(0))
        {
            if (!hasAuthority)
            print("has Autority " + hasAuthority);
            //setCardProperty();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            init();
            if (!firstTurn)
            {
                firstTurn = true;
                player.pickCard();
            }
            if (!hasAuthority)
                return;
            player.pickCard();
            /*if (gamestate.returnTurnState(whoseTurn))
            {
                updateCard();
            }
            else {
                print("Not your turn");
                return;
            }*/
            //updateCard();
           //print($"Player id {NetworkClient.connection.connectionId}");
           // print($"netindentiy id {netIdentity.connectionToClient.connectionId}");
            //if(NetworkClient.connection.connectionId == gamestate.returnTurnState())
            
        }
    }

    public void acceptButtonClicked() {
        print("accpet button clicked");
        clientTakecardButtonClicked?.Invoke();
    }

    public void discardButtonClicked() {
        print("discard button clicked");
        clientDiscardButtonClicked?.Invoke();
    }

    #endregion

    private void init()
    {
        gamestate = FindObjectOfType<Gamestate>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
}
