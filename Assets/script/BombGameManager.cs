using TMPro;
using UnityEngine;
using UnityEngine.UI; // ← 追加（Text 用）

public class BombGameManager : MonoBehaviour
{
    [Header("UI表示")]
    public TMP_Text turnText;
    public TurnBannerAnimation turnBanner;

    [Header("爆弾スプライト")]
    public GameObject bombSprite;

    [Header("爆発ライン（ランダム）")]
    public int minExplosion = 20;
    public int maxExplosion = 90;

    private int explosionLine;
    private int pressCount = 0;

    private bool isPlayer1Turn = true;

    [Header("CPU設定")]
    private float cpuThinkTime = 0f;

    [Header("爆発演出")]
    public GameObject explosionEffectPrefab;
    public FlashEffect flash;
    public AudioSource explosionSE;

    [Header("結果画面UI")]
    public GameObject resultPanel;
    public ResultUI resultUI;

    [Header("神ポイント")]
    public int p1Points = 0;
    public int p2Points = 0;

    public TMP_Text p1PointText;

    public TMP_Text p2PointText;

    public Button godAdviceButton;     // 助言ボタン
    public GodAdviceSystem god;

    [Header("押した回数表示")]
    public TMP_Text PressCountText;

    bool doublePressForTuruns = false;
    bool forceOppnetTurNext = false;
    private bool hasPressedThisTurn = false;
    int doublePressRemaingTruns = 0;

    void Start()
    {
        explosionLine = Random.Range(minExplosion, maxExplosion + 1);

        // ▼ 最初に Player1 の番を表示
        UpdateTurnText();
        UpdatePressCountUI();
    }

    void Update()
    {
        if (GameMode.Instance.currentMode == PlayMode.TwoPlayers)
        {
            TwoPlayerMode();
        }
        else
        {
            CpuMode();
        }
    }

    // ============================
    // ▼ UIの更新（重要）
    // ============================
    void UpdateTurnText()
    {
        if (isPlayer1Turn)
        {
            turnBanner.PlayBanner( "Player1 の番！");
        }
        else
        {
            if (GameMode.Instance.currentMode == PlayMode.CPU)
                turnBanner.PlayBanner ( "CPU の番！");
            else
                turnBanner.PlayBanner("Player2 の番！");
        }
    }

