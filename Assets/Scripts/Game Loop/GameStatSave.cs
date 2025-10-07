using UnityEngine;

namespace Game_Loop
{
    public class GameStatSave : SaveBase
    {
        [SerializeField] public int cardsPrintedCount;
        [SerializeField] public int cardsShreddedCount;
        [SerializeField] public int boostersOpenedCount;
        [SerializeField] public bool isPrinterUnlocked;
        [SerializeField] public bool isShredderUnlocked;
        [SerializeField] public bool isBinderUnlocked;

        protected override string PrefKey => "Game Stats";
    }
}
