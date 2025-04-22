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
        print("front image" + frontImage.name);
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

        

        float currentY = transform.localEulerAngles.y;
        print(currentY);

        if (!reachedHalf )
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, midRotation, Time.deltaTime * flipSpeed*100f);
            if (Quaternion.Angle(transform.rotation, midRotation) < 1f)
            {
                backImage.gameObject.SetActive(false);
                frontImage.gameObject.SetActive(true);
                print("flipped");
                reachedHalf = true;
            }
        }

        else
        {
            // Second half: go to 180°
            transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, Time.deltaTime * flipSpeed * 100f);

            if (Quaternion.Angle(transform.rotation, endRotation) < 1f)
            {
                transform.rotation = endRotation;
                isFlipping = false;
                isFlipped = true;
            }
        }
    }

    public void FlipBack()
    {
        StartCoroutine(FlipBackCoroutine());
    }

    private IEnumerator FlipBackCoroutine()
    {
        Quaternion back = Quaternion.Euler(0, 0, 0);
        reachedHalf = false;

        while (Quaternion.Angle(transform.rotation, back) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, back, Time.deltaTime * flipSpeed * 100f);

            float currentY = transform.localEulerAngles.y;

            if (!reachedHalf && (currentY >= 85f && currentY <= 95f))
            {
                frontImage.gameObject.SetActive(false);
                backImage.gameObject.SetActive(true);
                Debug.Log("Flipped back to show back image at Y = " + currentY);
                reachedHalf = true;
            }

            yield return null;
        }

        transform.rotation = back;
        isFlipped = false;
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
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        print("card resetted");
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}
