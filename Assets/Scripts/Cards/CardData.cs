using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Cards/New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite artwork;

    public bool availableForPlayer;
    public bool availableForEnemy;
}
