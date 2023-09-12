using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsManager : SingletonMonoBehaviour<InstructionsManager>
{
    public TextMeshProUGUI instructionsMesh;
    public Animator instructionsAnim;
    private readonly string[] instructions = new string[3]
    {
        "<font=RiskyBusiness><size=300%>Favourable Odds</size></font>\n\n" +
        "" +
        "Select one of the correct answers from the five presented options within fifteen seconds.\n\n" +
        "" +
        "With each question, the number of risk points available increases, whilst the number of correct answers decreases.\n\n" +
        "" +
        "An incorrect answer at any time will reduce your risk points to zero, however, abstention will preserve your currently accrued risk.\n\n" +
        "" +
        "To play question four, you must choose to <color=#F8A3A3>RISK</color> based on the answers alone. This question is worth double your risk points but only one of the answers will be correct. If you choose to risk, abstention will be treated as an incorrect answer.\n\n" +
        "" +
        "There are [###] sets of four questions in this round. Good luck!",



        "<font=RiskyBusiness><size=300%>The Board Game</size></font>\n\n" +
        "" +
        "Select as many of the nine presented options as you like within sixty seconds.\n\n" +
        "" +
        "At least four and up to seven of the answers will be correct.\n\n" +
        "" +
        "For each correct answer identified you earn five risk points.\n\n" +
        "" +
        "However, if you identify even one incorrect answer, you score nothing for the board.\n\n" +
        "" +
        "There are [###] boards in this round. Good luck!",



        "<font=RiskyBusiness><size=300%>This or That?</size></font>\n\n" +
        "" +
        "Select the correct answer from the two options presented within seven seconds.\n\n" +
        "" +
        "Answering correctly earns you increasingly more risk points (1, 2, 3 etc.)\n\n" +
        "" +
        "Answering incorrectly at any time will reduce your risk points to zero and reset your streak.\n\n" +
        "" +
        "Abstaining from a question will bank your risk points and reset your streak.\n\n" +
        "" +
        "There are [###] questions in this round. Good luck!",
    };

    [Button]
    public void OnShowInstructions()
    {
        instructionsAnim.SetTrigger("toggle");
        instructionsMesh.text = instructions[(int)GameplayManager.Get.currentRound].
            Replace("[###]", Extensions.NumberToWords(GameplayManager.Get.currentRound == GameplayManager.Round.FavourableOdds ? (QuestionManager.GetRoundQCount() / 4) : QuestionManager.GetRoundQCount()));
    }

    [Button]
    public void OnHideInstructions()
    {
        instructionsAnim.SetTrigger("toggle");
    }
}
