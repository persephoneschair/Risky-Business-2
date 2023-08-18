using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientLeaderboardStrap : MonoBehaviour
{
    public TextMeshProUGUI positionMesh;
    public TextMeshProUGUI nameMesh;
    public TextMeshProUGUI riskMesh;
    public TextMeshProUGUI bankMesh;
    public BetterBox box;
    public Color[] boxOptions;
    public Color[] borderOptions;

    public void PopulateStrap(string[] data, int index)
    {
        positionMesh.text = Extensions.AddOrdinal(index + 1);
        nameMesh.text = data[0];
        bankMesh.text = data[1];
        riskMesh.text = data[2];

        SetBorderColor(nameMesh.text.ToUpperInvariant().Trim() == ClientMainGame.Get.nameMesh.text.ToUpperInvariant().Trim());
    }

    public void SetBorderColor(bool isClient)
    {
        box.borderColor = isClient ? borderOptions[1] : borderOptions[0];
        box.border.color = isClient ? borderOptions[1] : borderOptions[0];
    }
}
