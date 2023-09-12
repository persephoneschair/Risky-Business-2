using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwitchLib.Client.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLeaderboardStrap : MonoBehaviour
{
    public Vector3 startPos;
    public PlayerObject containedPlayer;

    public TextMeshProUGUI playerNameMesh;
    public TextMeshProUGUI bankedMesh;
    public TextMeshProUGUI riskMesh;
    public RawImage avatarRend;

    public Image borderRend;
    public Image backgroundRend;

    public Color[] borderCols;
    public Color[] backgroundCols;
    public enum ColorOptions { Default, LockedIn, Correct, Incorrect };

    private Vector3 targetPosition;
    private float elapsedTime = 0;

    public void SetUpStrap()
    {
        startPos = GetComponent<RectTransform>().localPosition;
        targetPosition = startPos;
        playerNameMesh.text = "";
        bankedMesh.text = "";
        riskMesh.text = "";
        gameObject.SetActive(false);
    }

    public void PopulateStrap(PlayerObject pl, bool isClone)
    {
        //Flicker to life?
        gameObject.SetActive(true);
        containedPlayer = pl;
        playerNameMesh.text = pl.playerName;
        avatarRend.texture = pl.profileImage;
        bankedMesh.text = pl.bankedPoints.ToString();
        riskMesh.text = pl.riskPoints.ToString();
        if (!isClone)
            pl.strap = this;
        else
            pl.cloneStrap = this;
    }

    public void SetStrapColor(ColorOptions color)
    {
        backgroundRend.color = backgroundCols[(int)color];
        borderRend.color = borderCols[(int)color];
    }

    public void MoveStrap(Vector3 targetPos, int i)
    {
        //playerNameMesh.text = (i + 1).ToString();
        targetPosition = targetPos;
        elapsedTime = 0;
    }

    public void Update()
    {
        LerpStraps();
    }

    private void LerpStraps()
    {
        elapsedTime += Time.deltaTime * 1f;
        float percentageComplete = elapsedTime / LeaderboardManager.Get.reorderDuration;
        this.gameObject.transform.localPosition = Vector3.Lerp(this.gameObject.transform.localPosition, targetPosition, Mathf.SmoothStep(0, 1, percentageComplete));
    }

    public void SetStrapScore(int startValue, int endValue, bool isBank)
    {
        if (startValue == endValue)
            return;
        StartCoroutine(StrapTick(startValue, endValue, isBank));
    }

    IEnumerator StrapTick(int startValue, int endValue, bool isBank)
    {
        TextMeshProUGUI mesh = isBank ? bankedMesh : riskMesh;
        int iterateValue = endValue >= startValue ? 1 : -1;

        while(startValue != endValue)
        {
            startValue += iterateValue;
            mesh.text = startValue.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
