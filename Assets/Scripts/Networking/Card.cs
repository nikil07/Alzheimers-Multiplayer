using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SerializeField] Sprite[] cardImages = new Sprite[0];

    [SyncVar(hook =nameof(ClientHandleCardIndexUpdated))]
    private int cardIndex;

    private SpriteRenderer spriteRenderer;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        cardIndex = Random.Range(0, cardImages.Length);
    }

    [Command]
    public void cmdUpdateCardIndex(int newIndex) {
        cardIndex = newIndex;
    }

    #endregion

    #region Client

    private void ClientHandleCardIndexUpdated(int oldIndex, int newIndex) {
        init();
        spriteRenderer.sprite = cardImages[newIndex];
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
