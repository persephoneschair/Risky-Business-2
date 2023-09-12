using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostQuestionDisplay : SingletonMonoBehaviour<HostQuestionDisplay>
{
    [Header("General Animators")]
    public Animator boxAnim;
    public Animator timerAppearAnim;
    public Animator timerBarAnim;
    public Animator metaDataAnim;
    public Animator questionBoxAnim;
    private float[] timerSpeeds = new float[3] { 1.65f, 0.515f, 4f };
    private float[] timerTickDelays = new float[3] { 12f, 52.25f, 2f };

    [Header("General Meshes")]
    public TextMeshProUGUI metaDataMesh;
    public TextMeshProUGUI questionMesh;

    [Header("Round 1")]
    public Animator r1JumpAnim;
    public AnswerDisplay[] r1Boxes;

    [Header("Round 2")]
    public Animator r2JumpAnim;
    public AnswerDisplay[] r2Boxes;

    [Header("Round 3")]
    public Animator r3JumpAnim;
    public AnswerDisplay[] r3Boxes;

    #region General

    public void BringInBox()
    {
        boxAnim.SetTrigger("toggle");
    }

    public void BringInTimer()
    {
        timerAppearAnim.SetTrigger("toggle");
    }

    public void BringInMetaData(string metaDataContent)
    {
        metaDataAnim.SetTrigger("toggle");
        metaDataMesh.text = metaDataContent;
    }

    public void BringInQuestionStrap(string questionContent)
    {
        questionBoxAnim.SetTrigger("toggle");
        questionMesh.text = questionContent;
    }

    #endregion

    #region Round Specific

    public void BringInR1Box(int index, string boxContent)
    {
        r1Boxes[index].SetDisplay(AnswerDisplay.DisplayOption.PreQuestion, boxContent);
    }

    public void RevealR1BoxAnswers()
    {
        r1JumpAnim.SetTrigger("toggle");
        for (int i = 0; i < r1Boxes.Length; i++)
            r1Boxes[i].SetResult(GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers[i].isCorrect);
    }

    public void BringInR2Box(int index, string boxContent)
    {
        r2Boxes[index].SetDisplay(AnswerDisplay.DisplayOption.PreQuestion, boxContent);
    }

    public void RevealR2BoxAnswers()
    {
        r2JumpAnim.SetTrigger("toggle");
        for (int i = 0; i < r2Boxes.Length; i++)
            r2Boxes[i].SetResult(GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers[i].isCorrect);
    }

    public void BringInR3Box(int index, string boxContent)
    {
        r3Boxes[index].SetDisplay(AnswerDisplay.DisplayOption.PreQuestion, boxContent);
    }

    public void RevealR3BoxAnswers()
    {
        r3JumpAnim.SetTrigger("toggle");
        for (int i = 0; i < r3Boxes.Length; i++)
            r3Boxes[i].SetResult(GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound].currentQuestion.answers[i].isCorrect);
    }

    #endregion

    #region Timer

    public void StartTimer(int overrideClockSpeed = 10)
    {
        timerBarAnim.speed = timerSpeeds[overrideClockSpeed == 10 ? (int)GameplayManager.Get.currentRound : overrideClockSpeed];
        timerBarAnim.SetTrigger("start");
        Invoke("PlayTimerTicks", timerTickDelays[overrideClockSpeed == 10 ? (int)GameplayManager.Get.currentRound : overrideClockSpeed]);
    }

    private void PlayTimerTicks()
    {
        StartCoroutine(TimerTicks());
    }

    public IEnumerator TimerTicks()
    {
        for(int i = 0; i < 24; i++)
        {
            AudioManager.Get.Play(AudioManager.OneShotClip.Tick);
            yield return new WaitForSeconds(0.25f);
        }
        AudioManager.Get.Play(AudioManager.OneShotClip.Ding);
    }

    public void ResetTimer()
    {
        timerBarAnim.speed = 1f;
        timerBarAnim.SetTrigger("reset");
    }

    #endregion

    #region Clearance

    public void ClearObjectsMidRound()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.Wheee);
        ResetTimer();
        questionBoxAnim.SetTrigger("toggle");
        metaDataAnim.SetTrigger("toggle");

        switch(GameplayManager.Get.currentRound)
        {
            case GameplayManager.Round.FavourableOdds:
                foreach (AnswerDisplay ad in r1Boxes)
                    ad.anim.SetTrigger("toggle");
                break;

            case GameplayManager.Round.TheBoardGame:
                foreach (AnswerDisplay ad in r2Boxes)
                    ad.anim.SetTrigger("toggle");
                break;

            case GameplayManager.Round.ThisOrThat:
                foreach (AnswerDisplay ad in r3Boxes)
                    ad.anim.SetTrigger("toggle");
                break;
        }
    }

    public void ClearObjectsEndOfRound()
    {
        ClearObjectsMidRound();
        BringInTimer();
        BringInBox();
    }

    #endregion
}
