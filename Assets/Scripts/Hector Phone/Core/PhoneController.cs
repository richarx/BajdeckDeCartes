using UnityEngine;

public class PhoneController : ReceivingAchievementMonoBehaviour
{
    PhoneDisplay display;

    public override void OnAchievementReceived(AchievementAsset achievement)
    {
        
    }

    void Awake()
    {
        display = GetComponent<PhoneDisplay>();
    }


}
