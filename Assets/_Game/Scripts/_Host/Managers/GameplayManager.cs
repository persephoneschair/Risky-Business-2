using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Linq;
using Control;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    public RoundBase[] rounds;

    public static int nextQuestionIndex = 0;

    public enum GameplayStage
    {
        RunTitles,
        OpenLobby,
        LockLobby,
        RevealInstructions,
        HideInstructions,
        RunQuestion,
        RunPostBankOrRisk,

        ResetPostQuestion,
        RollCredits,
        DoNothing
    };

    [Header("Gameplay Status")]
    public GameplayStage currentStage = GameplayStage.DoNothing;

    public enum Round { FavourableOdds, TheBoardGame, ThisOrThat, None };
    public Round currentRound = Round.None;

    [Button]
    public void ProgressGameplay()
    {
        switch (currentStage)
        {
            case GameplayStage.RunTitles:
                if(Operator.Get.recoveryMode)
                {
                    //If in recovery mode, we need to call Restore Players to restore specific player data (client end should be handled by the reload host call)
                    //Also need to call Restore gameplay state to bring us back to where we need to be (skipping titles along the way)
                    //Reveal instructions would probably be a sensible place to go to, though check that doesn't iterate any game state data itself
                }
                else
                    TitlesManager.Get.RunTitleSequence();
                break;

            case GameplayStage.OpenLobby:
                LetterSpinner.Get.StartRotating();
                BuildHexagonLayer.Get.SpiralToColor();
                AudioManager.Get.Play(AudioManager.LoopClip.GameplayLoop);
                LobbyManager.Get.OnOpenLobby();
                currentStage++;
                break;

            case GameplayStage.LockLobby:
                AudioManager.Get.StopLoop();
                AudioManager.Get.Play(AudioManager.LoopClip.WinTheme, false);
                LobbyManager.Get.OnLockLobby();
                currentRound = Round.FavourableOdds;
                currentStage++;
                break;

            case GameplayStage.RevealInstructions:
                InstructionsManager.Get.OnShowInstructions();
                AudioManager.Get.Play(AudioManager.LoopClip.RoundIntro, false);
                currentStage++;
                break;

            case GameplayStage.HideInstructions:
                InstructionsManager.Get.OnHideInstructions();
                AudioManager.Get.Play(AudioManager.OneShotClip.Wheee);
                QuestionManager.currentQuestionIndex = 0;
                currentStage++;
                break;

            case GameplayStage.RunQuestion:
                rounds[(int)currentRound].LoadQuestion();
                currentStage = GameplayStage.DoNothing;
                break;

            case GameplayStage.RunPostBankOrRisk:
                (rounds[(int)currentRound] as FavourableOdds).RunPostBankOrRisk();
                currentStage = GameplayStage.DoNothing;
                break;

            case GameplayStage.ResetPostQuestion:
                currentStage = GameplayStage.RunQuestion;
                rounds[(int)currentRound].ResetForNewQuestion();
                QuestionManager.currentQuestionIndex++;
                break;

            case GameplayStage.RollCredits:
                GameplayPennys.Get.UpdatePennysAndMedals();
                LobbyManager.Get.TogglePermaCode();
                LetterSpinner.Get.KillLetters();
                foreach(PlayerObject po in PlayerManager.Get.players)
                {
                    po.strap.gameObject.SetActive(false);
                    po.cloneStrap.gameObject.SetActive(false);
                }    
                CreditsManager.Get.RollCredits();
                currentStage++;
                break;

            case GameplayStage.DoNothing:
                break;
        }
    }
}
