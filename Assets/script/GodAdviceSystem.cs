using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GodAdviceSystem : MonoBehaviour
{
    public TMP_Text messageText;
    public float messageTime = 3f; // •\Ž¦ŽžŠÔ

    public void ShowMessage(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(Show(msg));
    }

    IEnumerator Show(string msg)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageTime);

        messageText.gameObject.SetActive(false);
    }
}