using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    public Image flashImage;

    public IEnumerator Flash()
    {
        // îí Å® ìßñæ
        flashImage.color = new Color(1, 1, 1, 1);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4f;
            flashImage.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }
    }
}