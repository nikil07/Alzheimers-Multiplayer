using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardClickDetector : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;
    private int openCardIndex;

    enum Gamestate { IDLE, PICKUPMODE, DISCARDED };
    Gamestate gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = Gamestate.IDLE;
        mainCamera = Camera.main;
        Deck.ClientNewCardOpened += handleNewDeckCardOpened;
    }

    private void OnDestroy()
    {
        Deck.ClientNewCardOpened -= handleNewDeckCardOpened;
    }

    // Update is called once per frame
    void Update()
    {
        checkForHit();
    }

    private void handleNewDeckCardOpened(int openCardIndex) {
        this.openCardIndex = openCardIndex;
    }

    private void checkForHit() {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);
        if (!hit)
            return;
        if (!hit.collider.TryGetComponent<Card>(out Card card)) { return; }
        if (!card.hasAuthority) { return; }
        print("hitted a card, opencard: " + openCardIndex);
        card.swapCardAndDeck(openCardIndex);
    }
}
