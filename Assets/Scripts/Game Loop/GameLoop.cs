using System.Collections.Generic;
using UnityEngine;

namespace Game_Loop
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField] private AchievementAsset firstBooster;
        [SerializeField] private AchievementAsset binder;
        [SerializeField] private AchievementAsset logHistory;
        [SerializeField] private AchievementAsset printer;
        [SerializeField] private AchievementAsset shredder;

        private GameStatSave save;
        
        private void Start()
        {
            save = SaveBase.Load<GameStatSave>();
            
            BoosterOpening.OnFinishOpeningPack.AddListener(OnOpenPack);
        }

        private void OnOpenPack(List<CardInstance> cardInstances)
        {
            save.boostersOpenedCount += 1;
            save.Save();
            
            Debug.Log($"On Open Pack : {save.boostersOpenedCount}");
            
            if (save.boostersOpenedCount == 1)
                firstBooster.Trigger();
            
            if (save.boostersOpenedCount == 3)
                binder.Trigger();
        }
    }
}
