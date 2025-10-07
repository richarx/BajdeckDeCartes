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
                SFXManager.Instance.PlaySFXNoPitchModifier(common);
                break;
            case Rarity.Rare:
                SFXManager.Instance.PlaySFXNoPitchModifier(rare);
                break;
            case Rarity.Epic:
                SFXManager.Instance.PlaySFXNoPitchModifier(epic);
                break;
            case Rarity.Legendary:
                SFXManager.Instance.PlaySFXNoPitchModifier(legendary);
                break;
        }
    }
}
