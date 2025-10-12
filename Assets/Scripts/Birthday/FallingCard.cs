using System.Collections.Generic;
using UnityEngine;

public class FallingCard : MonoBehaviour
{
    [SerializeField] GameObject particulePrefab = null;

    [SerializeField] float width = 100f;
    
    [SerializeField] float scaleRange = 0.5f;
    [SerializeField] float scaleMean = 1.5f;

    [SerializeField] float rotationVelocityRange = 10;
    [SerializeField] float rotationMean = 20;

    [SerializeField] float fallingRange = 1.5f;
    [SerializeField] float fallingMean = 0.2f;

    [SerializeField] float meanTimeBetweenCard = 1f;
    [SerializeField] float rangeTimeBetweenCard = 0.8f;

    [SerializeField] List<CardData> _cardPool = new List<CardData>(); 
    
    private float _elapsedTime = 0f;
    private float _currentNextTime;
    private Printer _printer;

    private void Awake()
    {

        _printer = FindAnyObjectByType<Printer>(FindObjectsInactive.Include);
        Debug.Log(_printer);
    }
    private void Start()
    {
        _currentNextTime = GetRandom(meanTimeBetweenCard, rangeTimeBetweenCard);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _currentNextTime)
        {
            _elapsedTime -= _currentNextTime;
            _currentNextTime = GetRandom(meanTimeBetweenCard, rangeTimeBetweenCard);
            GameObject card = _printer.PrintCardFromPool(_cardPool);

            card.GetComponent<BoxCollider2D>().enabled = false;

            card.transform.SetParent(transform, true);
            float x = GetRandom(transform.position.x, width);

            float rotationVelocity = GetRandom(rotationMean, rotationVelocityRange);

            card.transform.SetPositionAndRotation(new Vector3(x, transform.position.y, transform.position.z), Quaternion.identity);
            
            Rotator rotator = card.AddComponent<Rotator>();
            rotator.rotationVelocity = rotationVelocity;

            float scale = GetRandom(scaleMean, scaleRange);
            card.GetComponent<Draggable>().SetToScale(scale);
            CardTableManager.Instance.Remove(card.GetComponent<Draggable>());
            card.transform.localScale = new Vector3(scale, scale, 1);

            Fall fall = card.AddComponent<Fall>();
            fall.velocity = GetRandom(fallingMean, fallingRange);

            Instantiate(particulePrefab, card.transform).transform.localPosition = Vector3.zero;
            
            var canvas = card.transform.GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvas.Length; i++)
            {
                canvas[i].sortingOrder = 4;
                canvas[i].sortingLayerName = "BIRTHD";
            }
        }
    }

    private float GetRandom(float mean, float width)
    {
        return (Random.Range(mean - width / 2, mean + width / 2));
    }
}
