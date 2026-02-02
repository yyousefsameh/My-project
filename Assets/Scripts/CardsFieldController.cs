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


    private void CreateCardsInCardsField()
    {
        for (int i = 0; i < 6; i++)
        {
            //create a card  
            GameObject card = Instantiate(CardPrefab);
            // To give the card a name as a game object in hierarchy
            card.name = "Card" + i;
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

    //Click on a card
    // private void ClickOnACard()
    // {
    //     foreach (Button cardButton in CardsButtons)
    //     {
    //         //when a card is clicked on it will call the function of OnCardClick
    //         cardButton.onClick.AddListener(() => OnCardClick());
    //     }
    // }

    // private void OnCardClick()
    // {



    // }


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
        // ClickOnACard();
    }



}