    // ============================
    // ▼ 2人プレイモード
    // ============================
    void TwoPlayerMode()
    {
        if (isPlayer1Turn)
        {
            if (Input.GetKeyDown(KeyCode.A)) Push();
            if (Input.GetKeyDown(KeyCode.S)) PlayerEndTurn();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.L)) Push();
            if (Input.GetKeyDown(KeyCode.K)) PlayerEndTurn();
        }
    }

    // ============================
    // ▼ CPUモード
    // ============================
    void CpuMode()
    {
        if (isPlayer1Turn)
        {
            if (Input.GetKeyDown(KeyCode.Space)) Push();
            if (Input.GetKeyDown(KeyCode.LeftShift)) PlayerEndTurn();
        }
        else
        {
            cpuThinkTime -= Time.deltaTime;

            if (cpuThinkTime <= 0f)
            {
                cpuThinkTime = Random.Range(0.2f, 0.9f);

                if (Random.value < 0.7f)
                    Push();
                else
                    FoeceEndTrun();
            }
        }
    }

    // ============================
    // ▼ 押す
    // ============================
    void Push()
    {
        if (doublePressForTuruns)
            pressCount += 2;
        else
        pressCount+=1;

        hasPressedThisTurn = true;
        AddGodPoint(1);

        if (pressCount >= explosionLine)
        {
            Explosion();
            return;
        }
        UpdatePressCountUI();
    }
    void UpdatePressCountUI()
    {
        PressCountText.text = "押した回数:" + pressCount.ToString();
    }

    // ============================
    // ▼ ターン交代
    // ============================
    void EndTurnCore()
    {
        isPlayer1Turn = !isPlayer1Turn;
        hasPressedThisTurn = false;

        // ここに元の EndTurn() の処理
        // 例：doublePress のターン減少、forceOpponent の処理、UI更新など
        if (doublePressForTuruns)
        {
            doublePressRemaingTruns--;
            if (doublePressRemaingTruns <= 0)
                doublePressForTuruns = false;
        }

        if (forceOppnetTurNext)
        {
            isPlayer1Turn = !isPlayer1Turn;
            forceOppnetTurNext = false;
        }

        UpdateTurnText();
        UpdatePointUI();
    }

    void PlayerEndTurn()
    {
        if(!hasPressedThisTurn)
        {
            Debug.Log("交代できません");
            return;
        }
        EndTurnCore();
    }

    void FoeceEndTrun()
    {
        EndTurnCore();
    }

    // ============================
    // ▼ 爆発
    // ============================
    void Explosion()
    {
        bombSprite.SetActive(false);

        Instantiate(explosionEffectPrefab, bombSprite.transform.position, Quaternion.identity);
        StartCoroutine(flash.Flash());
        explosionSE.Play();

        string loser = isPlayer1Turn ?
            "Player1" :
            (GameMode.Instance.currentMode == PlayMode.CPU ? "CPU" : "Player2");

        string winner = (loser == "Player1") ? "Player2" : "Player1";

        if (GameMode.Instance.currentMode == PlayMode.CPU && winner == "Player2")
            winner = "CPU";

        // ▼ 結果パネルを表示
        resultPanel.SetActive(true);
        resultUI.SetWinner(winner);

        Debug.Log("💥 BOOM!!! 爆発！勝者 → " + winner);
    }

    void AddGodPoint(int amount)
    {
        if (isPlayer1Turn)
            p1Points += amount;
        else
            p2Points += amount;

        UpdatePointUI();
    }

    public void OnGodAdviceButton()
    {
        // 足りないときは無視
        if (isPlayer1Turn && p1Points < 5) return;
        if (!isPlayer1Turn && p2Points < 5) return;

        // ポイント消費
        if (isPlayer1Turn)
            p1Points -= 5;
        else
            p2Points -= 5;

        UpdatePointUI();

        // 助言を発動
        GiveGodAdvice();
    }

    void UpdatePointUI()
    {
        p1PointText.text = "P1 神P：" + p1Points;
        p2PointText.text = (GameMode.Instance.currentMode == PlayMode.CPU ?
                             "CPU 神P：" : "P2 神P：") + p2Points;

        // 今ターンのプレイヤーが 5P 以上持っていたらボタンを有効化
        if (isPlayer1Turn)
            godAdviceButton.interactable = p1Points >= 5;
        else
            godAdviceButton.interactable = p2Points >= 5;
    }
    void GiveGodAdvice()
    {
        float r = Random.value;

        int diff = explosionLine - pressCount;

        if(r<0.5f)
        {
            GiveTrueAdvice();
            return;
        }

        if(r<0.75f)
        {
            string[] lies = new string[]
            {
                "神 「押せ押せ押せぇい！」",
                "神 「まだまだ爆発せん大丈夫じゃ！」",
                "神 「交代など無用そのまま押せ」",
                "神 「今日のご飯はカレーじゃ」",
                "神 「今月は金欠じゃ！」",
                "神 「おれがトップだ」",
                "神 「もう大丈夫私が来た」",
                "神 「もうすぐ爆発じゃ！」",
                "神　「さっさと押せ」"
            };
            god.ShowMessage(lies[Random.Range(0, lies.Length)]);
            return;
        }
        TriggerBadEvent();
    }
    void GiveTrueAdvice()
    {
        int diff = explosionLine - pressCount;

        // diff に応じてセリフを自動切り替え
        if (diff >= 20)
        {
            string[] lines = new string[]
            {
            "神「む？まだ余裕があるようじゃな。」",
            "神「焦らずともよい、まだ遊べるぞ。」",
            "神「そなた、慎重さを忘れねば大丈夫じゃ。」",
            "神「まだまだ押せるぞ」"
            };
            god.ShowMessage(lines[Random.Range(0, lines.Length)]);
        }
        else if (diff >= 7)
        {
            string[] lines = new string[]
            {
            "神「そろそろ気を付ける頃じゃな。」",
            "神「軽率に押しすぎるでないぞ。」",
            "神「油断は禁物じゃが、まだ耐える。」",
            "神 「押す回数を考えるべきじゃな」",
            };
            god.ShowMessage(lines[Random.Range(0, lines.Length)]);
        }
        else if (diff >= 3)
        {
            string[] lines = new string[]
            {
            "神「かなり危険な領域に入ったぞ…。」",
            "神「そなた、無理をすると命取りじゃ。」",
            "神「決断を誤るな…交代も視野に入れよ。」"
            };
            god.ShowMessage(lines[Random.Range(0, lines.Length)]);
        }
        else if (diff >= 1)
        {
            string[] lines = new string[]
            {
            "神「危険じゃ！！あと数回で爆ぜるぞ…。」",
            "神「その押し方、爆弾がもう限界じゃ。」",
            "神「そなた…その一押しで死ぬやもしれんぞ。」",
            };
            god.ShowMessage(lines[Random.Range(0, lines.Length)]);
        }
        else // diff == 0（次で爆発確定）
        {
            string[] lines = new string[]
            {
            "神「次で確実に爆発するぞ…覚悟せよ。」",
            "神「これ以上は余でも止められぬ…。」",
            "神「次で終わりじゃ…天命に任せよ。」",
            "神 「絶対に押してはならん交代するんじゃ！」"
            };
            god.ShowMessage(lines[Random.Range(0, lines.Length)]);
        }
    }

    void TriggerBadEvent()
    {
        int ev = Random.Range(0, 6);

        switch(ev)
        {
            case 0:
                god.ShowMessage("神? 「2連続で相手のターンにしてやろう」");
                FoeceEndTrun();
                forceOppnetTurNext = true;
                    break;

            case 1:
                god.ShowMessage("神? 「強制交代じゃ」");
                FoeceEndTrun();
                break;

            case 2:
                god.ShowMessage("神? 「3ターンの間、押すたびに２回分じゃ」");
                doublePressForTuruns = true;
                doublePressRemaingTruns = 3;
                break;

            case 3:
                if (isPlayer1Turn) p1Points = 0;
                else p2Points = 0;
                UpdatePointUI();
                break;
            case 4:
                god.ShowMessage("神? 「爆発までの回数を削っておいたぞ...ふはは」");
                int raduce = Random.RandomRange(1, 4);
                explosionLine = Mathf.Max(1, explosionLine - raduce);
                break;
            case 5:
                god.ShowMessage("神? 「ターンを逆にしておいたぞ」");
                isPlayer1Turn = !isPlayer1Turn;
                UpdateTurnText();
                UpdatePointUI();
                break;

        }
    }
}