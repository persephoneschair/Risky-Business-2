using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheBoardGame : RoundBase
{
    public override IEnumerator LoadDisplays()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"Get ready for the question...");

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < currentQuestion.answers.Count; i++)
        {
            HostQuestionDisplay.Get.BringInR2Box(i, currentQuestion.answers[i].answer);
            AudioManager.Get.PlayPop();
            yield return new WaitForSeconds(1f);
        }

        int index = QuestionManager.currentQuestionIndex;

        HostQuestionDisplay.Get.BringInMetaData($"Question {(index + 1).ToString()}/{QuestionManager.GetRoundQCount()} | Anything from four to seven may be correct");
        HostQuestionDisplay.Get.BringInQuestionStrap(currentQuestion.question);
        StartCoroutine(base.LoadDisplays());
    }

    public override void QuestionRunning()
    {
        AudioManager.Get.StopLoop();
        AudioManager.Get.Play(AudioManager.LoopClip.R2Timer, false);
        Invoke("OnQuestionEnded", 59f);

        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultiSelectQuestion, $"{currentQuestion.question}|57|{string.Join("|", currentQuestion.answers.Select(x => x.answer).ToArray())}");
    }

    public override void OnQuestionEnded()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players)
        {
            if(pl.wasCorrect)
            {
                pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
                pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Correct);
            }
            else
            {
                pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
            }
        }
        LeaderboardManager.Get.OrderByCorrectRiskPoints();
        HostQuestionDisplay.Get.RevealR2BoxAnswers();

        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct answers were {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|CORRECT");
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct answers were {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|INCORRECT");

        base.OnQuestionEnded();
    }

    public override void ResetForNewQuestion()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players)
        {
            if (pl.wasCorrect)
            {
                pl.strap.SetStrapScore(pl.bankedPoints, (pl.riskPoints + pl.bankedPoints), true);
                pl.cloneStrap.SetStrapScore(pl.bankedPoints, (pl.riskPoints + pl.bankedPoints), true);
                pl.bankedPoints += pl.riskPoints;
            }

            pl.strap.SetStrapScore(pl.riskPoints, 0, false);
            pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
            pl.riskPoints = 0;

            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
        }
        LeaderboardManager.Get.OrderByBankedPoints();

        base.ResetForNewQuestion();

        //Control for end of round
        if (QuestionManager.currentQuestionIndex == QuestionManager.GetRoundQCount() - 1)
        {
            HostQuestionDisplay.Get.ClearObjectsEndOfRound();
            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, "End of round 2");

            EndOfRound();
        }
        else
            HostQuestionDisplay.Get.ClearObjectsMidRound();
    }
}
