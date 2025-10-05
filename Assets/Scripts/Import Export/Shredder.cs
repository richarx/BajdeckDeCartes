using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> OnCardShredded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Card"))
        {
            CardInstance cardInstance = collision.GetComponent<CardInstance>();
            if (cardInstance != null)
            {
                string code = Conversion.ToCode(cardInstance, Resources.Load<BuildKey>("build_key")?.Value);
                Debug.Log($"Carte déchiquetée : {code} avec clé {Resources.Load<BuildKey>("build_key")?.Value}");
                ClipboardUtility.CopyToClipboard(code);
                OnCardShredded?.Invoke(code);
            }

            Destroy(collision.gameObject);
        }
    }
}
