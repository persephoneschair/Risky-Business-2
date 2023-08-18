using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLeaderboardManager : MonoBehaviour
{
    public GameObject scrollRectContent;
    public ClientLeaderboardStrap[] straps;

    public void PopulateStrap(string[] data, int index)
    {
        straps[index].PopulateStrap(data, index);
    }

    public void RefreshScrollRect()
    {
        scrollRectContent.GetComponent<RectTransform>().localPosition = new Vector3(0, -2000f, 0);
    }

    public void OnMouseOver()
    {
        Debug.Log(this.gameObject.name);
    }
}
