using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;      
    public GameObject tilePrefab;

    [Header("White Pieces")]
    public GameObject pawnWhitePrefab;
    public GameObject rookWhitePrefab;
    public GameObject knightWhitePrefab;
    public GameObject bishopWhitePrefab;
    public GameObject queenWhitePrefab;
    public GameObject kingWhitePrefab;

    [Header("Black Pieces")]
    public GameObject pawnBlackPrefab;
    public GameObject rookBlackPrefab;
    public GameObject knightBlackPrefab;
    public GameObject bishopBlackPrefab;
    public GameObject queenBlackPrefab;
    public GameObject kingBlackPrefab;

    public static Board instance; // Singleton for easy access
    private const int BOARD_SIZE = 8;

    private GameObject[,] tiles;
    public ChessFigure[,] pieces;

    private int selectionX = -1, selectionY = -1;
    private ChessFigure selectedPiece;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateBoard();
        SpawnAllPieces();
    }

    void Update()
    {
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedPiece == null)
                {
                    // Select a piece
                    if (pieces[selectionX, selectionY] != null)
                    {
                        selectedPiece = pieces[selectionX, selectionY];
                        HighlightTiles(selectedPiece.PossibleMove());
                    }
                }
                else
                {
                    // Try to move
                    bool[,] moves = selectedPiece.PossibleMove();
                    if (moves[selectionX, selectionY])
                    {
                        MovePiece(selectedPiece, selectionX, selectionY);
                    }
                    ClearHighlights();
                    selectedPiece = null;
                }
            }
        }
    }

    // ---------- Board & Pieces ----------

    void GenerateBoard()
    {
        tiles = new GameObject[BOARD_SIZE, BOARD_SIZE];

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                Renderer rend = tile.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = (x + y) % 2 == 0 ? Color.white : Color.black;
                }

                tiles[x, y] = tile;
            }
        }
    }

    void SpawnAllPieces()
    {
        pieces = new ChessFigure[BOARD_SIZE, BOARD_SIZE];

        // Pawns
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            SpawnPiece(pawnWhitePrefab, x, 1, true);
            SpawnPiece(pawnBlackPrefab, x, 6, false);
        }

        // Rooks
        SpawnPiece(rookWhitePrefab, 0, 0, true);
        SpawnPiece(rookWhitePrefab, 7, 0, true);
        SpawnPiece(rookBlackPrefab, 0, 7, false);
        SpawnPiece(rookBlackPrefab, 7, 7, false);

        // Knights
        SpawnPiece(knightWhitePrefab, 1, 0, true);
        SpawnPiece(knightWhitePrefab, 6, 0, true);
        SpawnPiece(knightBlackPrefab, 1, 7, false);
        SpawnPiece(knightBlackPrefab, 6, 7, false);

        // Bishops
        SpawnPiece(bishopWhitePrefab, 2, 0, true);
        SpawnPiece(bishopWhitePrefab, 5, 0, true);
        SpawnPiece(bishopBlackPrefab, 2, 7, false);
        SpawnPiece(bishopBlackPrefab, 5, 7, false);

        // Queens
        SpawnPiece(queenWhitePrefab, 3, 0, true);
        SpawnPiece(queenBlackPrefab, 3, 7, false);

        // Kings
        SpawnPiece(kingWhitePrefab, 4, 0, true);
        SpawnPiece(kingBlackPrefab, 4, 7, false);
    }

    void SpawnPiece(GameObject prefab, int x, int y, bool isWhite)
    {
        Vector3 position = new Vector3(x, 0.5f, y);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        ChessFigure figure = obj.GetComponent<ChessFigure>();
        figure.SetPosition(x, y);
        figure.isWhite = isWhite;

        pieces[x, y] = figure;
    }

    // ---------- Highlighting ----------

    void HighlightTiles(bool[,] moves)
    {
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                Renderer rend = tiles[x, y].GetComponent<Renderer>();
                if (rend != null)
                {
                    if (moves[x, y])
                        rend.material.color = Color.green;
                    else
                        rend.material.color = (x + y) % 2 == 0 ? Color.white : Color.black;
                }
            }
        }
    }

    void ClearHighlights()
    {
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                Renderer rend = tiles[x, y].GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = (x + y) % 2 == 0 ? Color.white : Color.black;
                }
            }
        }
    }

    // ---------- Selection & Movement ----------

    void UpdateSelection()
    {
        if (!mainCamera) return;

        RaycastHit hit;
        float raycastDistance = 50.0f;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, raycastDistance))
        {
            selectionX = Mathf.FloorToInt(hit.point.x);
            selectionY = Mathf.FloorToInt(hit.point.z);
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    void MovePiece(ChessFigure piece, int x, int y)
    {
        // Capture
        if (pieces[x, y] != null)
        {
            Destroy(pieces[x, y].gameObject);
        }

        pieces[piece.CurrentX, piece.CurrentY] = null;
        piece.SetPosition(x, y);
        piece.transform.position = new Vector3(x, 0.5f, y);
        pieces[x, y] = piece;
    }
}

// ---------- Base & Example Pieces ----------

public abstract class ChessFigure : MonoBehaviour
{
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public bool isWhite;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}

public class Rook : ChessFigure
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];
        int x = CurrentX;
        int y = CurrentY;

        // Right
        for (int i = x + 1; i < 8; i++)
        {
            if (Board.instance.pieces[i, y] == null)
                moves[i, y] = true;
            else
            {
                if (Board.instance.pieces[i, y].isWhite != this.isWhite)
                    moves[i, y] = true;
                break;
            }
        }

        // Left
        for (int i = x - 1; i >= 0; i--)
        {
            if (Board.instance.pieces[i, y] == null)
                moves[i, y] = true;
            else
            {
                if (Board.instance.pieces[i, y].isWhite != this.isWhite)
                    moves[i, y] = true;
                break;
            }
        }

        // Up
        for (int j = y + 1; j < 8; j++)
        {
            if (Board.instance.pieces[x, j] == null)
                moves[x, j] = true;
            else
            {
                if (Board.instance.pieces[x, j].isWhite != this.isWhite)
                    moves[x, j] = true;
                break;
            }
        }

        // Down
        for (int j = y - 1; j >= 0; j--)
        {
            if (Board.instance.pieces[x, j] == null)
                moves[x, j] = true;
            else
            {
                if (Board.instance.pieces[x, j].isWhite != this.isWhite)
                    moves[x, j] = true;
                break;
            }
        }

        return moves;
    }
}
