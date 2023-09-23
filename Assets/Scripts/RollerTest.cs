using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RollerTest : MonoBehaviour
{
    public int increaseNumber;
    void Start()
    {
        StartCoroutine(Test(increaseNumber));
    }
    IEnumerator Test(int i)
    {
        Debug.Log(i);
        yield return new WaitForSeconds(1.0f);
        if (i > 5)
        {
            StopCoroutine(Test(i));
        }
        StartCoroutine(Test(++i));
    }
}
