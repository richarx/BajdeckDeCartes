using TMPro;
using UnityEngine;
using UnityEngine.UI;

//// idée: log clickable ? (pour copier les code du shredder)
public class HistoryDisplay : MonoBehaviour
{
    [SerializeField] Sprite infoIcon;
    [SerializeField] Sprite shredderCodeIcon;
    [SerializeField] Sprite achievementIcon;
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

    void CreateNewLog(LogType source, string text)
    {
        GameObject inst = Instantiate(referenceObj, targetParent);

        inst.GetComponentInChildren<Image>().sprite = source switch
        {
            LogType.Info => infoIcon,
            LogType.ShredderCode => shredderCodeIcon,
            LogType.Achievement => achievementIcon,

            _ => throw new System.NotImplementedException(),
        };

        inst.GetComponentInChildren<TextMeshProUGUI>().text = text;

        inst.SetActive(true);
    }
}
