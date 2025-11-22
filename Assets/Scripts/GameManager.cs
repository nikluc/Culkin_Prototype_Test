using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gridLayout;
    public GameObject cardPrefab;

    public CardData[] uniqueCards;

    [Header("Game Properties")]
    public Vector2Int gridSize = Vector2Int.one * 2;

    private List<CardItem> _totalCardList = new List<CardItem>();
    private System.Random rng = new System.Random();
    private CardItem firstSelectedCard = null;

    void Start()
    {
        ClearCards();
        PairSetup();
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

    private void PairSetup()
    {
        int totalCards = gridSize.x * gridSize.y;

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

    public void OnCardFlipped(CardItem flippedCard)
    {
        if (firstSelectedCard == null)
        {
            firstSelectedCard = flippedCard;
        }
        else
        {
            CardItem secondSelectedCard = flippedCard;

            // Launch pair comparison coroutine independently
            StartCoroutine(HandlePair(firstSelectedCard, secondSelectedCard));

            // Reset for next pair
            firstSelectedCard = null;
        }
    }

    private IEnumerator HandlePair(CardItem card1, CardItem card2)
    {
        // Optional small delay before checking
        yield return new WaitForSeconds(0.3f);

        if (card1.cardData == card2.cardData)
        {
            // Matched pair vanish animation
            card1.Vanish();
            card2.Vanish();
        }
        else
        {
            // Mismatch flip back animation
            card1.FlipBack();
            card2.FlipBack();
        }

        // Wait for animations to finish
        yield return new WaitForSeconds(0.5f);
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

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    // Save current game state
    public void SaveGame()
    {

        Debug.Log("Game saved");
    }

    // Load saved game state
    public void LoadGame()
    {
        Debug.Log("Game loaded");
    }


}
