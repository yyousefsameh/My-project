using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsFieldController : MonoBehaviour
{
    [SerializeField] private Sprite CardBackImage;
    [SerializeField] private List<Sprite> allpossibleCardImages;
    [SerializeField] private List<Sprite> allpossiblecardImagesChosen = new();
    [SerializeField] private List<Button> CardsButtons = new();
    [SerializeField] private Transform CardsField;

    [SerializeField] private GameObject CardPrefab;

    [SerializeField] private float timeToChooseSecondCard = 2f;

    readonly int numberOfCards = 6;
    private bool IsFirstCardGuessed;
    private bool IsSecondCardGuessed;
    private int totalGameGuesses;
    private int countCorrectGuesses = 0;
    private int firstCardIndex;
    private int secondCardIndex;
    string spritesPath = "Images/Sprites";

    private string firstGuessCardName, secondGuessCardName;

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
    }


    private void CheckIfCardsMatch()
    {
        totalGameGuesses++;

        if (firstGuessCardName == secondGuessCardName)
        {
            countCorrectGuesses++;

            Debug.Log("Puzzle Match");

            ResetGuesses();
        }
        else
        {
            Debug.Log("Puzzle don't Match");
            Invoke(nameof(FlipCardsBackToItsOriginalPosition), 1f);
        }
    }

    private void DisableCardClicks(int cardIndex)
    {
        CardsButtons[cardIndex].interactable = false;
    }


    #region CardFlipping
    private void FlipCardsBackToItsOriginalPosition()
    {
        FlipFirstCard();
        FlipSecondCard();
        EnableCardsClicksOn();
        ResetGuesses();
    }
    private void FlipFirstCard()
    {
        CardsButtons[firstCardIndex].image.sprite = CardBackImage;
    }
    private void FlipSecondCard()
    {
        CardsButtons[secondCardIndex].image.sprite = CardBackImage;
    }
    private void AutoFlipFirstCard()
    {
        // Only flip if player didn't choose second card
        if (IsFirstCardGuessed && !IsSecondCardGuessed)
        {
            FlipFirstCard();
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






    // Start is called before the first frame update
    private void Awake()
    {
        LoadAllPossibleCardsSprites();
        CreateCardsInCardsField();
    }
    void Start()
    {
        GetCards();
        PrepareCardsMatchingPairs();
        ClickOnACard();
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
