using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TitlesManager : SingletonMonoBehaviour<TitlesManager>
{
    [TextArea (3,4)] public string[] titleOptions;
    public TextMeshProUGUI titlesMesh;
    public Animator titlesAnim;

    [Button]
    public void RunTitleSequence()
    {
        if (Operator.Get.skipOpeningTitles)
            EndOfTitleSequence();
        else
        {
            GameplayManager.Get.currentStage = GameplayManager.GameplayStage.DoNothing;
            StartCoroutine(TitleSequence());
        }           
    }

    IEnumerator TitleSequence()
    {
        //56 for titles
        //127 for credits
        //Both at 0.4372
        AudioManager.Get.Play(AudioManager.LoopClip.Titles, false);
        BuildHexagonLayer.Get.SetHexesToDance(56, 0.4372f);
        yield return new WaitForSeconds(2f);
        for(int i = 0; i < titleOptions.Length - 1; i++)
        {
            titlesMesh.text = titleOptions[i];
            titlesAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(5f);
            titlesAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(2f);
        }
        titlesMesh.text = titleOptions.LastOrDefault();
        titlesAnim.SetTrigger("toggle");
        yield return new WaitForSeconds(7f);
        EndOfTitleSequence();
    }

    void EndOfTitleSequence()
    {
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.OpenLobby;
        GameplayManager.Get.ProgressGameplay();
        this.gameObject.SetActive(false);
    }
}
