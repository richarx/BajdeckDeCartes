using UnityEngine;

public abstract class ReceivingAchievementMonoBehaviour : MonoBehaviour
{
    public abstract void OnAchievementReceived(AchievementAsset achievement);
}
