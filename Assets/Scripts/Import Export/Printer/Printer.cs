using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;

public class Printer : MonoBehaviour
{
    public static Printer instance;

    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private TMPro.TMP_InputField _inputField;
    [SerializeField] private GameObject _boosterPrefab;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private CardData _errorCard;

    private PrinterAnimation _printerAnimation;
    private bool _printError = false;

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
    

    public async UniTaskVoid Print(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            _printerAnimation.StartPrinting();
            await PrinterAnimation.OnEndPrinting;
            gameObject.SetActive(true);
            EjectObject(gameObject);
        }
    }

    public async UniTaskVoid Print(string code)
    {
        Conversion.Data data = Conversion.FromCode(code, Resources.Load<BuildKey>("build_key")?.Value);
        if (data != null && Conversion.IsAllowed(code))
        {
            if (data.Number > 200)
            {
                PrintBoosters(256 - data.Number).Forget();
                Conversion.ExcludeCode(code);
                return;
            }
            Debug.Log($"Printing card with code: {code}");
            _printerAnimation.StartPrinting();
            await PrinterAnimation.OnEndPrinting;

            Conversion.ExcludeCode(code);
            EjectObject(_generatorConfig.GenerateCard(code, Resources.Load<BuildKey>("build_key")?.Value));
        }
        else if (!_printError)
        {
            _printError = true;
            _printerAnimation.StartPrinting();
            await PrinterAnimation.OnEndPrinting;
            EjectObject(_generatorConfig.GenerateCard(_errorCard));
            _printError = false;
        }
    }

    public async UniTaskVoid Print(CardData cardData)
    {
        _printerAnimation.StartPrinting();
        await PrinterAnimation.OnEndPrinting;
        EjectObject(_generatorConfig.GenerateCard(cardData));
    }

    public async UniTaskVoid PrintBoosters(int number)
    {
        Debug.Log($"Printing {number} boosters");
        if (_printerAnimation != null)
        {

            _printerAnimation.StartPrinting();
            await PrinterAnimation.OnEndPrinting;
        }
        for (int i = 0; i < number; i++)
        {
            EjectObject(Instantiate(_boosterPrefab));
        }
    }

    [Button]
    public void SpawnOneBoosterCenter()
    {
        PrintBoosters(1).Forget();
    }

    public void Print()
    {
        Print(_inputField.text).Forget();
    }
}
