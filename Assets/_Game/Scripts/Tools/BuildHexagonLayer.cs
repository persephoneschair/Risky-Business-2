using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHexagonLayer : SingletonMonoBehaviour<BuildHexagonLayer>
{
    private const float sq3 = 1.7320508075688772935274463415059f;
    private float hexSideMultiplier = 1;

    public GameObject tile;
    public uint radius;

    public enum HexColor { Blue, Cyan, Green, Orange, Red, Black };
    public List<Renderer> hexRends = new List<Renderer>();
    public Material[] hexMats;

    public int danceIterations = 10;
    public float danceSwitchDuration = 1f;
    [Range(0, 10)] public int densityOfBlackOnDance = 6;

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
        SetHexesToRandomLit();
    }

    public void SetHexesToOff()
    {
        foreach (Renderer r in hexRends)
            r.material = hexMats[(int)HexColor.Black];
    }

    public void SetHexesToRandomLit()
    {
        foreach (Renderer r in hexRends)
        {
            int index = UnityEngine.Random.Range((int)HexColor.Blue, (int)HexColor.Black);
            r.material = hexMats[index];
        }
    }
    public void SetHexesToRandomLitWithBlack()
    {
        foreach (Renderer r in hexRends)
        {
            int index = UnityEngine.Random.Range((int)HexColor.Blue, (int)HexColor.Black);
            if (densityOfBlackOnDance != 0 && UnityEngine.Random.Range(0,densityOfBlackOnDance) != 0)
                index = (int)HexColor.Black;
            r.material = hexMats[index];
        }
    }

    [Button]
    public void SetHexesToDance()
    {
        StartCoroutine(DanceRoutine());
    }

    public void SetHexesToDance(int iterations, float switchDuration)
    {
        danceIterations = iterations;
        danceSwitchDuration = switchDuration;
        SetHexesToDance();
    }

    IEnumerator DanceRoutine()
    {
        for (int i = 0; i < danceIterations; i++)
        {
            SetHexesToRandomLitWithBlack();
            yield return new WaitForSeconds(danceSwitchDuration);
        }
        SetHexesToOff();
    }

    public void SpiralToColor()
    {
        StartCoroutine(SpiralRoutine());
    }

    IEnumerator SpiralRoutine()
    {
        foreach (Renderer r in hexRends)
        {
            yield return new WaitForSeconds(0.005f);
            r.material = hexMats[UnityEngine.Random.Range((int)HexColor.Blue, (int)HexColor.Black)];
        }
    }
}
