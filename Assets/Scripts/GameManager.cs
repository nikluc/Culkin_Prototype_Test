using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References Card Spawning")]
    public GameObject gridLayout;
    public GameObject cardPrefab;

    [Header("Menu UI Refrence")]
    public TMP_InputField rowInput;
    public TMP_InputField columnInput;
    public GameObject mainmenu;
    public GameObject inGamemenu;
    public Button startButton;

    [Header("In-Game UI refrence")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public GameObject[] comboFill;

    [Header("Audio Clips")]
    public AudioClip correct;
    public AudioClip incorrect;
    public AudioClip win;


    public CardData[] uniqueCards;

    [Header("Game Properties")]
    public Vector2Int gridSize = Vector2Int.one * 2;
    public Vector2 defaultCellSize = new Vector2(100, 100);
    public Vector2 minCellSize = new Vector2(60, 60);
    public int multiplierThreshold = 3;

    private List<CardItem> _totalCardList = new List<CardItem>();
    private System.Random _rng = new System.Random();
    private CardItem _firstSelectedCard = null;
    private const string _SaveKey = "MemoryGameSave";
    private GridLayoutGroup _gridLayoutGroup;

    // Scoring fields
    private int _matchCount = 0;
    private int _multiplier = 1;
    private int _totalScore = 0;

    // Use this to calculate base score per match
    private int _baseScorePerMatch;
    private int _cardsFound= 0;

    void Start()
    {
        ClearCards();
        _gridLayoutGroup = gridLayout.GetComponent<GridLayoutGroup>();
        // Calculate base score depending on difficulty
        ResetScore();

        // PairSetup();
    }

    #region GameSetup
    public void StartGame()
    {
        ResetScore();
        mainmenu.SetActive(false);
        inGamemenu.SetActive(true);
        gridLayout.SetActive(true);
        gridSize = new Vector2Int(int.Parse(rowInput.text), int.Parse(columnInput.text));
        _gridLayoutGroup.constraintCount = gridSize.x;
        PairSetup();
        ApplyGridVisuals();
    }

    private void RestartGame()
    {
        _cardsFound = 0;
        ClearCards();
        PairSetup();
        ApplyGridVisuals();
    }

    // Clear existing cards from previous game setup

    public void ClearCards()
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }
        _totalCardList.Clear();
    }


    public void CheckGridSize()
    {
        if (rowInput.isFocused || rowInput.isFocused)
        {
            startButton.interactable = false;
            return;
        }
        try
        {
            gridSize = new Vector2Int(int.Parse(rowInput.text), int.Parse(columnInput.text));
            if ((gridSize.x * gridSize.y) % 2 != 0)
            {
                startButton.interactable = false;
            }
            else
            {
                startButton.interactable = true;
            }
        }
        catch
        {
            // Invalid input is given
            Debug.LogWarning("Grid size invalid. Please choose an even number of total cards.");
            startButton.interactable = false;
        }
    }

    private void PairSetup()
    {
        int totalCards = gridSize.x * gridSize.y;
        _baseScorePerMatch = (uniqueCards.Length + gridSize.x * gridSize.y) / 2;

        if (totalCards % 2 == 0)
        {
            int pairsNeeded = totalCards / 2;
            int uniqueCardsCount = uniqueCards.Length;
            int index = 0;

            // Clear any existing cards before setup
            ClearCards();

            for (int i = 0; i < pairsNeeded; i++)
            {
                CardData card = uniqueCards[index];
                _totalCardList.Add(CreateCard(card));
                _totalCardList.Add(CreateCard(card));

                index = (index + 1) % uniqueCardsCount;
            }

            Shuffle(_totalCardList);

            // Arrange cards in the hierarchy to match list order
            for (int i = 0; i < _totalCardList.Count; i++)
            {
                _totalCardList[i].transform.SetSiblingIndex(i);
            }
        }
        else
        {
            Debug.LogWarning("Grid size invalid. Please choose an even number of total cards.");
        }
    }

    private CardItem CreateCard(CardData data, bool loaded = false)
    {
        GameObject tempCard = Instantiate(cardPrefab, gridLayout.transform);
        tempCard.transform.localScale = Vector3.one;
        CardItem tempItem = tempCard.GetComponent<CardItem>();
        tempItem.cardData = data;
        tempItem.CardSetup(loaded);
        return tempItem;
    }


    #endregion

    #region Score Management

    public void ResetScore(bool allReset = true)
    {
        _matchCount = 0;
        _multiplier = 1;
        if (allReset)
        {
            _totalScore = 0;
            _cardsFound = 0;

        }

        for (int i = 0; i < comboFill.Length; i++)
        {
            comboFill[i].SetActive(false);
        }
        scoreText.text = _totalScore.ToString();
        multiplierText.text = "X(" + _multiplier.ToString() + ")";
    }

    public void OnMatchFound()
    {
        _matchCount++;

        for (int i = 0; i < comboFill.Length; i++)
        {
            comboFill[i].SetActive(i < _matchCount);
        }

        // Increase multiplier every 3 matches
        if (_matchCount % multiplierThreshold == 0)
        {
            _multiplier++;
            _matchCount = 0;
        }

        int scoreToAdd = _baseScorePerMatch * _multiplier;
        _totalScore += scoreToAdd;

        scoreText.text = _totalScore.ToString();
        multiplierText.text = "X("+_multiplier.ToString()+")";

        _cardsFound += 2;

        if(_cardsFound >= _totalCardList.Count)
        {
            Debug.Log("All cards found! Restarting game...");
            AudioManager.Instance.PlaySFX(win, 1);
            Invoke("RestartGame", 1f);
        }

        Debug.Log($"Match {_matchCount} found! Added {scoreToAdd} points. Total score: {_totalScore}, Multiplier: x{_multiplier}");
    }

    private IEnumerator HandlePair(CardItem card1, CardItem card2)
    {
        // Optional small delay before checking
        yield return new WaitForSeconds(0.3f);

        if (card1.cardData == card2.cardData)
        {
            // Matched pair vanish animation
            AudioManager.Instance.PlaySFX(correct, 1);
            card1.Vanish();
            card2.Vanish();
            OnMatchFound();

        }
        else
        {
            // Mismatch flip back animation
            AudioManager.Instance.PlaySFX(incorrect, 1);
            card1.FlipBack();
            card2.FlipBack();

            ResetScore(false);
        }

        // Wait for animations to finish
        yield return new WaitForSeconds(0.5f);
    }


    #endregion

    private void Update()
    {
        if (rowInput.text == "" || columnInput.text == "")
        {
            startButton.interactable = false;
            return;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void OnCardFlipped(CardItem flippedCard)
    {
        if (_firstSelectedCard == null)
        {
            _firstSelectedCard = flippedCard;
        }
        else
        {
            CardItem secondSelectedCard = flippedCard;

            // Launch pair comparison coroutine independently
            StartCoroutine(HandlePair(_firstSelectedCard, secondSelectedCard));

            // Reset for next pair
            _firstSelectedCard = null;
        }
    }

    #region Save & Load

    // Save current game state
    public void SaveGame()
    {
        SaveLoadData saveData = new SaveLoadData();
        saveData.gridWidth = gridSize.x;
        saveData.gridHeight = gridSize.y;

        int count = _totalCardList.Count;
        saveData.cardIDs = new string[count];
        saveData.matchedStates = new bool[count];
        saveData.flippedStates = new bool[count];

        for (int i = 0; i < count; i++)
        {
            CardItem card = _totalCardList[i];
            saveData.cardIDs[i] = card.cardData.cardName;
            saveData.matchedStates[i] = card.isVanished();  // inactive means matched
            saveData.flippedStates[i] = card.IsFlipped();
        }

        saveData.matchCount = _matchCount;
        saveData.multiplier = _multiplier;
        saveData.totalScore = _totalScore;
        saveData.noRows = gridSize.x;
        saveData.noCols = gridSize.y;

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(_SaveKey, json);
        PlayerPrefs.Save();

        mainmenu.SetActive(true);
        inGamemenu.SetActive(false);
        gridLayout.SetActive(false);

        Debug.Log("Game saved");
    }

    // Load saved game state
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(_SaveKey))
        {
            Debug.LogWarning("No saved game found");
            return;
        }

        mainmenu.SetActive(false);
        inGamemenu.SetActive(true);
        gridLayout.SetActive(true);


        string json = PlayerPrefs.GetString(_SaveKey);
        SaveLoadData saveData = JsonUtility.FromJson<SaveLoadData>(json);

        if (saveData == null)
        {
            Debug.LogError("Failed to parse saved data");
            return;
        }

        gridSize = new Vector2Int(saveData.gridWidth, saveData.gridHeight);

        ClearCards();

        _totalCardList = new List<CardItem>();

        int count = saveData.cardIDs.Length;
        for (int i = 0; i < count; i++)
        {
            CardData cardData = GetCardDataByUniqueID(saveData.cardIDs[i]);
            if (cardData == null)
            {
                Debug.LogError($"CardData not found for uniqueID {saveData.cardIDs[i]}");
                continue;
            }

            CardItem card = CreateCard(cardData,true);

            if (saveData.matchedStates[i])
            {
                card.Vanish();
            }
            else if (saveData.flippedStates[i])
            {
                card.CardFlip();
            }
            else
            {
                card.ResetCard(true);
            }

            _totalCardList.Add(card);
        }

        _matchCount = saveData.matchCount;
        _multiplier = saveData.multiplier;
        _totalScore = saveData.totalScore;
        _cardsFound = saveData.matchedStates.Length;

        scoreText.text = _totalScore.ToString();
        multiplierText.text = "X(" + _multiplier.ToString() + ")";

        gridSize = new Vector2Int(saveData.noRows, saveData.noCols);
        _gridLayoutGroup.constraintCount = gridSize.x;
        ApplyGridVisuals();

        Debug.Log("Game loaded");
    }


    #endregion


    #region Helpers
    private void ApplyGridVisuals()
    {
        if (_gridLayoutGroup == null) return;

        Vector2 cellSize = defaultCellSize;

        // If grid is larger than 8 in either axis, scale cell size down
        int maxAxis = Mathf.Max(gridSize.x, gridSize.y);
        if (maxAxis > 8)
        {
            float factor = 8f / maxAxis;
            cellSize *= factor;

            // Clamp so it never goes below a minimum
            cellSize.x = Mathf.Max(cellSize.x, minCellSize.x);
            cellSize.y = Mathf.Max(cellSize.y, minCellSize.y);
        }

        _gridLayoutGroup.cellSize = cellSize;
    }

    // Helper: Find CardData by uniqueID
    private CardData GetCardDataByUniqueID(string cardName)
    {
        foreach (var card in uniqueCards)
        {
            if (card.cardName == cardName)
                return card;
        }
        return null;
    }


    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    #endregion


}


[Serializable]
public class SaveLoadData
{
    public int gridWidth;
    public int gridHeight;
    public string[] cardIDs;
    public bool[] matchedStates;
    public bool[] flippedStates;

    public int matchCount;
    public int multiplier;
    public int totalScore;
    public int noRows;
    public int noCols;
}