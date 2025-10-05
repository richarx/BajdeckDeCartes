using TMPro;
using UnityEngine;
using UnityEngine.UI;

//// idée: log clickable ? (pour copier les code du shredder)
public class HistoryDisplay : MonoBehaviour
{
    [SerializeField] Sprite noneIcon;
    [SerializeField] Sprite telephoneIcon;
    [SerializeField] Sprite shredderIcon;
    [Space]
    [SerializeField] GameObject referenceObj;

    Transform targetParent;

    void Awake()
    {
        targetParent = referenceObj.transform.parent;

        referenceObj.SetActive(false);
    }

    void Start() => History.OnNewLogWithCatchUp += CreateNewLog;

    void OnDestroy() => History.OnNewLogWithCatchUp -= CreateNewLog;

    void CreateNewLog(LogSource source, string text)
    {
        GameObject inst = Instantiate(referenceObj, targetParent);

        inst.GetComponentInChildren<Image>().sprite = source switch
        {
            LogSource.None => noneIcon,
            LogSource.Telephone => telephoneIcon,
            LogSource.Shredder => shredderIcon,

            _ => throw new System.NotImplementedException(),
        };

        inst.GetComponentInChildren<TextMeshProUGUI>().text = text;

        inst.SetActive(true);
    }
}
