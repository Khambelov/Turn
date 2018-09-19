using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAnimation : MonoBehaviour {

    public Image progressBar;

    private void OnEnable()
    {
        StartCoroutine(LoadAnim());
    }

    IEnumerator LoadAnim()
    {
        float t = 60f;

        progressBar.fillClockwise = true;

        while (progressBar.fillAmount < 0.999f)
        {
            progressBar.fillAmount += 1 / t;
            yield return new WaitForSeconds(0.001f);
        }

        progressBar.fillClockwise = false;

        while (progressBar.fillAmount > 0f)
        {
            progressBar.fillAmount -= 1 / t;
            yield return new WaitForSeconds(0.001f);
        }

        StartCoroutine(LoadAnim());

        yield break;
    }
}
