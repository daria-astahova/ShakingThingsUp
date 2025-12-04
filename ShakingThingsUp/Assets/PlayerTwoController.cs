using UnityEngine;

public class PlayerTwoController : MonoBehaviour
{
    public BoardManager board;

    public Transform selector;
    public int selX = 2;
    public int selY = 0;

    public GameObject queen;
    public GameObject knightLeft;
    public GameObject knightRight;

    private GameObject heldPiece = null;

    private enum Mode { Attack, Defend }
    private Mode currentMode = Mode.Defend;

    void Update()
    {
        HandleModeInput();
        HandleSelectorMovement();
        HandlePickDrop();
    }

    void HandleModeInput()
    {
        if (Input.GetKeyDown(KeyCode.J))
            currentMode = Mode.Attack;

        if (Input.GetKeyDown(KeyCode.L))
            currentMode = Mode.Defend;
    }

    void HandleSelectorMovement()
    {
        if (Input.GetKeyDown(KeyCode.I)) selY = Mathf.Clamp(selY - 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.K)) selY = Mathf.Clamp(selY + 1, 0, 7);
        if (Input.GetKeyDown(KeyCode.J)) selX = Mathf.Clamp(selX - 1, 0, 4);
        if (Input.GetKeyDown(KeyCode.L)) selX = Mathf.Clamp(selX + 1, 0, 4);

        Tile t = board.GetTile(selX, selY);
        selector.position = t.WorldPos + Vector3.up * 0.2f;
    }

    void HandlePickDrop()
    {
        Tile tile = board.GetTile(selX, selY);

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (heldPiece != null) return;

            if (tile.value >= 0)
            {
                Debug.Log("Opponent’s square! Cannot pick up.");
                return;
            }

            if (tile.hasCharacter && tile.occupyingPiece.CompareTag("P2"))
            {
                heldPiece = tile.occupyingPiece;
                tile.hasCharacter = false;
                tile.occupyingPiece = null;
                Debug.Log("Picked up: " + heldPiece.name);
            }
            else
            {
                Debug.Log("No P2 piece to pick up here.");
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (heldPiece == null) return;

            Tile dest = board.GetTile(selX, selY);

            if (dest.hasCharacter)
            {
                Debug.Log("Tile already occupied!");
                return;
            }

            if (dest.value >= 0)
            {
                Debug.Log("Must move onto your own territory!");
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

    void ApplyQueenInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        if (currentMode == Mode.Defend)
        {
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 1))
                t.AddInfluence(-power);
        }
        else
        {
            foreach (Tile t in board.GetNeighborsRadius(cx, cy, 2))
                t.AddInfluence(-power);
        }
    }

    void ApplyKnightInfluence(Tile center, int power)
    {
        int cx = center.x;
        int cy = center.y;

        if (power == 1)
        {
            Tile L = board.GetTile(cx - 1, cy);
            Tile R = board.GetTile(cx + 1, cy);
            if (L != null) L.AddInfluence(-1);
            if (R != null) R.AddInfluence(-1);
        }
        else if (power == 2)
        {
            if (currentMode == Mode.Defend)
            {
                Tile L = board.GetTile(cx - 1, cy);
                Tile R = board.GetTile(cx + 1, cy);
                if (L != null) L.AddInfluence(-2);
                if (R != null) R.AddInfluence(-2);
            }
            else
            {
                Tile L2 = board.GetTile(cx - 2, cy);
                Tile R2 = board.GetTile(cx + 2, cy);
                if (L2 != null) L2.AddInfluence(-1);
                if (R2 != null) R2.AddInfluence(-1);
            }
        }
    }
}
