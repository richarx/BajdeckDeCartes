using System.Collections.Generic;
using UnityEngine;

namespace Game_Loop
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField] private BinderCompletion binderCompletion;
        [SerializeField] private Binder binderScript;
        [SerializeField] private BinderButton binderButton;
        [SerializeField] EntitySpawner spawner;
        [SerializeField] GameObject printerInScene;
        [SerializeField] GameObject shredderInScene;

        [Header("Tuto")]
        [SerializeField] private AchievementAsset firstBooster;
        [SerializeField] private AchievementAsset binder;
        [SerializeField] private AchievementAsset logHistory;
        [SerializeField] private AchievementAsset printer;
        [SerializeField] private AchievementAsset shredder;

        [Header("PageCompletion")]
        [SerializeField] private AchievementAsset firstPage;
        [SerializeField] private AchievementAsset secondPage;
        [SerializeField] private AchievementAsset thirdPage;

        [Header("CardsCompletion")]
        [SerializeField] private AchievementAsset firstCard;
        [SerializeField] private AchievementAsset secondCard;
        [SerializeField] private AchievementAsset thirdCard;
        [SerializeField] private AchievementAsset fourthCard;
        [SerializeField] private AchievementAsset Completion;

        [Header("ShreddCompletion")]
        [SerializeField] private AchievementAsset shreddFirst;
        [SerializeField] private AchievementAsset shreddSecond;
        [SerializeField] private AchievementAsset shreddThird;
        
        [Header("PrintCompletion")]
        [SerializeField] private AchievementAsset printFirst;
        [SerializeField] private AchievementAsset printSecond;
        [SerializeField] private AchievementAsset printThird;

        private bool hasBinderProc;

        private GameStatSave save;
        
        private void Start()
        {
            save = SaveBase.Load<GameStatSave>();
            
            BoosterOpening.OnFinishOpeningPack.AddListener(OnOpenPack);
            
            binderScript.OnSlotChanged += OnAddCard;
            ShredderAnimation.OnEndShredding.AddListener((_) => OnShredd());
            PrinterAnimation.OnEndPrinting.AddListener(OnPrint);
            //vrai shredder/printer plutôt que animation

            HistoryController.OnCloseLogPanel.AddListener(OnCloseHistoryPanel);

            SetupBoard();
        }

        private void SetupBoard()
        {
            printerInScene.SetActive(save.isPrinterUnlocked);
            shredderInScene.SetActive(save.isShredderUnlocked);
            binderButton.gameObject.SetActive(save.isBinderUnlocked);
        }

        private void OnOpenPack(List<CardInstance> cardInstances)
        {
            save.boostersOpenedCount += 1;
            save.Save();
            
            Debug.Log($"On Open Pack : {save.boostersOpenedCount}");
            
            if (save.boostersOpenedCount >= 1)
                firstBooster.Trigger();

            if (save.boostersOpenedCount >= 3)
            {
                binder.Trigger();
                binderButton.gameObject.SetActive(true);
                hasBinderProc = true;
                save.isBinderUnlocked = true;
                save.Save();
            }
        }

        private void OnShredd()
        {
            save.cardsShreddedCount += 1;
            save.Save();

            if (save.cardsShreddedCount >= 5)
                shreddFirst.Trigger();
            if (save.cardsShreddedCount >= 10)
                shreddSecond.Trigger();
            if (save.cardsShreddedCount >= 50)
                shreddThird.Trigger();
        }

        private void OnPrint()
        {
            save.cardsPrintedCount += 1;
            save.Save();

            if (save.cardsPrintedCount >= 1)
            {
                shredder.Trigger();
                save.isShredderUnlocked = true;
                save.Save();
            }

            if (save.cardsPrintedCount >= 5)
                printFirst.Trigger();
            if (save.cardsPrintedCount >= 10)
                printSecond.Trigger();
            if (save.cardsPrintedCount >= 50)
                printThird.Trigger();
        }

        private void OnAddCard()
        {
            int pageCount = binderCompletion.ComputeCompletedPagesCount();
            int cardCount = binderCompletion.ComputeTotalCardsCount();

            Debug.Log(cardCount);

            if (pageCount >= 1)
                firstPage.Trigger();
            if (pageCount >= 3)
                secondPage.Trigger();
            if (pageCount >= 7)
                thirdPage.Trigger();

            if (cardCount >= 1)
                logHistory.Trigger();

            if (cardCount >= 30)
                firstCard.Trigger();
            if (cardCount >= 50)
                secondCard.Trigger();
            if (cardCount >= 75)
                thirdCard.Trigger();
            if (cardCount >= 105)
                fourthCard.Trigger();
            if (cardCount >= 106)
                Completion.Trigger();
        }

        private void OnCloseHistoryPanel()
        {
            if (hasBinderProc == true)
            {
                printer.Trigger();
                ClipboardUtility.CopyToClipboard("AQAAJ3Y_DjT_kuwYZg");
                save.isPrinterUnlocked = true;
                save.Save();
            }
        }
    }
}
