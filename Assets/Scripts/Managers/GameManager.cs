using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    private GridLayoutGroup layoutGrid;

    void Start()
    {
        
        GenerateCards();

       // AdjustGridLayout();
    }

    // Generate the cards with the sprites to the prefab with their id's stored.
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


    //Reveals the card.
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


    // Checks for the card matching.
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

    // Shuffle the cards.
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

    // Adjust the grid layout dynamically.
    void AdjustGridLayout()
    {
        GridLayoutGroup gridLayoutGroup = cardContainer.GetComponent<GridLayoutGroup>();
        RectTransform cc = cardContainer.GetComponent<RectTransform>();

        float spacing = gridLayoutGroup.spacing.x;
        float paddingHorizontal = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float paddingVertical = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        float totalWidth = cc.rect.width - paddingHorizontal - ((columns - 1) * spacing);
        float totalHeight = cc.rect.height - paddingVertical - ((rows - 1) * spacing);

        float cellWidth = totalWidth / columns;
        float cellHeight = totalHeight / rows;

        float cellSize = Mathf.Min(cellWidth, cellHeight); // make it square

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }


}
