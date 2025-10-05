using UnityEngine;

public class Printer : MonoBehaviour
{
    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private TMPro.TMP_InputField _inputField;

    public void Print(string code)
    {
        if (Conversion.IsAllowed(code))
        {
            Debug.Log($"Printing card with code: {code}");

            Conversion.ExcludeCode(code);
            GameObject card = _generatorConfig.GenerateCard(code, Resources.Load<BuildKey>("build_key")?.Value);
        }
    }

    public void Print()
    {
        Print(_inputField.text);
    }
}
