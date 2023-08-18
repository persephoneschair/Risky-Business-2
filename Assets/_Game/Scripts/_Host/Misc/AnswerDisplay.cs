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
    public Color[] borderCols;
    public Color[] backgroundCols;

    public bool displayIsShown = false;
}
