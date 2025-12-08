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


    void HandleSelectorMovement()
    {
        if (Input.GetKeyDown(KeyCode.W)) selY = Mathf.Clamp(selY - 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.S)) selY = Mathf.Clamp(selY + 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.A)) selX = Mathf.Clamp(selX - 1, 0, 4);
        if (Input.GetKeyDown(KeyCode.D)) selX = Mathf.Clamp(selX + 1, 0, 4);
        
       Tile t = board.GetTile(selX, selY);
selector.position = t.WorldPos + Vector3.up * 0.2f;

    }
void HandlePickDrop()
{
    Tile tile = board.GetTile(selX, selY);

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

    if (Input.GetKeyDown(KeyCode.E))
    {
        if (heldPiece == null) return;

        Tile dest = board.GetTile(selX, selY);

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
    }
}


    void ApplyInfluence(Tile centerTile, GameObject piece)
    {
        // Identify which piece and its power
        int power = 1;
        string pName = piece.name.ToLower();

        if (pName.Contains("queen")) power = 2;
        else if (pName.Contains("knightleft")) power = 2;
        else if (pName.Contains("knightright")) power = 1;

        if (piece.name.ToLower().Contains("queen"))
            ApplyQueenInfluence(centerTile, power);
        else
            ApplyKnightInfluence(centerTile, power);

        centerTile.UpdateAppearance();
    }

    void ApplyQueenInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        // DEFEND: radius 1 (8 tiles)
        if (currentMode == Mode.Defend)
        {
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 1))
                t.AddInfluence(power);
        }
        else // ATTACK: radius 2
        {
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 2))
                t.AddInfluence(power);
        }
    }

    void ApplyKnightInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        if (power == 1)
        {
            // Affects 1 tile sideways
            Tile L = board.GetTile(cx - 1, cy);
            Tile R = board.GetTile(cx + 1, cy);
            if (L != null) L.AddInfluence(1);
            if (R != null) R.AddInfluence(1);
        }
        else if (power == 2)
        {
            if (currentMode == Mode.Defend)
            {
                Tile L = board.GetTile(cx - 1, cy);
                Tile R = board.GetTile(cx + 1, cy);
                if (L != null) L.AddInfluence(2);
                if (R != null) R.AddInfluence(2);
            }
            else // ATTACK
            {
                Tile L2 = board.GetTile(cx - 2, cy);
                Tile R2 = board.GetTile(cx + 2, cy);
                if (L2 != null) L2.AddInfluence(1);
                if (R2 != null) R2.AddInfluence(1);
            }
        }
    }
}
