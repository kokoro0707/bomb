using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public TMP_Text winnerText;

    public void SetWinner(string winner)
    {
        winnerText.text = winner + " ‚ÌŸ‚¿I";
    }

    public void OnRetry()
    {
        SceneManager.LoadScene("main");
    }

    public void OnTitle()
    {
        SceneManager.LoadScene("Title");
    }
}

