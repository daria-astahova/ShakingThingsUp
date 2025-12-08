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

    // We don't really need PieceSelect anymore, everything is player-driven
    private enum Phase { StartRound, ModeSelect, Movement, AI }
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

            currentPhase = Phase.Movement;
            yield return StartCoroutine(DoMovement());

            // currentPhase = Phase.AI;
            // yield return StartCoroutine(DoAIPhase());

            round++;
        }
    }

    // -------------------------
    // ROUND START + RESET
    // -------------------------
    IEnumerator DoStartRound()
    {
        ResetRoundState();

        roundText.text = "ROUND " + round;
        roundText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        roundText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reset selectors, flags, callbacks, and UI at the start of each round.
    /// </summary>
    void ResetRoundState()
    {
        // Clear UI text
        if (timerText != null)
            timerText.text = "";

        // ----- Player 1 -----
        p1.EnableModeInput = false;
        p1.EnableSelection = false;
        p1.EnableMovement = false;

        p1.modeChosen = false;
        p1.onMoveComplete = null;

        // Reset selector position for P1 (C8 = x=2, y=7)
        p1.selX = 2;
        p1.selY = 7;
        Tile p1StartTile = board.GetTile(p1.selX, p1.selY);
        if (p1StartTile != null && p1.selector != null)
            p1.selector.position = p1StartTile.WorldPos + Vector3.up * 0.2f;

        // ----- Player 2 -----
        p2.EnableModeInput = false;
        p2.EnableSelection = false;
        p2.EnableMovement = false;

        p2.modeChosen = false;
        p2.onMoveComplete = null;

        // Reset selector position for P2 (C1 = x=2, y=0)
        p2.selX = 2;
        p2.selY = 0;
        Tile p2StartTile = board.GetTile(p2.selX, p2.selY);
        if (p2StartTile != null && p2.selector != null)
            p2.selector.position = p2StartTile.WorldPos + Vector3.up * 0.2f;
    }

    // -------------------------
    // MODE SELECT (NO TIMER)
    // -------------------------
    IEnumerator DoModeSelect()
    {
        modePanel.SetActive(true);

        // Reset mode flags (just in case)
        p1.modeChosen = false;
        p2.modeChosen = false;

        p1.EnableModeInput = true;
        p2.EnableModeInput = true;

        if (timerText != null)
            timerText.text = ""; // you can put "Choose your mode!" here if you want

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
    // MOVEMENT / POSITIONING
    // No timer, just waits until both finished
    // -------------------------
    IEnumerator DoMovement()
    {
        bool p1Done = false;
        bool p2Done = false;

        // These get called by the players when they finish placing their piece
        p1.onMoveComplete = () => p1Done = true;
        p2.onMoveComplete = () => p2Done = true;

        p1.EnableSelection = true;  // allow moving selector
        p2.EnableSelection = true;
        p1.EnableMovement = true;   // allow pick/drop
        p2.EnableMovement = true;

        if (timerText != null)
            timerText.text = ""; // or "Place your pieces"

        // 🔥 Wait until BOTH are done
        while (!p1Done || !p2Done)
        {
            yield return null;
        }

        p1.EnableSelection = false;
        p2.EnableSelection = false;
        p1.EnableMovement = false;
        p2.EnableMovement = false;
    }

    // IEnumerator DoAIPhase()
    // {
    //     ai.AdvanceLevel(round);
    //     ai.PerformTurn();

    //     yield return new WaitForSeconds(0.35f);
    // }
}
