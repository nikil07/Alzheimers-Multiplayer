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
    Gamestate gamestate;

    public static event Action clientTakecardButtonClicked;
    public static event Action clientDiscardButtonClicked;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        openCardIndex = UnityEngine.Random.Range(0, cardImages.Length);
    }

    [Command(ignoreAuthority =true)]
    public void cmdUpdateCardIndex(int newIndex)
    {
        openCardIndex = newIndex;
    }

    #endregion

    #region Client

    private void ClientHandleCardIndexUpdated(int oldIndex, int newIndex)
    {
        init();
        spriteRenderer.sprite = cardImages[newIndex];
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
            if(NetworkClient.connection.connectionId == gamestate.returnTurnState())
                updateCard();
        }
    }

    public void acceptButtonClicked() {
        clientTakecardButtonClicked?.Invoke();
    }

    public void discardButtonClicked() {
        clientDiscardButtonClicked?.Invoke();
    }

    #endregion

    private void init()
    {
        gamestate = FindObjectOfType<Gamestate>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
