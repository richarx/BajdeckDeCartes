using UnityEngine;

public class CardParentSingleton : MonoBehaviour
{
    public static CardParentSingleton instance = null;

    public void Awake()
    {
        CardParentSingleton.instance = this;
    }
}
