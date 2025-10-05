using UnityEngine;
using EasyButtons;

public class Printer : MonoBehaviour
{
    public static Printer instance;

    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private TMPro.TMP_InputField _inputField;

    [SerializeField] private GameObject _boosterPrefab;

    private void Awake()
    {
        Printer.instance = this;
    }

    public void Print(string code)
    {
        if (Conversion.IsAllowed(code))
        {
            Debug.Log($"Printing card with code: {code}");

            Conversion.ExcludeCode(code);
            GameObject card = _generatorConfig.GenerateCard(code, Resources.Load<BuildKey>("build_key")?.Value);
        }
    }
    
    public void PrintBoosters(Vector3 startPosition, int number)
    {
        for (int i = 0; i< number;i++)
        {
            var newBooster = Instantiate(_boosterPrefab);

            // Put anim
            newBooster.transform.position = startPosition;
        }
    }

    [Button]
    public void SpawnOneBoosterCenter()
    {
        PrintBoosters(Vector3.zero, 1);
    }

    public void Print()
    {
        Print(_inputField.text);
    }
}
