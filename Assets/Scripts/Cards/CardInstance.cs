using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity { Common, Rare, Epic, Legendary }

public class CardInstance : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _artworkImage;


    [System.Serializable]
    public class CardInstanceSave
    {
        public string cardId;
        public int wearLevel;
        public int rarityLevel;

        public float posX, posY;
    }

    public CardData Data { get; private set; }
    public Rarity Rarity { get; private set; }
    public int WearLevel { get; private set; }

    public void Initialize(CardData data, Rarity rarity, int wearLevel)
    {
        Data = data;
        Rarity = rarity;
        WearLevel = wearLevel;
        _nameText.text = Data.cardName;
        _descriptionText.text = Data.description;
        _artworkImage.sprite = Data.artwork;
    }


    public CardInstanceSave ToSaveData()
    {
        return new CardInstanceSave
        {
            cardId = Data.name,
            wearLevel = WearLevel,
            rarityLevel = (int)Rarity,
            posX = transform.position.x,
            posY = transform.position.y
        };
    }

    public void LoadFromSave(CardInstanceSave save, CardDatabase db)
    {
        Data = db.GetByName(save.cardId);

        if (Data == null)
        {
            Debug.LogWarning($"CardData with name {save.cardId} not found in database!");
            return;
        }
        WearLevel = save.wearLevel;
        Rarity = (Rarity)save.rarityLevel;
        transform.position = new Vector3(save.posX, save.posY, 0);
    }
}

