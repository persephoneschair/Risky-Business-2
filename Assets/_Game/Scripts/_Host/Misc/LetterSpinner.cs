using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSpinner : SingletonMonoBehaviour<LetterSpinner>
{
    public Animator[] anims;
    public bool rotating;

    public void StartRotating()
    {
        rotating = true;
        for(int i = 0; i < anims.Length; i++)
        {
            anims[i].gameObject.SetActive(true);
            StartCoroutine(RotationRoutine(i));
        }            
    }

    IEnumerator RotationRoutine(int index)
    {
        while(rotating)
        {
            int chance = UnityEngine.Random.Range(0, 5);
            anims[index].SetInteger("rotate", chance);
            yield return new WaitForSeconds(1f);
            anims[index].SetInteger("rotate", 0);
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
        }
    }

    public void KillLetters()
    {
        for (int i = 0; i < anims.Length; i++)
            anims[i].gameObject.SetActive(false);
    }
}
