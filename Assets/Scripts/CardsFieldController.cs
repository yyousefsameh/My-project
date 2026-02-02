using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsFieldController : MonoBehaviour
{
    [SerializeField] private Sprite CardBackImage;
    [SerializeField] private Sprite[] allpossibleCardImages;
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

    private string firstGuessCard, secondGuessCard;

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


    private void GetCards()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        for (int i = 0; i < cards.Length; i++)
        {
            CardsButtons.Add(cards[i].GetComponent<Button>());
            CardsButtons[i].image.sprite = CardBackImage;
        }
    }

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
        // 1. Get the name of the EXACT button that was just clicked
        string clickedName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        int cardIndex = int.Parse(clickedName);

        if (!IsFirstCardGuessed)
        {
            IsFirstCardGuessed = true;
            firstCardIndex = cardIndex;
            // Change the sprite of the button to the on of the sprites in the array of chosen sprite cards
            CardsButtons[firstCardIndex].image.sprite = allpossiblecardImagesChosen[firstCardIndex];

            // Polish tip: Disable the button so the player can't click it again as their second guess
            // making the button with alpha
            CardsButtons[firstCardIndex].interactable = false;
        }
        else if (!IsSecondCardGuessed)
        {
            IsSecondCardGuessed = true;
            secondCardIndex = cardIndex;
            CardsButtons[secondCardIndex].image.sprite = allpossiblecardImagesChosen[secondCardIndex];

            CardsButtons[secondCardIndex].interactable = false;


            // Start the check to see if they match!

        }
    }


    // Start is called before the first frame update
    private void Awake()
    {
        LoadAllPossibleCardsSprites();
        CreateCardsInCardsField();
    }

    private void LoadAllPossibleCardsSprites()
    {
        allpossibleCardImages = Resources.LoadAll<Sprite>("Images/Sprites");
    }

    void Start()
    {
        GetCards();
        PrepareCardsMatchingPairs();
        ClickOnACard();
    }



}
