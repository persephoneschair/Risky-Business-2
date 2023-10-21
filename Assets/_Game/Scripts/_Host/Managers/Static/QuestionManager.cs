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

    public static string ConvertLegacyPack(TextAsset tx, string author)
    {
        currentPack = new Pack();
        currentPack.author = author;
        List<LegacyQuestion> legacyQs = JsonConvert.DeserializeObject<List<LegacyQuestion>>(tx.ToString());
        int counter = 0;
        foreach(LegacyQuestion lq in legacyQs)
        {
            Question q = new Question();
            q.question = lq.question;
            foreach(string a in lq.correctAnswers)
            {
                Answer ans = new Answer();
                ans.answer = a;
                ans.isCorrect = true;
                q.answers.Add(ans);
            }
            foreach(string a in lq.incorrectAnswers)
            {
                Answer ans = new Answer();
                ans.answer = a;
                ans.isCorrect = false;
                q.answers.Add(ans);
            }
            if (counter < 12)
                currentPack.favourableOdds.Add(q);
            else if (counter < 15)
                currentPack.boardGame.Add(q);
            else
                currentPack.thisOrThat.Add(q);
            counter++;
        }
        return JsonConvert.SerializeObject(currentPack, Formatting.Indented);
    }
}
