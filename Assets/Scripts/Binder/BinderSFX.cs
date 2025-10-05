using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BinderSFX : MonoBehaviour
{
    [SerializeField] private List<AudioClip> turnPageSounds;
    [SerializeField] private List<AudioClip> openBookSounds;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayTurnPageSound()
    {
        SFXManager.Instance.PlayRandomSFX(turnPageSounds.ToArray());
    }

    public void PlayOpenBookSounds()
    {
        SFXManager.Instance.PlayRandomSFX(openBookSounds.ToArray());
    }
}
