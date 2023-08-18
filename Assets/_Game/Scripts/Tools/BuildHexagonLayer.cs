using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHexagonLayer : MonoBehaviour
{
    private const float sq3 = 1.7320508075688772935274463415059f;
    private float hexSideMultiplier = 1;

    public GameObject tile;
    public uint radius;

    public List<Renderer> hexRends = new List<Renderer>();
    public Material[] hexMats;

    void Start()
    {
        //Point of the next hexagon to be spawned
        Vector3 currentPoint = transform.position;

        if (tile.transform.localScale.x != tile.transform.localScale.z)
        {
            Debug.LogError("Hexagon has not uniform scale: cannot determine its side. Aborting");
            return;
        }

        //Spawn scheme: nDR, nDX, nDL, nUL, nUX, End??, UX, nUR
        Vector3[] mv = {
            new Vector3(1.5f,0, -sq3*0.5f),       //DR
            new Vector3(0,0, -sq3),               //DX
            new Vector3(-1.5f,0, -sq3*0.5f),      //DL
            new Vector3(-1.5f,0, sq3*0.5f),       //UL
            new Vector3(0,0, sq3),                //UX
            new Vector3(1.5f,0, sq3*0.5f)         //UR
        };

        int lmv = mv.Length;
        float HexSide = tile.transform.localScale.x * hexSideMultiplier;

        for (int mult = 0; mult <= radius; mult++)
        {
            int hn = 0;
            for (int j = 0; j < lmv; j++)
            {
                for (int i = 0; i < mult; i++, hn++)
                {
                    GameObject h = Instantiate(tile, currentPoint, tile.transform.rotation, transform);
                    h.name = string.Format("Hex Layer: {0}, n: {1}", mult, hn);
                    currentPoint += (mv[j] * HexSide);
                    hexRends.Add(h.GetComponentsInChildren<Renderer>()[1]);
                }

                //This is the final hexagon of any given "layer"
                if (j == 4)
                {
                    GameObject h = Instantiate(tile, currentPoint, tile.transform.rotation, transform);
                    h.name = string.Format("Hex Layer: {0}, n: {1}", mult, hn);
                    currentPoint += (mv[j] * HexSide);
                    hn++;
                    hexRends.Add(h.GetComponentsInChildren<Renderer>()[1]);
                    //Floors Completed
                    if (mult == radius)
                        break;
                }
            }
        }
        this.gameObject.transform.localEulerAngles = new Vector3(-90f, 0, 0);
        foreach (Renderer r in hexRends)
            r.material = hexMats[UnityEngine.Random.Range(0, hexMats.Length)];
    }
}
