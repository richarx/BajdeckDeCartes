using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    private static float BETWEEN_CARDS = 0.01f;
    public static CardManager Instance { get; private set; } = null;
    private static CardManager _instance = null;
    public static UnityEvent instanceUpdated = new UnityEvent();
    [SerializeField] private CardGeneratorConfig _generatorConfig;
    private List<CardInstance> _cards = new List<CardInstance>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        Save save = Save.Load<Save>();
        foreach (var cardPos in save.cards)
        {
            Debug.Log($"Loading card from code: {cardPos.code}");
            CardInstance cardInstance = _generatorConfig.GenerateCard(cardPos.code, save.GetKey()).GetComponent<CardInstance>();
            if (cardInstance != null)
            {
                _cards.Add(cardInstance);
                cardInstance.transform.SetParent(transform);
                cardInstance.transform.position = cardPos.position;
                cardInstance.transform.eulerAngles = cardPos.rotation;
            }
        }
    }

    void OnApplicationQuit()
    {
        _instance = null;
    }

    private void SaveState()
    {
        Save save = Save.Load<Save>();
        save.cards.Clear();
        foreach (var card in _cards)
        {
            save.cards.Add(new Save.CardPos()
            {
                code = Conversion.ToCode(card, save.GetKey()),
                position = card.transform.position,
                rotation = card.transform.eulerAngles
            });
        }
        save.Save();
    }

    private void MoveUp(int index)
    {
        for (; index < _cards.Count; index++)
        {
            Vector3 pos = _cards[index].transform.position;
            pos.z = BETWEEN_CARDS * -1 * (index - 1); // décale d'une carte vers l'avant (donc vers le négatif)
            _cards[index].transform.position = pos;
        }
    }

    // TODO a appeller quand la carte est fini d'être drop
    public void PutAtTop(CardInstance cardInstance)
    {
        // récupérer la position de la cardIstance dans la liste
        int index = _cards.IndexOf(cardInstance);
        if (index >= 0)
        {
            MoveUp(index + 1);
            _cards.RemoveAt(index);
        }
        _cards.Add(cardInstance);
        Vector3 newPos = cardInstance.transform.position;
        newPos.z = BETWEEN_CARDS * -1 * (_cards.Count - 1);

        SaveState();
    }

    public void Remove(CardInstance cardInstance)
    {
        int index = _cards.IndexOf(cardInstance);
        if (index >= 0)
        {
            MoveUp(index + 1);
            _cards.RemoveAt(index);
            SaveState();
        }
    }

    void Start()
    {
        Instance = this;
        instanceUpdated.Invoke();
    }

    private class Save : SaveBase
    {
        [Serializable]
        public class CardPos
        {
            public Vector3 position;
            public Vector3 rotation;
            public string code;
        }

        [SerializeField] public List<CardPos> cards = new();
        [SerializeField] private string _saltkey;
        protected override string PrefKey => "MoustiqueAreOnTheWayToPutOutTheFire";

        protected override void OnSaveInitialization()
        {
            if (string.IsNullOrEmpty(_saltkey))
            {
                _saltkey = Guid.NewGuid().ToString();
            }
        }

        public string GetKey()
        {
            string salted = $"{_saltkey}_AAAA_!x7";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(salted);
            return System.Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }
    }
}
