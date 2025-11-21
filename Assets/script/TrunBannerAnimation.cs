using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TurnBannerAnimation : MonoBehaviour
{
    public TMP_Text bannerText;

    RectTransform rect;

    Vector2 leftStart;   // 左の外
    Vector2 centerPos;   // 画面中央
    Vector2 rightEnd;    // 右の外

    public float speedFast = 1400f;   // 通常の速いスピード
    public float speedSlow = 250f;    // 画面中央だけ遅いスピード

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        float w = Screen.width;

        leftStart = new Vector2(-w, 0);
        centerPos = new Vector2(0, 0);
        rightEnd = new Vector2(w, 0);
    }

    public void PlayBanner(string message)
    {
        bannerText.text = message;
        StartCoroutine(BannerFlow());
    }

    IEnumerator BannerFlow()
    {
        // 1. 左外に初期化
        rect.anchoredPosition = leftStart;

        // 2. 左 → 中央（速い）
        while (Vector2.Distance(rect.anchoredPosition, centerPos) > 10f)
        {
            rect.anchoredPosition =
                Vector2.MoveTowards(rect.anchoredPosition, centerPos, speedFast * Time.deltaTime);

            yield return null;
        }

        // 3. 中央 → 少し右（遅い）
        Vector2 slowEnd = new Vector2(Screen.width * 0.3f, 0);

        while (Vector2.Distance(rect.anchoredPosition, slowEnd) > 10f)
        {
            rect.anchoredPosition =
                Vector2.MoveTowards(rect.anchoredPosition, slowEnd, speedSlow * Time.deltaTime);

            yield return null;
        }

        // 4. 最後は速い速度で右外へ
        while (Vector2.Distance(rect.anchoredPosition, rightEnd) > 10f)
        {
            rect.anchoredPosition =
                Vector2.MoveTowards(rect.anchoredPosition, rightEnd, speedFast * Time.deltaTime);

            yield return null;
        }
    }
}