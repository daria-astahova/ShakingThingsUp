using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // --- FIELDS AND PROPERTIES ---

    [Header("Asset References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject tilePrefab;

    [Header("White Pieces")]
    [SerializeField] private GameObject pawnWhitePrefab;
    [SerializeField] private GameObject rookWhitePrefab;
    [SerializeField] private GameObject knightWhitePrefab;
    [SerializeField] private GameObject bishopWhitePrefab;
    [SerializeField] private GameObject queenWhitePrefab;
    [SerializeField] private GameObject kingWhitePrefab;

    [Header("Black Pieces")]
    [SerializeField] private GameObject pawnBlackPrefab;
    [SerializeField] private GameObject rookBlackPrefab;
    [SerializeField] private GameObject knightBlackPrefab;
    [SerializeField] private GameObject bishopBlackPrefab;
    [SerializeField] private GameObject queenBlackPrefab;
    [SerializeField] private GameObject kingBlackPrefab;

    public static Board instance;

    // CHANGED: Replaced single BOARD_SIZE with public width and length
    public const int BOARD_WIDTH = 5;
    public const int BOARD_LENGTH = 15;

    private GameObject[,] tiles;
    public ChessFigure[,] pieces;

    // Game State
    private ChessFigure selectedPiece;
    private int selectionX = -1, selectionY = -1;
    private bool isWhiteTurn = true;

    // --- UNITY LIFECYCLE ---

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GenerateBoard();
        SpawnAllPieces();
    }

    void Update()
    {
        UpdateSelection();
        HandlePlayerInput();
    }

    // --- INPUT & SELECTION (No changes needed here) ---

    private void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && selectionX >= 0 && selectionY >= 0)
        {
            if (selectedPiece == null)
            {
                SelectPiece(selectionX, selectionY);
            }
            else
            {
                MoveSelectedPiece(selectionX, selectionY);
            }
        }
    }

    private void SelectPiece(int x, int y)
    {
        if (x < 0 || x >= BOARD_WIDTH || y < 0 || y >= BOARD_LENGTH) return; // Boundary check
        ChessFigure piece = pieces[x, y];
        if (piece != null && piece.isWhite == isWhiteTurn)
        {
            selectedPiece = piece;
            UpdateTileHighlights(selectedPiece.PossibleMove());
        }
    }

    private void MoveSelectedPiece(int x, int y)
    {
        bool[,] possibleMoves = selectedPiece.PossibleMove();
        if (possibleMoves[x, y])
        {
            if (pieces[x, y] != null)
            {
                Destroy(pieces[x, y].gameObject);
            }
            pieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;
            pieces[x, y] = selectedPiece;
            selectedPiece.SetPosition(x, y);
            selectedPiece.transform.position = new Vector3(x, 0.5f, y);
            isWhiteTurn = !isWhiteTurn;
        }
        selectedPiece = null;
        UpdateTileHighlights();
    }

    private void UpdateSelection()
    {
        if (!mainCamera) return;
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100f))
        {
            selectionX = Mathf.RoundToInt(hit.point.x);
            selectionY = Mathf.RoundToInt(hit.point.z);
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    // --- BOARD & PIECE GENERATION ---

    void GenerateBoard()
    {
        // CHANGED: Use new constants for array size
        tiles = new GameObject[BOARD_WIDTH, BOARD_LENGTH];
        
        // CHANGED: Loops now use width and length
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int y = 0; y < BOARD_LENGTH; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile ({x}, {y})";
                Renderer rend = tile.GetComponent<Renderer>();
                rend.material.color = (x + y) % 2 == 0 ? Color.white : new Color(0.2f, 0.2f, 0.2f);
                tiles[x, y] = tile;
            }
        }
    }

    // REWRITTEN: The piece layout must be completely changed for the new board size.
    // This is just an example layout, you can customize it however you want!
    void SpawnAllPieces()
    {
        pieces = new ChessFigure[BOARD_WIDTH, BOARD_LENGTH];

        // --- White Pieces (at the "bottom" of the board) ---
        // Pawns on the second rank (y=1)
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            SpawnPiece(pawnWhitePrefab, x, 1);
        }
        // Back rank (y=0)
        // SpawnPiece(rookWhitePrefab, 0, 0);
        // SpawnPiece(knightWhitePrefab, 1, 0);
        // SpawnPiece(kingWhitePrefab, 2, 0); // King in the middle of 5 wide
        // SpawnPiece(bishopWhitePrefab, 3, 0);
        // SpawnPiece(rookWhitePrefab, 4, 0);
        // Add a queen somewhere, for example
        SpawnPiece(queenWhitePrefab, 2, 2);


        // --- Black Pieces (at the "top" of the board) ---
        // Pawns on the second-to-last rank (y=13)
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            SpawnPiece(pawnBlackPrefab, x, BOARD_LENGTH - 2);
        }
        // Back rank (y=14)
        SpawnPiece(rookBlackPrefab, 0, BOARD_LENGTH - 1);
        SpawnPiece(knightBlackPrefab, 1, BOARD_LENGTH - 1);
        SpawnPiece(kingBlackPrefab, 2, BOARD_LENGTH - 1);
        SpawnPiece(bishopBlackPrefab, 3, BOARD_LENGTH - 1);
        SpawnPiece(rookBlackPrefab, 4, BOARD_LENGTH - 1);
        // Add a queen somewhere
        SpawnPiece(queenBlackPrefab, 2, BOARD_LENGTH - 3);
    }

    void SpawnPiece(GameObject prefab, int x, int y)
    {
        Vector3 position = new Vector3(x, 0.5f, y);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        ChessFigure figure = obj.GetComponent<ChessFigure>();
        figure.SetPosition(x, y);
        pieces[x, y] = figure;
    }

    // --- UI & HIGHLIGHTING ---

    void UpdateTileHighlights(bool[,] moves = null)
    {
        // CHANGED: Loops now use width and length
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int y = 0; y < BOARD_LENGTH; y++)
            {
                Renderer rend = tiles[x, y].GetComponent<Renderer>();
                if (moves != null && moves[x, y])
                {
                    rend.material.color = Color.green;
                }
                else
                {
                    rend.material.color = (x + y) % 2 == 0 ? Color.white : new Color(0.2f, 0.2f, 0.2f);
                }
            }
        }
    }
}