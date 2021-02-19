using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class Card : NetworkBehaviour
{
    [SerializeField] Sprite[] cardImages = new Sprite[0];
    [SerializeField] Sprite cardBack;
    [SerializeField] bool canBeSeen;

    [SyncVar(hook =nameof(ClientHandleCardIndexUpdated))]
    private int cardIndex;

    private SpriteRenderer spriteRenderer;
    private bool isInCardPickState;
    private Animator animator;
    private NetworkAnimator networkAnimator;

    public static event Action<int> ServerCardSwappedWithDeck;

    private void Start()
    {
        Deck.clientDiscardButtonClicked += handleDiscardButtonClicked;
        Deck.clientTakecardButtonClicked += handleTakecardButtonClicked;
        ServerCardSwappedWithDeck += handleServerCardSwappedWithDeck;
    }

    private void OnDestroy()
    {
        Deck.clientDiscardButtonClicked -= handleDiscardButtonClicked;
        Deck.clientTakecardButtonClicked -= handleTakecardButtonClicked;
        ServerCardSwappedWithDeck = handleServerCardSwappedWithDeck;
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        init();
        cardIndex = UnityEngine.Random.Range(0, cardImages.Length);
        //spriteRenderer.sprite = cardBack;
        spriteRenderer.sprite = cardImages[cardIndex];
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
       
    }

    [Command]
    public void cmdUpdateCardIndex(int newIndex) {
        cardIndex = newIndex;
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void handleDiscardButtonClicked() {
        print("Discard button clicked on card");
    }

    private void handleTakecardButtonClicked() {
        print("Take button clicked on card");
        animator.SetBool("isShake", true);
        networkAnimator.SetTrigger("startShake");
    }

    void OnMouseOver()
    {
        
        // Needs collider on object
        //print("OnMouseOver");
        if (Input.GetMouseButtonDown(0))
        {
            //setCardProperty();
            
        }
        else if (Input.GetMouseButtonDown(1))
        {
            
            //StartCoroutine(displayCardTemporarily());
        }
    }

    private void handleCardClicked() {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider)
        {
            print("card cicked");
            cmdUpdateCardIndex(getOpenDeckCardIndex());
        }
    }

    private int getOpenDeckCardIndex() {

        return -1;
    }

    IEnumerator displayCardTemporarily()
    {
        spriteRenderer.sprite = cardImages[cardIndex];
        yield return new WaitForSeconds(3);
        spriteRenderer.sprite = cardBack;
    }

    private void ClientHandleCardIndexUpdated(int oldIndex, int newIndex) {
        init();
        spriteRenderer.sprite = cardImages[newIndex];
    }

    private void handleServerCardSwappedWithDeck(int index) {
        animator.SetBool("isShake", false);
        networkAnimator.SetTrigger("startShake");
    }

    public void swapCardAndDeck(int newIndex) {
        
        ServerCardSwappedWithDeck?.Invoke(cardIndex);
        cmdUpdateCardIndex(newIndex);
        networkAnimator.SetTrigger("startShake");
        animator.SetBool("isShake", false);
    }

    [ContextMenu("click")]
    public void chageCard() {
        cmdUpdateCardIndex(UnityEngine.Random.Range(0,cardImages.Length));
    }

    #endregion

    private void init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }
}
