using DG.Tweening;
using EasyButtons;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Printer : MonoBehaviour
{
    public static Printer instance;

    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private TMPro.TMP_InputField _inputField;
    [SerializeField] private GameObject _boosterPrefab;
    [SerializeField] private Transform _exitPoint;

    
    
    private PrinterAnimation _printerAnimation;

    private void Awake()
    {
        Printer.instance = this;
        _printerAnimation = GetComponentInChildren<PrinterAnimation>();
    }

    public void EjectObject(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        obj.transform.position = _exitPoint.position + (Vector3)Random.insideUnitCircle * 0.1f;

        float angle = Random.Range(-25f, 25f);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;

        float force = Random.Range(1.5f, 3f);
        float torque = Random.Range(-3f, 3f);

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }

    public async void Print(string code)
    {
        _printerAnimation.StartPrinting();
        await _printerAnimation.OnEndPrinting;
        if (Conversion.IsAllowed(code))
        {
            Debug.Log($"Printing card with code: {code}");

            Conversion.ExcludeCode(code);
            EjectObject(_generatorConfig.GenerateCard(code, Resources.Load<BuildKey>("build_key")?.Value));
        }
    }

    public async void Print(CardData cardData)
    {
        _printerAnimation.StartPrinting();
        await _printerAnimation.OnEndPrinting;
        EjectObject(_generatorConfig.GenerateCard(cardData));
    }

    
    
    [ContextMenu("Print")]
    public void PrintDEBUG()
    {
        PrintBoosters(5);   
    }

    public async void PrintBoosters(int number)
    {
        _printerAnimation.StartPrinting();
        await _printerAnimation.OnEndPrinting;
        for (int i = 0; i < number; i++)
        {
            var obj = Instantiate(_boosterPrefab);
            obj.transform.SetParent(BoosterParentSingletion.instance.transform, true);
            EjectObject(obj);
            //await Task.Delay(TimeSpan.FromSeconds(0.5f)); ;
        }
    }
    
    private void SendToBoosterLand(GameObject obj)
    {
        obj.transform.position = this.transform.position;
        DOTween.Sequence()
                .SetLink(obj)
                .SetEase(Ease.InQuad)
                .Join(obj.transform.DOMove(BoosterParentSingletion.instance.transform.position, 0.2f))
                .OnComplete(() => obj.transform.SetParent(BoosterParentSingletion.instance.transform));

    }

    [Button]
    public void SpawnOneBoosterCenter()
    {
        PrintBoosters(1);
    }

    public void Print()
    {
        Print(_inputField.text);
    }
}
