using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SerializeField] Sprite[] cardImages = new Sprite[0];
    [SerializeField] Sprite cardBack;
    [SerializeField] bool canBeSeen;

    [SyncVar(hook =nameof(ClientHandleCardIndexUpdated))]
    private int cardIndex;

    private SpriteRenderer spriteRenderer;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        Deck.clientDiscardButtonClicked += handleDiscardButtonClicked;
        Deck.clientTakecardButtonClicked += handleTakecardButtonClicked;
        init();
        spriteRenderer.sprite = cardBack;
        cardIndex = Random.Range(0, cardImages.Length);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Deck.clientDiscardButtonClicked -= handleDiscardButtonClicked;
        Deck.clientTakecardButtonClicked -= handleTakecardButtonClicked;
    }

    [Command]
    public void cmdUpdateCardIndex(int newIndex) {
        cardIndex = newIndex;
    }

    #endregion

    #region Client

    private void handleDiscardButtonClicked() {
        print("Discard button clicked on card");
    }

    private void handleTakecardButtonClicked() {
        print("Take button clicked on card");
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
            print("mouse ciciked");
            StartCoroutine(displayCardTemporarily());
        }
    }

    IEnumerator displayCardTemporarily()
    {
        spriteRenderer.sprite = cardImages[cardIndex];
        yield return new WaitForSeconds(3);
        spriteRenderer.sprite = cardBack;
    }

    private void ClientHandleCardIndexUpdated(int oldIndex, int newIndex) {
        init();
        //spriteRenderer.sprite = cardImages[newIndex];
    }

    [ContextMenu("Update card")]
    public void updateCard() {
        cmdUpdateCardIndex(Random.Range(0,cardImages.Length));
    }

    #endregion

    private void init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
