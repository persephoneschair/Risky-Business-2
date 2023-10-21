using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoundBase : MonoBehaviour
{
    public Question currentQuestion = null;

    public virtual void LoadQuestion()
    {
        if (QuestionManager.currentQuestionIndex == 0)
        {
            HostQuestionDisplay.Get.BringInBox();
            HostQuestionDisplay.Get.BringInTimer();
        }

        currentQuestion = QuestionManager.GetQuestion();

        if (Operator.Get.specialArunModeForR3 && GameplayManager.Get.currentRound == GameplayManager.Round.ThisOrThat)
            currentQuestion.answers = currentQuestion.answers.OrderBy(x => x.answer).ToList();
        else
            currentQuestion.answers.Shuffle();

        AudioManager.Get.Play(AudioManager.LoopClip.GameplayLoop);
        StartCoroutine(LoadDisplays());
    }

    public virtual IEnumerator LoadDisplays()
    {
        RunQuestion();
        yield return new WaitForSeconds(0f);
    }

    public virtual void RunQuestion()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.Ding);
        HostQuestionDisplay.Get.StartTimer();
        QuestionRunning();
    }

    public virtual void QuestionRunning()
    {
        
    }

    public virtual void OnQuestionEnded()
    {
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.ResetPostQuestion;
    }

    public virtual void ResetForNewQuestion()
    {
        ResetPlayerVariables();
    }

    public virtual void ResetPlayerVariables()
    {
        foreach (PlayerObject po in PlayerManager.Get.players)
        {
            po.wasCorrect = false;
            po.attemptedQ = false;
            po.chosenToRisk = false;
            po.strap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Default);
            po.cloneStrap.SetStrapColor(GlobalLeaderboardStrap.ColorOptions.Default);
        }
    }

    public virtual void EndOfRound()
    {
        AudioManager.Get.Play(AudioManager.LoopClip.WinTheme, false);
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RevealInstructions;
        GameplayManager.Get.currentRound++;
    }
}
