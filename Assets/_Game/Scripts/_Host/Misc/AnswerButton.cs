using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public Button button;
    public Animator anim;
    public TextMeshProUGUI mesh;
    public bool buttonIsShown = false;
    public Image checkmark;

    public void PrepButtonForReveal(string text)
    {
        checkmark.enabled = false;
        button.interactable = false;
        if(!buttonIsShown)
        {
            buttonIsShown = true;
            anim.SetTrigger("toggle");
        }        
        mesh.text = text;
    }

    public void PrepButtonForAnswering()
    {
        button.interactable = true;
    }

    public void PlayerHasAnswered(bool playedThisAnswer)
    {
        checkmark.enabled = playedThisAnswer;
        button.interactable = false;
    }

    public void HideButton()
    {
        checkmark.enabled = false;
        button.interactable = false;
        if (buttonIsShown)
        {
            buttonIsShown = false;
            anim.SetTrigger("toggle");
        }
    }
}
