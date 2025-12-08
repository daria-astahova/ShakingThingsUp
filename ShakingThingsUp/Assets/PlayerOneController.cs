using UnityEngine;

public class PlayerOneController : MonoBehaviour
{
    public BoardManager board;
    public bool modeChosen = false;

    [Header("Selector")]
    public Transform selector;
    public int selX = 2; // Start at middle col
    public int selY = 7; // Row 8 (0-index is 7)

    [Header("Pieces")]
    public GameObject queen;      // power = 2
    public GameObject knightLeft; // power = 2
    public GameObject knightRight;// power = 1

    private GameObject heldPiece = null;

    private enum Mode { Attack, Defend }
    private Mode currentMode = Mode.Defend;

    public bool EnableModeInput = false;
    public bool EnableSelection = false;
    public bool EnableMovement = false;

    public System.Action onMoveComplete;

    void Update()
    {
        if (EnableModeInput)
            HandleModeInput();

        if (EnableSelection)
            HandleSelectorMovement();

        if (EnableMovement)
            HandlePickDrop();
    }

    // ---------------- MODE SELECT ----------------
    void HandleModeInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentMode = Mode.Attack;
            modeChosen = true;
            Debug.Log("P1 chose ATTACK");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentMode = Mode.Defend;
            modeChosen = true;
            Debug.Log("P1 chose DEFEND");
        }
    }

    // ---------------- SELECTOR MOVEMENT ----------------
    void HandleSelectorMovement()
    {
        if (Input.GetKeyDown(KeyCode.W)) selY = Mathf.Clamp(selY - 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.S)) selY = Mathf.Clamp(selY + 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.A)) selX = Mathf.Clamp(selX - 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.D)) selX = Mathf.Clamp(selX + 1, 0, 7);

        Tile t = board.GetTile(selX, selY);
        if (t != null && selector != null)
            selector.position = t.WorldPos + Vector3.up * 0.2f;
    }

    // ---------------- PICK / DROP ----------------
    void HandlePickDrop()
    {
        Tile tile = board.GetTile(selX, selY);
        if (tile == null) return;

        // Q = pick up
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldPiece != null) return;

            if (tile.value <= 0)
            {
                Debug.Log("Opponent’s square! Cannot pick up.");
                return;
            }

            if (tile.hasCharacter && tile.occupyingPiece.CompareTag("P1"))
            {
                heldPiece = tile.occupyingPiece;
                tile.hasCharacter = false;
                tile.occupyingPiece = null;
                Debug.Log("Picked up: " + heldPiece.name);
            }
            else
            {
                Debug.Log("No P1 piece to pick up here.");
            }
        }

        // E = drop
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldPiece == null) return;

            Tile dest = board.GetTile(selX, selY);
            if (dest == null) return;

            if (dest.hasCharacter)
            {
                Debug.Log("Tile already occupied!");
                return;
            }

            if (dest.value < 0)
            {
                Debug.Log("Cannot move into P2 territory!");
                return;
            }

            heldPiece.transform.position = dest.WorldPos + Vector3.up * 0.3f;
            dest.hasCharacter = true;
            dest.occupyingPiece = heldPiece;

            ApplyInfluence(dest, heldPiece);
            Debug.Log("Dropped: " + heldPiece.name);

            heldPiece = null;

            // tell TurnManager this player is done for this round
            onMoveComplete?.Invoke();
        }
    }

    // ---------------- INFLUENCE LOGIC ----------------
    void ApplyInfluence(Tile centerTile, GameObject piece)
    {
        int power = 1;
        string pName = piece.name.ToLower();

        if (pName.Contains("queen")) power = 2;
        else if (pName.Contains("knightleft")) power = 2;
        else if (pName.Contains("knightright")) power = 1;

        if (pName.Contains("queen"))
            ApplyQueenInfluence(centerTile, power);
        else
            ApplyKnightInfluence(centerTile, power);

        centerTile.UpdateAppearance();
    }

    // Queen: DEFEND = small + strong, ATTACK = big + weak (1 per tile)
    void ApplyQueenInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        if (currentMode == Mode.Defend)
        {
            // fewer tiles, higher numbers
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 1))
                t.AddInfluence(power);
        }
        else // ATTACK
        {
            // more tiles, but always +1
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 2))
                t.AddInfluence(1);
        }
    }

    // Knights: DEFEND = close + power, ATTACK = wider + 1s
    void ApplyKnightInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        if (currentMode == Mode.Defend)
        {
            // smaller area, stronger pushes (uses piece power)
            Tile L = board.GetTile(cx - 1, cy);
            Tile R = board.GetTile(cx + 1, cy);
            if (L != null) L.AddInfluence(power);
            if (R != null) R.AddInfluence(power);
        }
        else // ATTACK
        {
            // more tiles, all +1
            Tile L1 = board.GetTile(cx - 1, cy);
            Tile R1 = board.GetTile(cx + 1, cy);
            Tile L2 = board.GetTile(cx - 2, cy);
            Tile R2 = board.GetTile(cx + 2, cy);

            if (L1 != null) L1.AddInfluence(1);
            if (R1 != null) R1.AddInfluence(1);
            if (L2 != null) L2.AddInfluence(1);
            if (R2 != null) R2.AddInfluence(1);
        }
    }
}
