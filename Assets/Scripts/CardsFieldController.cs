using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsFieldController : MonoBehaviour
{


    #region UI Variables
    [SerializeField] TextMeshProUGUI totalGameScoreText, totalGuessesText, correctGuessesText, timerText, gameConditionText;
    float gameTimeLeftToEnd = 10f;
    float gameHalfTimeLeftToEnd;
    int totalGameScore = 0;
    #endregion

    #region Game Sounds
    [SerializeField] AudioSource cardFlipAudioSource;
    [SerializeField] AudioSource cardFlipBackAudioSource;
    [SerializeField] AudioSource cardPairMatchAudioSource;
    [SerializeField] AudioSource cardPairMissMatchAudioSource;
    [SerializeField] AudioSource gameOverAudioSource;
    #endregion


    bool isGameEnds = false;
    int totalPairs;

    [SerializeField] private float timeToChooseSecondCard = 2f;





    #region card variables
    [SerializeField] private Sprite CardBackImage;
    [SerializeField] private List<Sprite> allpossibleCardImages;
    [SerializeField] private List<Sprite> allpossiblecardImagesChosen = new();
    [SerializeField] private List<Button> CardsButtons = new();
    [SerializeField] private Transform CardsField;

    [SerializeField] private GameObject CardPrefab;
    readonly int numberOfCards = 6;
    private bool IsFirstCardGuessed;
    private bool IsSecondCardGuessed;
    private int totalGameGuesses;
    private int countCorrectGuesses = 0;
    private int firstCardIndex;
    private int secondCardIndex;
    string spritesPath = "Images/Sprites";

    private string firstGuessCardName, secondGuessCardName;

    #endregion

    private void CreateCardsInCardsField()
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            //create a card  
            GameObject card = Instantiate(CardPrefab);
            // To give the card a name as a game object in hierarchy
            card.name = "" + i;
            //To Attach the card to the CardsField
            card.transform.SetParent(CardsField, false);

        }

    }

    #region GetCards and its indices
    private void GetCards()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        for (int i = 0; i < cards.Length; i++)
        {
            CardsButtons.Add(cards[i].GetComponent<Button>());
            CardsButtons[i].image.sprite = CardBackImage;
        }
    }
    private int GetClickedCardIndex()
    {
        string clickedName = UnityEngine.EventSystems.EventSystem
            .current
            .currentSelectedGameObject
            .name;

        return int.Parse(clickedName);
    }
    #endregion

    private void PrepareCardsMatchingPairs()
    {
        int totalCardSlots = CardsButtons.Count;
        int totalPairsNeeded = totalCardSlots / 2;
        int cardSpritePointer = 0;
        //loop through the total number of card slots

        for (int i = 0; i < totalCardSlots; i++)
        {
            //to make the loop start from the beginnning of the sprites array again 
            if (cardSpritePointer == totalPairsNeeded)
            {
                cardSpritePointer = 0;
            }
            // cards that will be matched in the game not all the card sprites
            allpossiblecardImagesChosen.Add(allpossibleCardImages[cardSpritePointer]);
            cardSpritePointer++;

        }
    }
    // Click on a card
    private void ClickOnACard()
    {
        foreach (Button cardButton in CardsButtons)
        {
            //when a card is clicked on it will call the function of OnCardClick
            cardButton.onClick.AddListener(() => OnCardClick());
        }
    }

    private void OnCardClick()
    {
        int cardIndex = GetClickedCardIndex();

        if (!IsFirstCardGuessed)
        {
            HandleFirstCardGuess(cardIndex);
        }
        else if (!IsSecondCardGuessed)
        {
            HandleSecondCardGuess(cardIndex);
        }
    }




    #region Card Guess
    private void HandleFirstCardGuess(int cardIndex)
    {
        IsFirstCardGuessed = true;
        firstCardIndex = cardIndex;

        RevealCard(cardIndex);
        // get the name of a certain sprite
        firstGuessCardName = allpossiblecardImagesChosen[cardIndex].name;

        DisableCardClicks(cardIndex);


        Invoke(nameof(AutoFlipFirstCard), timeToChooseSecondCard);
    }
    private void HandleSecondCardGuess(int cardIndex)
    {
        IsSecondCardGuessed = true;
        secondCardIndex = cardIndex;

        RevealCard(cardIndex);

        secondGuessCardName = allpossiblecardImagesChosen[cardIndex].name;
        CheckIfCardsMatch();
        DisableCardClicks(cardIndex);

    }

    #endregion
    private void RevealCard(int cardIndex)
    {
        CardsButtons[cardIndex].image.sprite =
            allpossiblecardImagesChosen[cardIndex];

        cardFlipAudioSource.Play();


    }



    private void CheckIfCardsMatch()
    {
        totalGameGuesses++;

        if (firstGuessCardName == secondGuessCardName)
        {
            Debug.Log("Puzzle Match");
            cardPairMatchAudioSource.Play();
            TotalGameScore();
            CorrectGuesses();
            ResetGuesses();
        }
        else
        {
            Debug.Log("Puzzle don't Match");
            cardPairMissMatchAudioSource.Play();
            Invoke(nameof(FlipCardsBackToItsOriginalPosition), 1.5f);
        }
        TotalGameGuesses();

    }

    private void TotalGameGuesses()
    {
        totalGuessesText.text = "Total Guesses = " + totalGameGuesses;
    }

    private void CorrectGuesses()
    {
        countCorrectGuesses++;
        correctGuessesText.text = "Correct Guesses = " + countCorrectGuesses;

        if (countCorrectGuesses == totalPairs && gameTimeLeftToEnd > 0)
        {
            GameWin();
        }
    }


    private void TotalGameScore()
    {
        totalGameScore += 100;
        totalGameScoreText.text = "Score = " + totalGameScore;
    }

    private void DisableCardClicks(int cardIndex)
    {
        CardsButtons[cardIndex].interactable = false;
    }


    #region CardFlipping
    private void FlipCardsBackToItsOriginalPosition()
    {
        FlipFirstCardBack();
        FlipSecondCardBack();
        EnableCardsClicksOn();
        ResetGuesses();
    }
    private void FlipFirstCardBack()
    {
        CardsButtons[firstCardIndex].image.sprite = CardBackImage;
        cardFlipBackAudioSource.Play();

    }
    private void FlipSecondCardBack()
    {
        CardsButtons[secondCardIndex].image.sprite = CardBackImage;
        cardFlipBackAudioSource.Play();
    }
    private void AutoFlipFirstCard()
    {
        // Only flip if player didn't choose second card
        if (IsFirstCardGuessed && !IsSecondCardGuessed)
        {
            FlipFirstCardBack();
            EnableFirstCardClicks();
            ResetGuesses();
        }
    }

    #endregion

    #region EnableCardClicks

    private void EnableCardsClicksOn()
    {
        EnableFirstCardClicks();
        EnableSecondCardClicks();
    }

    private void EnableFirstCardClicks()
    {
        CardsButtons[firstCardIndex].interactable = true;
    }

    private void EnableSecondCardClicks()
    {
        CardsButtons[secondCardIndex].interactable = true;
    }
    #endregion



    #region Game State

    private void GameOver()
    {
        if (isGameEnds) return;

        isGameEnds = true;
        PlayerConditionInGame(0);
        DisableAllCards();
        gameOverAudioSource.Play();
        Debug.Log("Game Over");
    }

    private void GameWin()
    {
        if (isGameEnds) return;

        isGameEnds = true;
        PlayerConditionInGame(1);
        DisableAllCards();
        Debug.Log("Player Wins!");
    }

    private void PlayerConditionInGame(int playerCondition)
    {
        gameConditionText.gameObject.SetActive(true);
        gameConditionText.text = playerCondition == 1 ? "You Win" : "You Lose";
    }

    private void DisableAllCards()
    {
        foreach (Button card in CardsButtons)
        {
            card.interactable = false;
        }
    }

    #endregion


    // Start is called before the first frame update
    private void Awake()
    {
        LoadAllPossibleCardsSprites();
        CreateCardsInCardsField();
    }
    void Start()
    {
        totalPairs = numberOfCards / 2;
        gameHalfTimeLeftToEnd = gameTimeLeftToEnd / 2;
        timerText.text = "Time = " + gameTimeLeftToEnd + " s";
        GetCards();
        PrepareCardsMatchingPairs();
        ClickOnACard();
    }

    void Update()
    {
        DecrementGameTimer();
    }

    private void DecrementGameTimer()
    {
        if (isGameEnds) return;

        if (gameTimeLeftToEnd > 0)
        {
            gameTimeLeftToEnd -= Time.deltaTime;

            if (gameTimeLeftToEnd < gameHalfTimeLeftToEnd)
            {
                timerText.color = Color.red;
            }

            if (gameTimeLeftToEnd <= 0 && countCorrectGuesses < totalPairs)
            {
                gameTimeLeftToEnd = 0;
                GameOver();
            }
        }

        timerText.text = "Time = " + gameTimeLeftToEnd.ToString("0") + " s";
    }



    private void LoadAllPossibleCardsSprites()
    {
        allpossibleCardImages = new List<Sprite>(
        Resources.LoadAll<Sprite>(spritesPath)
    );
    }
    private void ResetGuesses()
    {
        IsFirstCardGuessed = false;
        IsSecondCardGuessed = false;
    }

}
