using UnityEngine;
using TMPro;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public PlayerOneController p1;
    public PlayerTwoController p2;

    public GameObject modePanel;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;

    public ChaosAI ai;
    public BoardManager board;

    private int round = 1;

    private enum Phase { StartRound, ModeSelect, PieceSelect, Movement, AI }
    private Phase currentPhase;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        // Wait for BoardManager to finish loading tiles
        yield return null;

        ai.Initialize(board);
        StartCoroutine(RoundRoutine());
    }

    IEnumerator RoundRoutine()
    {
        while (true)
        {
            currentPhase = Phase.StartRound;
            yield return StartCoroutine(DoStartRound());

            currentPhase = Phase.ModeSelect;
            yield return StartCoroutine(DoModeSelect());

            currentPhase = Phase.PieceSelect;
            yield return StartCoroutine(DoPieceSelect());

            currentPhase = Phase.Movement;
            yield return StartCoroutine(DoMovement());

            currentPhase = Phase.AI;
            yield return StartCoroutine(DoAIPhase());

            round++;
        }
    }

    IEnumerator DoStartRound()
    {
        roundText.text = "ROUND " + round;
        roundText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        roundText.gameObject.SetActive(false);
    }

    // -------------------------
    // MODE SELECT (NO TIMER)
    // -------------------------
    IEnumerator DoModeSelect()
    {
        modePanel.SetActive(true);

        // Reset mode flags
        p1.modeChosen = false;
        p2.modeChosen = false;

        p1.EnableModeInput = true;
        p2.EnableModeInput = true;

        // Optional: clear or label the timer text for this phase
        timerText.text = "";

        // 🔥 Wait until BOTH have chosen a mode
        while (!p1.modeChosen || !p2.modeChosen)
        {
            yield return null;
        }

        // Stop inputs
        p1.EnableModeInput = false;
        p2.EnableModeInput = false;

        modePanel.SetActive(false);
    }

    // -------------------------
    // PIECE SELECT (still timed, tweak if you want)
    // -------------------------
    IEnumerator DoPieceSelect()
    {
        p1.EnableSelection = true;
        p2.EnableSelection = true;

        float t = 10f;
        while (t > 0f)
        {
            timerText.text = Mathf.Ceil(t).ToString();
            t -= Time.deltaTime;
            yield return null;
        }

        p1.EnableSelection = false;
        p2.EnableSelection = false;
    }

    // -------------------------
    // MOVEMENT / POSITIONING
    // 30s OR both players placed
    // -------------------------
    IEnumerator DoMovement()
    {
        bool p1Done = false;
        bool p2Done = false;

        // These should be invoked by the players when they finish placing their piece
        p1.onMoveComplete = () => p1Done = true;
        p2.onMoveComplete = () => p2Done = true;

        p1.EnableMovement = true;
        p2.EnableMovement = true;

        float t = 30f;

        while (t > 0f && (!p1Done || !p2Done))
        {
            timerText.text = Mathf.Ceil(t).ToString();
            t -= Time.deltaTime;
            yield return null;
        }

        p1.EnableMovement = false;
        p2.EnableMovement = false;
    }

    IEnumerator DoAIPhase()
    {
        ai.AdvanceLevel(round);
        ai.PerformTurn();

        yield return new WaitForSeconds(0.35f);
    }
}
