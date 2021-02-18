using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] Sprite[] cardImages = new Sprite[0];
    

    [SyncVar(hook = nameof(ClientHandleCardIndexUpdated))]
    private int openCardIndex;

    private SpriteRenderer spriteRenderer;

    public static event Action clientTakecardButtonClicked;
    public static event Action clientDiscardButtonClicked;
    public static event Action<int> ClientNewCardOpened;

    public int getOpenCardIndex() {
        return openCardIndex;
    }

    private void Start()
    {
        Card.ServerCardSwappedWithDeck += handleServerCardSwappedWithDeck;
    }

    private void OnDestroy()
    {
        Card.ServerCardSwappedWithDeck -= handleServerCardSwappedWithDeck;
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

    [Command(ignoreAuthority =true)]
    public void cmdUpdateCardIndex(int newIndex)
    {
        print("cmdUpdateCardIndex , new index : " + newIndex);
        openCardIndex = newIndex;
    }

    #endregion

    #region Client

    private void handleServerCardSwappedWithDeck(int newIndex) {
        print("handleServerCardSwappedWithDeck , new Index : " + newIndex);
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
            
            print($"Player number {NetworkClient.connection.connectionId} is trying to change deck");
            updateCard();
            //if(NetworkClient.connection.connectionId == gamestate.returnTurnState())
            //  updateCard();
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
        //gamestate = FindObjectOfType<Gamestate>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
