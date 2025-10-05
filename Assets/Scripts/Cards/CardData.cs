using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Cards/New Card")]
public class CardData : ScriptableObject
{
    [SerializeField] private string _cardName;
    public string CardName => _cardName;
    [SerializeField, TextArea] private string _description;
    public string Description => _description;
    [SerializeField] private Sprite _artwork;
    public Sprite Artwork => _artwork;
    [SerializeField] private GameObject _alternatePrefab;
    public GameObject AlternatePrefab => _alternatePrefab;
    [SerializeField] private int _number;
    public int Number => _number;

    public void DEBUGSetNumber(int newNumber)
    {
        _number = newNumber;
    }
}
