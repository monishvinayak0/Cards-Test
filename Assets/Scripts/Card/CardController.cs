using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardController : MonoBehaviour
{
    public Image frontImage;
    public Image backImage;
    public CardData cardData;

    private GameManager gameManager;

    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;
    private float flipSpeed = 5f;

    private Quaternion startRotation;
    private Quaternion midRotation;
    private Quaternion endRotation;
    private bool reachedHalf = false;

    public void Init(CardData data, GameManager manager)
    {
        cardData = data;
        gameManager = manager;
        frontImage.sprite = cardData.frontSprite;

        // Force card to default state
        ResetCard();
    }

    public void OnCardClicked()
    {
        if (isMatched || isFlipping || gameManager == null || !gameManager.CanFlipCard()) return;

        AudioManager.Instance?.PlayFlip();
        gameManager.OnCardRevealed(this);
        StartFlip();
    }

    void StartFlip()
    {
        isFlipping = true;
        reachedHalf = false;

        startRotation = Quaternion.Euler(0, 0, 0);
        midRotation = Quaternion.Euler(0, 90, 0);
        endRotation = Quaternion.Euler(0, 180, 0);

        transform.localRotation = startRotation;
    }

    void Update()
    {
        if (!isFlipping) return;

        if (!reachedHalf)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, midRotation, Time.deltaTime * flipSpeed * 100f);
            if (Quaternion.Angle(transform.localRotation, midRotation) < 1f)
            {
                // Halfway: show front
                backImage.gameObject.SetActive(false);
                frontImage.gameObject.SetActive(true);
                reachedHalf = true;
            }
        }
        else
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, endRotation, Time.deltaTime * flipSpeed * 100f);
            if (Quaternion.Angle(transform.localRotation, endRotation) < 1f)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                isFlipping = false;
                isFlipped = true;

                // Final visual sync
                SyncImagesToRotation();
            }
        }
    }

    public void FlipBack()
    {
        StartCoroutine(FlipBackCoroutine());
    }

    private IEnumerator FlipBackCoroutine()
    {
        isFlipping = true;
        reachedHalf = false;

        startRotation = Quaternion.Euler(0, 180, 0);
        midRotation = Quaternion.Euler(0, 90, 0);
        endRotation = Quaternion.Euler(0, 0, 0);

        transform.localRotation = startRotation;

        // Phase 1: 180 → 90
        while (!reachedHalf)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, midRotation, Time.deltaTime * flipSpeed * 100f);

            if (Quaternion.Angle(transform.localRotation, midRotation) < 1f)
            {
                frontImage.gameObject.SetActive(false);
                backImage.gameObject.SetActive(true);
                reachedHalf = true;
            }

            yield return null;
        }

        // Phase 2: 90 → 0
        while (Quaternion.Angle(transform.localRotation, endRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, endRotation, Time.deltaTime * flipSpeed * 100f);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0, 0, 0);
        isFlipped = false;
        isFlipping = false;

        // Final visual sync
        SyncImagesToRotation();
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        isFlipping = false;

        // Reset transform cleanly
        transform.localRotation = Quaternion.identity;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        // Force correct image visibility
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public bool IsMatched()
    {
        return isMatched;
    }

    
    // Ensures the image visibility matches the card's Y-rotation angle.
    
    private void SyncImagesToRotation()
    {
        float y = transform.localEulerAngles.y;

        if (y > 90f && y < 270f)
        {
            // Card is facing forward
            frontImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
            isFlipped = true;
        }
        else
        {
            // Card is facing back
            frontImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);
            isFlipped = false;
        }
    }
}
