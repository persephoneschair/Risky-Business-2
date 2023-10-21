using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class FavourableOdds : RoundBase
{
    public override IEnumerator LoadDisplays()
    {
        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"Get ready for the question...");

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < currentQuestion.answers.Count; i++)
        {
            HostQuestionDisplay.Get.BringInR1Box(i, currentQuestion.answers[i].answer);
            AudioManager.Get.PlayPop();
            yield return new WaitForSeconds(2f);
        }

        int index = QuestionManager.currentQuestionIndex;
        string[] correct = new string[4] { "4 correct answers", "3 correct answers", "2 correct answers", "1 correct answer" };
        string[] pts = new string[4] { "2 points", "3 points", "5 points", "x2 points" };

        if(QuestionManager.currentQuestionIndex % 4 == 3)
        {
            HostQuestionDisplay.Get.BringInQuestionStrap("BANK OR RISK?");
            BankOrRiskRunning();
        }            
        else
        {
            HostQuestionDisplay.Get.BringInMetaData($"Question {((index % 4) + 1).ToString()}/4 | {correct[index % 4]} | {pts[index % 4]}");
            HostQuestionDisplay.Get.BringInQuestionStrap(currentQuestion.question);
            StartCoroutine(base.LoadDisplays());
        }
    }

    public override void QuestionRunning()
    {
        AudioManager.Get.StopLoop();
        AudioManager.Get.Play(AudioManager.LoopClip.R1Timer, false);
        Invoke("OnQuestionEnded", 18.25f);

        if (QuestionManager.currentQuestionIndex % 4 == 3)
        {
            foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.chosenToRisk))
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultipleChoiceQuestion, $"{currentQuestion.question}|15|{string.Join("|", currentQuestion.answers.Select(x => x.answer).ToArray())}");
        }
        else
            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultipleChoiceQuestion, $"{currentQuestion.question}|15|{string.Join("|", currentQuestion.answers.Select(x => x.answer).ToArray())}");
    }

    public override void OnQuestionEnded()
    {
        if (QuestionManager.currentQuestionIndex % 4 == 3)
            foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect && x.chosenToRisk))
            {
                pl.strap.SetStrapScore(pl.riskPoints, 0, false);
                pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
                pl.riskPoints = 0;
                pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Incorrect);
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
            }

        else
        {
            foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.attemptedQ))
            {
                pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
                pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
            }
        }

        LeaderboardManager.Get.OrderByRiskPoints();
        string wereOrWas = QuestionManager.currentQuestionIndex % 4 == 3 ? "answer was" : "answers were";
        HostQuestionDisplay.Get.RevealR1BoxAnswers();

        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct {wereOrWas} {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|CORRECT");
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.attemptedQ))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct {wereOrWas} {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|DEFAULT");
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"The correct {wereOrWas} {string.Join(", ", currentQuestion.answers.Where(x => x.isCorrect).Select(x => x.answer).ToArray())}|INCORRECT");

        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");

        base.OnQuestionEnded();
    }

    public void BankOrRiskRunning()
    {
        HostQuestionDisplay.Get.StartTimer(2);
        AudioManager.Get.Play(AudioManager.LoopClip.R3Timer, false);
        Invoke("OnBankOrRiskEnded", 8f);
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.riskPoints > 0))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultipleChoiceQuestion, $"{string.Join("\n", currentQuestion.answers.Select(x => x.answer).ToArray())}\n\nHit the RISK! button if you want to play!|7|RISK!");
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.riskPoints == 0))
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"You have no risk and so will not be able to play this question");

    }

    public void OnBankOrRiskEnded()
    {
        HostQuestionDisplay.Get.ResetTimer();
        HostQuestionDisplay.Get.BringInQuestionStrap("BANK OR RISK?");
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RunPostBankOrRisk;
        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.riskPoints > 0 && !x.chosenToRisk))
        {
            pl.strap.SetStrapScore(pl.riskPoints, 0, false);
            pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
            pl.strap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
            pl.cloneStrap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
            pl.bankedPoints += pl.riskPoints;
            pl.riskPoints = 0;
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, "Your points have been banked.");
            pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
            pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.LockedIn);
        }
        foreach(PlayerObject pl in PlayerManager.Get.players.Where(x => x.chosenToRisk))
        {
            pl.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Default);
            pl.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Default);
        }
        LeaderboardManager.Get.OrderByRiskPoints();
    }

    public void RunPostBankOrRisk()
    {
        HostQuestionDisplay.Get.BringInMetaData($"Question 4/4 | 1 correct answer | x2 points");
        HostQuestionDisplay.Get.BringInQuestionStrap(currentQuestion.question);
        base.RunQuestion();
    }


    public override void ResetForNewQuestion()
    {
        if (QuestionManager.currentQuestionIndex % 4 == 3)
        {
            foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.riskPoints > 0))
            {
                pl.strap.SetStrapScore(pl.riskPoints, 0, false);
                pl.cloneStrap.SetStrapScore(pl.riskPoints, 0, false);
                pl.strap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
                pl.cloneStrap.SetStrapScore(pl.bankedPoints, pl.bankedPoints + pl.riskPoints, true);
                pl.bankedPoints += pl.riskPoints;
                pl.riskPoints = 0;
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"<color=#F8A3A3>Risk: {pl.riskPoints}</color>\nBank: {pl.bankedPoints}");
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, "Your points have been banked.");
            }
            LeaderboardManager.Get.OrderByBankedPoints();
        }

        base.ResetForNewQuestion();

        //Control for end of round
        if (QuestionManager.currentQuestionIndex == QuestionManager.GetRoundQCount() - 1)
        {
            HostQuestionDisplay.Get.ClearObjectsEndOfRound();
            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, "End of round 1");

            EndOfRound();
        }
        else
            HostQuestionDisplay.Get.ClearObjectsMidRound();
    }
}
