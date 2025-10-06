using UnityEngine;

public class BoosterParentSingletion : MonoBehaviour
{
    public static BoosterParentSingletion instance;

    private void Awake()
    {
        BoosterParentSingletion.instance = this;   
    }
}
