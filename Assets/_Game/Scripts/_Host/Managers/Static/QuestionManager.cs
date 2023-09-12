using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.PlayerLoop;

public static class QuestionManager
{
    public static Pack currentPack = null;
    public static int currentQuestionIndex;

    public static void DecompilePack(TextAsset tx)
    {
        currentPack = JsonConvert.DeserializeObject<Pack>(tx.text);
    }

    public static int GetRoundQCount()
    {
        switch (GameplayManager.Get.currentRound)
        {
            case GameplayManager.Round.FavourableOdds:
                return currentPack.favourableOdds.Count;

            case GameplayManager.Round.TheBoardGame:
                return currentPack.boardGame.Count;

            case GameplayManager.Round.ThisOrThat:
                return currentPack.thisOrThat.Count;
            default:
                return 0;
        }
    }

    public static Question GetQuestion()
    {
        switch (GameplayManager.Get.currentRound)
        {
            case GameplayManager.Round.FavourableOdds:
                return currentPack.favourableOdds[currentQuestionIndex];

            case GameplayManager.Round.TheBoardGame:
                return currentPack.boardGame[currentQuestionIndex];

            case GameplayManager.Round.ThisOrThat:
                return currentPack.thisOrThat[currentQuestionIndex];

            default:
                return null;
        }
    }
}
