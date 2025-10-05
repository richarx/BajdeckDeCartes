using UnityEngine;

public class Printer : MonoBehaviour
{
    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private TMPro.TMP_InputField _inputField;

    public void Print(string code)
    {
        Debug.Log($"Printing card with code: {code} and key: {Resources.Load<BuildKey>("build_key")?.Value}");

        GameObject card = _generatorConfig.GenerateCard(code, Resources.Load<BuildKey>("build_key")?.Value);
    }

    public void Print()
    {
        Print(_inputField.text);
    }
}
