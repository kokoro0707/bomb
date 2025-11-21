using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtons : MonoBehaviour
{
    [Header("説明パネル")]
    public GameObject titlePanel;
    public GameObject howToPlayPanel;
    public GameObject rulePanel1;
    public GameObject rulePanel2;

    private enum Step
    {
        Title,
        HowToPlay,
        Rule1,
        Rule2,
        Done
    }

    private Step currentStep = Step.Title;

    void Update()
    {
        // スペースキーで進行
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GoNextStep();
        }
    }

    // ▼ CPUボタン
    public void OnCPUButton()
    {
        GameMode.Instance.SetMode(PlayMode.CPU);
        ShowHowToPlay();
    }

    // ▼ 2Pボタン
    public void OnTwoPlayersButton()
    {
        GameMode.Instance.SetMode(PlayMode.TwoPlayers);
        ShowHowToPlay();
    }

    // ▼ 操作説明パネル表示
    void ShowHowToPlay()
    {
        currentStep = Step.HowToPlay;

        titlePanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        rulePanel1.SetActive(false);
        rulePanel2.SetActive(false);
    }

    // ▼ ステップごとの進行
    void GoNextStep()
    {
        switch (currentStep)
        {
            case Step.HowToPlay:
                currentStep = Step.Rule1;
                howToPlayPanel.SetActive(false);
                rulePanel1.SetActive(true);
                break;

            case Step.Rule1:
                currentStep = Step.Rule2;
                rulePanel1.SetActive(false);
                rulePanel2.SetActive(true);
                break;

            case Step.Rule2:
                currentStep = Step.Done;
                SceneManager.LoadScene("main");
                break;
        }
    }
}

