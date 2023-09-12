using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ThisOrThat : RoundBase
{
    public override IEnumerator LoadDisplays()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"Get ready for the question...");

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < currentQuestion.answers.Count; i++)
        {
            HostQuestionDisplay.Get.BringInR3Box(i, currentQuestion.answers[i].answer);
            AudioManager.Get.PlayPop();
            yield return new WaitForSeconds(2f);
        }

        int index = QuestionManager.currentQuestionIndex;

        HostQuestionDisplay.Get.BringInMetaData($"Question {(index + 1).ToString()}/{QuestionManager.GetRoundQCount()}");
        HostQuestionDisplay.Get.BringInQuestionStrap(currentQuestion.question);
        StartCoroutine(base.LoadDisplays());
    }

    public override void QuestionRunning()
    {
        AudioManager.Get.StopLoop();
        AudioManager.Get.Play(AudioManager.LoopClip.R3Timer, false);
        Invoke("OnQuestionEnded", 8f);
        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultipleChoiceQuestion, $"{(QuestionManager.currentQuestionIndex + 1).ToString()}/{QuestionManager.GetRoundQCount()}) {currentQuestion.question}|7|{string.Join("|", currentQuestion.answers.Select(x => x.answer).ToArray())}");
    }

    public override void OnQuestionEnded()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.attemptedQ))
        {
            pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
            pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
        }

        LeaderboardManager.Get.OrderByRiskPoints();
        HostQuestionDisplay.Get.RevealR3BoxAnswers();

        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct answer was {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|CORRECT");
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct answer was {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|INCORRECT");

        base.OnQuestionEnded();
    }

    public override void ResetForNewQuestion()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.attemptedQ))
        {
            pl.strap.SetStrapScore(pl.riskPoints, 0, false);
            pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
            pl.strap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
            pl.cloneStrap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);

            pl.bankedPoints += pl.riskPoints;
            pl.riskPoints = 0;
            pl.pointsForNextQ = 1;
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
        }

        foreach(PlayerObject pl in PlayerManager.Get.players.Where(x => x.attemptedQ && !x.wasCorrect))
        {
            pl.strap.SetStrapScore(pl.riskPoints, 0, false);
            pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
            pl.riskPoints = 0;
            pl.pointsForNextQ = 1;

            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
        }

        if (QuestionManager.currentQuestionIndex != QuestionManager.GetRoundQCount() - 1)
            LeaderboardManager.Get.OrderByBankedPoints();

        base.ResetForNewQuestion();

        //Control for end of round
        if (QuestionManager.currentQuestionIndex == QuestionManager.GetRoundQCount() - 1)
        {
            foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.riskPoints > 0))
            {
                pl.strap.SetStrapScore(pl.riskPoints, 0, false);
                pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
                pl.strap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
                pl.cloneStrap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);

                pl.bankedPoints += pl.riskPoints;
                pl.riskPoints = 0;
                pl.pointsForNextQ = 1;

                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
            }

            LeaderboardManager.Get.OrderByBankedPoints();

            HostQuestionDisplay.Get.ClearObjectsEndOfRound();
            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"GAME OVER!\n\nYou have earned {(pl.bankedPoints * GameplayPennys.Get.multiplyFactor).ToString()} Pennys this game!");

            EndOfRound();
            GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RollCredits;
        }
        else
            HostQuestionDisplay.Get.ClearObjectsMidRound();
    }
}
