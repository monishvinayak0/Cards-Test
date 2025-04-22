using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardContainer;
    public List<Sprite> cardSprites;
    public int rows = 2, columns = 2;

    private List<CardController> cards = new();
    private CardController firstCard, secondCard;
    private bool canFlip = true;
    private int score = 0;

    void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        List<CardData> deck = new();
        int pairCount = (rows * columns) / 2;

        for (int i = 0; i < pairCount; i++)
        {
            var sprite = cardSprites[i];
            deck.Add(new CardData { id = i, frontSprite = sprite });
            deck.Add(new CardData { id = i, frontSprite = sprite });
        }

        Shuffle(deck);

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        cards.Clear();

        foreach (var data in deck)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            CardController card = cardObj.GetComponent<CardController>();
            card.Init(data, this);
            cards.Add(card);
        }
    }

    public bool CanFlipCard()
    {
        return canFlip && secondCard == null;
    }

    public void OnCardRevealed(CardController card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canFlip = false;
        yield return new WaitForSeconds(1f);

        if (firstCard.cardData.id == secondCard.cardData.id)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
            score += 10;
            AudioManager.Instance?.PlayMatch();
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
            score -= 2;
            AudioManager.Instance?.PlayMismatch();
        }

        firstCard = null;
        secondCard = null;
        canFlip = true;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
