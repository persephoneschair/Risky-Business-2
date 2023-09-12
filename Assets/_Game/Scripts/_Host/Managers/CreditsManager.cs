using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class CreditsManager : SingletonMonoBehaviour<CreditsManager>
{
    [TextArea(3, 4)] public string[] creditsOptions;
    public TextMeshProUGUI creditsMesh;
    public Animator creditsAnim;

    public GameObject endCard;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    [Button]
    public void RollCredits()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Credits());
    }

    IEnumerator Credits()
    {
        AudioManager.Get.Play(AudioManager.LoopClip.Credits, false);
        BuildHexagonLayer.Get.SetHexesToDance(127, 0.4372f);
        for (int i = 0; i < creditsOptions.Length - 1; i++)
        {
            creditsMesh.text = creditsOptions[i];
            creditsAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(3.05f);
            creditsAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(1.9f);
        }
        creditsMesh.text = creditsOptions.LastOrDefault();
        creditsAnim.SetTrigger("toggle");
        yield return new WaitForSeconds(3.75f);
        endCard.SetActive(true);
    }
}
