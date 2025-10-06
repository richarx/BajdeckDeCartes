using UnityEngine;

namespace Game_Loop
{
    public class GameStatSave : SaveBase
    {
        [SerializeField] public int cardsPrintedCount;
        [SerializeField] public int cardsShreddedCount;
        [SerializeField] public int boostersOpenedCount;

        protected override string PrefKey => "Game Stats";
    }
}
