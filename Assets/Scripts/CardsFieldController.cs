using UnityEngine;

public class CardsFieldController : MonoBehaviour
{
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
    // Start is called before the first frame update
    private void Awake()
    {
        CreateCardsInCardsField();
    }


}
