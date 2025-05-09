using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the core logic of the card matching game: scoring, flipping, layout, and UI.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Transform cardContainer;
    public List<Sprite> cardSprites;
    public int rows = 2, columns = 2;

    [Header("UI Elements")]
    public Text scoreText;
    public Text comboText;
    public Text turnsText;
    public Text highScoreText;
    public Image[] stars;
    public GameObject endCard;

    [Header("Level Settings")]
    public int currentLevel = 1;

    private List<CardController> cards = new();
    private CardController firstCard, secondCard;
    private bool canFlip = true;

    private int score = 0;
    private int combo = 0;
    private int turns = 0;
    private int highScore = 0;

    private int matchCount = 0;
    private int totalPairs;

    private string HighScoreKey => $"HighScore_Level_{currentLevel}";

    private void Start()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        AdjustGridLayout();
        GenerateCards();
        UpdateUI();
    }

    #region Card Generation & Shuffling

    /// <summary>
    /// Generates and shuffles cards for the current level.
    /// </summary>
    private void GenerateCards()
    {
        totalPairs = (rows * columns) / 2;
        if (cardSprites.Count < totalPairs)
        {
            Debug.LogError("Not enough sprites assigned for the number of pairs.");
            return;
        }

        List<CardData> deck = new();

        for (int i = 0; i < totalPairs; i++)
        {
            Sprite sprite = cardSprites[i];
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

    /// <summary>
    /// Shuffles a generic list.
    /// </summary>
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    #endregion

    #region Card Matching Logic

    public bool CanFlipCard() => canFlip && secondCard == null;

    public void OnCardRevealed(CardController card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            turns++;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        canFlip = false;
        yield return new WaitForSeconds(1f);

        if (firstCard.cardData.id == secondCard.cardData.id)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();

            combo++;
            int comboBonus = combo * 2;
            score += 10 + comboBonus;
            matchCount++;

            AudioManager.Instance?.PlayMatch();
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();

            combo = 0;
            score = Mathf.Max(score - 2, 0);

            AudioManager.Instance?.PlayMismatch();
        }

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
        }

        UpdateUI();
        CheckGameEnd();

        firstCard = null;
        secondCard = null;
        canFlip = true;
    }

    #endregion

    #region Game Completion & UI Updates

    private void CheckGameEnd()
    {
        if (matchCount == totalPairs)
        {
            ShowStars();
            Debug.Log("Game complete!");
        }
    }

    private void ShowStars()
    {
        endCard.SetActive(true);

        int perfectTurns = totalPairs;
        float ratio = (float)turns / perfectTurns;

        int starCount = 1;
        if (ratio <= 1.5f) starCount = 2;
        if (ratio <= 1f) starCount = 3;

        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = i < starCount;
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        comboText.text = combo > 1 ? $"Combo x{combo}" : "";
        turnsText.text = $"Turns: {turns}";
        highScoreText.text = $"High Score: {highScore}";
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        highScore = 0;
        UpdateUI();
    }

    #endregion

    #region Layout & Navigation

    /// <summary>
    /// Dynamically adjusts the card layout based on row/column count.
    /// </summary>
    private void AdjustGridLayout()
    {
        GridLayoutGroup gridLayoutGroup = cardContainer.GetComponent<GridLayoutGroup>();
        RectTransform cc = cardContainer.GetComponent<RectTransform>();

        float spacing = gridLayoutGroup.spacing.x;
        float paddingHorizontal = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float paddingVertical = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        float totalWidth = cc.rect.width - paddingHorizontal - (columns - 1) * spacing;
        float totalHeight = cc.rect.height - paddingVertical - (rows - 1) * spacing;

        float cellWidth = totalWidth / columns;
        float cellHeight = totalHeight / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
    }

    /// <summary>
    /// Loads the next scene based on index.
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    #endregion
}
