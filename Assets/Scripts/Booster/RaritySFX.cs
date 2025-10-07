using UnityEngine;

public class RaritySFX : MonoBehaviour
{
    [SerializeField] private AudioClip common;
    [SerializeField] private AudioClip rare;
    [SerializeField] private AudioClip epic;
    [SerializeField] private AudioClip legendary;

    void Start()
    {
        BoosterGlobalAnimation.OnDiscoverCard.AddListener(PlayRaritySound);
    }

    void Update()
    {
        
    }

    private void PlayRaritySound(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                SFXManager.Instance.PlaySFX(common);
                break;
            case Rarity.Rare:
                SFXManager.Instance.PlaySFX(rare);
                break;
            case Rarity.Epic:
                SFXManager.Instance.PlaySFX(epic);
                break;
            case Rarity.Legendary:
                SFXManager.Instance.PlaySFX(legendary);
                break;
        }
    }
}
