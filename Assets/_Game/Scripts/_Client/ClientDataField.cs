using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientDataField : MonoBehaviour
{
    public TextMeshProUGUI mesh;
    public BetterTextBlock betterTextBlock;

    public void UpdateMesh(string text)
    {
        mesh.text = text;
    }
}
