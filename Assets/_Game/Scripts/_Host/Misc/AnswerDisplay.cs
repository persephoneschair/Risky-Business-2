using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDisplay : MonoBehaviour
{
    public Animator anim;
    public TextMeshProUGUI mesh;
    public Image borderRend;
    public Image backgroundRend;
    public BetterTextBlock btb;

    public enum DisplayOption { PreQuestion, CorrectAnswer, IncorrectAnswer };
    public Color[] borderCols;
    public Color[] backgroundCols;

    public bool displayIsShown = false;

    public void SetDisplay(DisplayOption display, string text)
    {
        btb.borderColor = borderCols[(int)display];
        borderRend.color = borderCols[(int)display];
        btb.blockColorScheme = backgroundCols[(int)display];
        backgroundRend.color = backgroundCols[(int)display];
        mesh.text = text;
        anim.SetTrigger("toggle");
    }

    public void SetResult(bool correct)
    {
        btb.borderColor = correct ? borderCols[(int)DisplayOption.CorrectAnswer] : borderCols[(int)DisplayOption.IncorrectAnswer];
        borderRend.color = correct ? borderCols[(int)DisplayOption.CorrectAnswer] : borderCols[(int)DisplayOption.IncorrectAnswer];
        btb.blockColorScheme = correct ? backgroundCols[(int)DisplayOption.CorrectAnswer] : backgroundCols[(int)DisplayOption.IncorrectAnswer];
        backgroundRend.color = correct ? backgroundCols[(int)DisplayOption.CorrectAnswer] : backgroundCols[(int)DisplayOption.IncorrectAnswer];
    }
}
